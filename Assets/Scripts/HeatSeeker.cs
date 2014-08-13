using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HeatSeeker : MonoBehaviour 
{
    public float TurnSpeed = 50; // degrees per second
    public float MaxDetectionAngle = 90;
    public GameObject Target;
    
    private float _lastDistToTarget;
    private Vector3 _mylastPos;
    private Vector3 _targetLastPos;
    
    void Start()
    {
        
    }

	void Update () 
    {
        if (Target != null && CheckTargetLock())
        {
            // turn towards target
            var leadPoint = ComputLeadPoint();
            var lookRot = Quaternion.LookRotation(leadPoint - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, TurnSpeed * Time.deltaTime);

            // proximity fuse - detonate just when near-miss occurs
            var dist = Vector3.Distance(transform.position, Target.transform.position);
            var angle = Vector3.Angle(transform.forward, Target.transform.position - transform.position);
            print("dist: " + dist);
            if (dist < 20 && angle > 90)
            {
                // detonate
                GetComponent<Projectile>().Explode(transform.position);
                Destroy(gameObject);
            }
            _lastDistToTarget = dist;
            _targetLastPos = Target.transform.position;
        }
        _mylastPos = transform.position;
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
        var myVel = (transform.position - _mylastPos) / Time.deltaTime;
        var targetVel = (Target.transform.position - _targetLastPos) / Time.deltaTime;

        var relativeVel = rigidbody.velocity - targetVel;
        var dist = Vector3.Distance(transform.position, Target.transform.position);

        var bulletTimeToTarget = dist / relativeVel.magnitude;
        var leadPoint = Target.transform.position + (targetVel * bulletTimeToTarget);

        return leadPoint; 
    }

    private bool CheckTargetLock()
    {
        var dist = Vector3.Distance(transform.position, Target.transform.position);

        // check if obj is in front of me
        var angle = Vector3.Angle(transform.forward, Target.transform.position - transform.position);
        if (angle > MaxDetectionAngle)
        {
            return false;
        }

        // check if i am in targets visible cone
        //var orientation = Vector3.Angle(Target.transform.forward, transform.position - Target.transform.position);
        //if (orientation > Target.GetComponent<HeatSeekerTarget>().Angle)
        //{
        //    print(Time.fixedTime + ": Lost missile lock!");
        //    return false;
        //}

        return true;
    }
}
