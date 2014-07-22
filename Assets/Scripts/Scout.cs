using System.CodeDom;
using System.Linq;
using UnityEngine;
using System.Collections;

public class Scout : MonoBehaviour
{
    private const float _enginePower = 400;
    private const float _topSpeed = 50;
    private const float _turnRate = 25;
    private const float _rollRate = 90;
    private const float _crusingAlt = 100;
    private const float _diveDistance = 250;
    private const float _pullOutDistance = 100;

    private const float _waypointReachDist = 10;
    
    private GameObject _target;
    private Vector3 _waypoint;
    private Transform _mesh;
    private float _birthday;

    private ScoutState _state = ScoutState.EnteringZone;

    void Start()
    {
        _birthday = Time.fixedTime;

        _target = FindNextTarget();
        _mesh = transform.FindChild("scout");
        rigidbody.velocity = transform.forward * 2000;
    }

    void Update()
    {
        if (_target == null)
        {
            _target = FindNextTarget();
        }

        switch (_state)
        {
            case ScoutState.EnteringZone:
                EnteringZone();
                break;
            case ScoutState.Cruising:
                Cruising();
                break;
            case ScoutState.DivingAttack:
                DivingAttack();
                break;
            case ScoutState.ClimbingOut:
                ClimbingOut();
                break;
        }

        UpdatePhysics();
    }

    private void OnDestroy()
    {
        var trail = transform.FindChild("trail");
        trail.transform.parent = null;
        //Destroy(trail.gameObject, trail.GetComponent<TrailRenderer>().time);
    }

    private void UpdatePhysics()
    {
        var deltaMultiplier = Time.deltaTime / Time.fixedDeltaTime;
        var relativeVel = transform.worldToLocalMatrix * rigidbody.velocity;

        rigidbody.AddForce(transform.forward.normalized * _enginePower * deltaMultiplier);

        // drag by facing area
        var dragVector = new Vector3(
            -relativeVel.x * Mathf.Abs(relativeVel.x) * 2f,
            -relativeVel.y * Mathf.Abs(relativeVel.y) * 2f,
            -relativeVel.z * Mathf.Abs(relativeVel.z) * 0.025f
            );
        rigidbody.AddRelativeForce(dragVector * deltaMultiplier);
    }

    private void TurnTowards(Vector3 point)
    {
        var dir = point - transform.position;
        var look = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, look, _turnRate * Time.deltaTime);

        var angle = Vector3.Angle(transform.forward.IgnoreY(), dir.IgnoreY());
        var rollAngle = Mathf.Lerp(0, 90, Mathf.Clamp(angle / 10, 0, 1));

        var rel = transform.worldToLocalMatrix * dir;
        if (rel.x > 0)
            rollAngle = -rollAngle;

        var roll = Quaternion.Euler(0, 0, rollAngle);
        _mesh.transform.localRotation = Quaternion.RotateTowards(_mesh.transform.localRotation, roll, _rollRate * Time.deltaTime);
    }

    private void EnteringZone()
    {
        if (Time.fixedTime - _birthday > 2)
        {
            _state = ScoutState.Cruising;
        }
    }

    private void Cruising()
    {
        var dir = _target.transform.position.IgnoreY(_crusingAlt) - transform.position;
        _waypoint = _target.transform.position.IgnoreY() - (dir.IgnoreY().normalized * _diveDistance);
        _waypoint.y = _crusingAlt;

        TurnTowards(_waypoint);

        var dist = Vector3.Distance(transform.position, _waypoint);
        if (dist < _waypointReachDist)
        {
            //print(Time.fixedTime + ": Diving Attack");
            _waypoint = _target.transform.position;
            _state = ScoutState.DivingAttack;
        }
    }

    private void DivingAttack()
    {
        // turn
        TurnTowards(_target.transform.position);
        var diff = _target.transform.position - transform.position;
        var range = diff.magnitude;
        var angle = Vector3.Angle(transform.forward, diff);
        if (angle < 1 && range < _diveDistance)
        {
            GetComponent<MachineGun>().Fire();
        }

        var dist = Vector3.Distance(transform.position.IgnoreY(), _target.transform.position.IgnoreY());
        if (dist < _pullOutDistance)
        {
            //print(Time.fixedTime + ": Climbing Out");
            _waypoint = transform.position.IgnoreY(_crusingAlt) + (transform.forward.IgnoreY().normalized * _diveDistance * 2);
            _state = ScoutState.ClimbingOut;
        }
    }

    private void ClimbingOut()
    {
        // turn
        TurnTowards(_waypoint);

        var dist = Vector3.Distance(transform.position, _waypoint);
        if (dist < _waypointReachDist)
        {
            _waypoint = _target.transform.position;
            _state = ScoutState.Cruising;
        }
    }

    private void ComingAround()
    {
        // turn
        TurnTowards(_waypoint);

        var dist = Vector3.Distance(transform.position, _waypoint);
        if (dist < _waypointReachDist)
        {
            _waypoint = _target.transform.position;
            _state = ScoutState.ComingAround;
        }
    }

    private void OnDrawGizmos()
    {
        if (_target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _target.transform.position);
        }

        if (_waypoint != Vector3.zero)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, _waypoint);
        }
    }

    private GameObject FindNextTarget()
    {
        var objs = GameObject.FindGameObjectsWithTag("FriendlyStructure");
        return objs[Random.Range(0, objs.Length)];
    }

    private enum ScoutState
    {
        EnteringZone,
        Cruising,
        DivingAttack,
        ClimbingOut,
        ComingAround
    }
}
