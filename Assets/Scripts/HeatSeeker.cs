using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HeatSeeker : MonoBehaviour 
{
    public float ScanDelay = 0.2f;
    public float TurnSpeed = 60; // degrees per second
    public float MaxDetectionAngle = 45;
    public GameObject Target;
    
    private float _lastScan;
    private int _visibilityTestLayerMask;
    private float _lastDistToTarget;

    void Start()
    {
        //_lastScan = Time.fixedTime;
        _visibilityTestLayerMask = LayerMask.GetMask(new[] { "Terrain", "Buildings" });
    }

	void Update () 
    {
        if (Target != null)
        {
            var leadPoint = ComputLeadPoint();
            var lookRot = Quaternion.LookRotation(Target.transform.position - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, TurnSpeed * Time.deltaTime);

            // proximity fuse - detonate just when near-miss occurs
            var dist = Vector3.Distance(transform.position, Target.transform.position);
            var angle = Vector3.Angle(transform.forward, Target.transform.position - transform.position);
            
            if (dist < 50 && angle > 90)// && _lastDistToTarget < dist)
            {
                // detonate
                print("DETONATE!!!");
                print("LastDist: " + _lastDistToTarget + ", dist: " + dist + ", angle: " + angle);
                GetComponent<Projectile>().Explode(transform.position);
                Destroy(gameObject);
            }
            _lastDistToTarget = dist;
        }

        // rescan
        if (Time.fixedTime - _lastScan > ScanDelay)
        {
            // look for target
            Target = FindNewTarget();
            _lastScan = Time.fixedTime;
        }
	}

    void OnDrawGizmos()
    {
        if (Target != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, ComputLeadPoint());
        }
    }

    private Vector3 ComputLeadPoint()
    {
        var leadPoint = Target.transform.position;
        if (Target.rigidbody != null)
        {
            //var dist = Vector3.Distance(transform.position, _target.transform.position);
            //var relativeVel = transform.worldToLocalMatrix * (rigidbody.velocity - _target.rigidbody.velocity);
            //var bulletTimeToTarget = relativeVel.z / dist * Time.fixedDeltaTime;
            //leadPoint += _target.rigidbody.velocity * bulletTimeToTarget;

            var dist = Vector3.Distance(transform.position, Target.transform.position);
            var velocity = rigidbody.velocity.magnitude;
            var bulletTimeToTarget = dist / velocity; // *Time.fixedDeltaTime;
            leadPoint = Target.transform.position + (Target.rigidbody.velocity * bulletTimeToTarget);
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
