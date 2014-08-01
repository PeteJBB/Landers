using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HeatSeeker : MonoBehaviour 
{
    public float ScanDelay = 0.1f;
    public float TurnSpeed = 60; // degrees per second
    public float MaxDetectionAngle = 45;

    private float _lastScan;
    private GameObject _target;
    private int _visibilityTestLayerMask;

    void Start()
    {
        //_lastScan = Time.fixedTime;
        _visibilityTestLayerMask = LayerMask.GetMask(new[] { "Terrain", "Buildings" });
    }

	void Update () 
    {
        if (Time.fixedTime - _lastScan > ScanDelay)
        {
            // look for target
            _target = FindNewTarget();
            _lastScan = Time.fixedTime;
        }

        if (_target != null)
        {
            var leadPoint = ComputLeadPoint();
            var lookRot = Quaternion.LookRotation(_target.transform.position - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, TurnSpeed * Time.deltaTime);
        }
	}

    void OnDrawGizmos()
    {
        if (_target != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, ComputLeadPoint());
        }
    }

    private Vector3 ComputLeadPoint()
    {
        var leadPoint = _target.transform.position;
        if (_target.rigidbody != null)
        {
            //var dist = Vector3.Distance(transform.position, _target.transform.position);
            //var relativeVel = transform.worldToLocalMatrix * (rigidbody.velocity - _target.rigidbody.velocity);
            //var bulletTimeToTarget = relativeVel.z / dist * Time.fixedDeltaTime;
            //leadPoint += _target.rigidbody.velocity * bulletTimeToTarget;

            var dist = Vector3.Distance(transform.position, _target.transform.position);
            var velocity = rigidbody.velocity.magnitude;
            var bulletTimeToTarget = dist / velocity; // *Time.fixedDeltaTime;
            leadPoint = _target.transform.position + (_target.rigidbody.velocity * bulletTimeToTarget);
        }
        return leadPoint;
    }


    private GameObject FindNewTarget()
    {
        var detections = new List<Detection>();
        foreach (var obj in GameObject.FindGameObjectsWithTag("EnemyPlane"))
        {
            var angle = Vector3.Angle(transform.forward, obj.transform.position - transform.position);
            if (angle < MaxDetectionAngle)
            {
                detections.Add(new Detection() { angle = angle, gameObject = obj });
            }
        }
        detections = detections.OrderBy(x => x.angle).ToList();

        foreach (var d in detections)
        {
            if (!Physics.Linecast(transform.position, d.gameObject.transform.position, _visibilityTestLayerMask))
            {
                return d.gameObject;
            }
        }

        return null;
    }

    private class Detection
    {
        public float angle;
        public GameObject gameObject;
    }
}
