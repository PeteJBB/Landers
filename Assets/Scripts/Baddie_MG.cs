using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Baddie_MG : MonoBehaviour
{
    private const float _evasiveMoveDistance = 10;
    private const float _idleWaitTime = 3; // when idle, wait this long before looking for something to do

    private float _lastAttackTime;
    private float _idleStartTime;
    
    private NavMeshAgent _agent;
    private Terrain _terrain;
    private BaddieState _state;
    private GameObject _target;

    private int _visibilityTestLayerMask;
    
    private Transform _turret;
    private const float _turretSpeed = 45f;
    private MachineGun _machineGun;

    void Start()
    {
        _state = BaddieState.Idle;
        _agent = GetComponent<NavMeshAgent>();

        _turret = transform.FindChild("Turret");
        _machineGun = _turret.GetComponent<MachineGun>();

        _visibilityTestLayerMask = LayerMask.GetMask(new[] { "Terrain", "Buildings" });
    }

    void Update()
    {
        _terrain = Utility.GetTerrainByWorldPos(transform.position);
        if (_terrain == null)
            return;

        var threat = GetNearestThreat();
        if (threat != null)
        {
            _target = threat;
            _state = BaddieState.Defending;
        }
        else if (_state == BaddieState.Defending)
        {
            // threat has passed
            _state = BaddieState.Idle;
        }

        if (_state == BaddieState.Idle)
        {
            if (Time.fixedTime - _idleStartTime > _idleWaitTime)
            {
                // just pick a random point and go there
                var dest = GetRandomDestination();
                var path = new NavMeshPath();
                if (_agent.CalculatePath(dest, path))
                {
                    _agent.SetDestination(dest);
                    _state = BaddieState.Wandering;
                }
            }
        }
        else if (_state == BaddieState.Wandering)
        {
            TurnTurretTowards(_turret.position + transform.forward);
            if (Vector3.Distance(transform.position, _agent.destination) < 1)
            {
                // arrived!
                _state = BaddieState.Idle;
                _idleStartTime = Time.fixedTime;
            }
        }
        else if (_state == BaddieState.Defending)
        {
            // move around
            if (Vector3.Distance(transform.position, _agent.destination) < 0.1f)
            {
                var dest = GetRandomDestination(transform.position, _evasiveMoveDistance);
                _agent.SetDestination(dest);
            }

            // shoot at player
            var aimPoint = ComputLeadPoint(_target);
            TurnTurretTowards(aimPoint);
            var angle = Vector3.Angle(_turret.forward, aimPoint - _turret.position);
            if(angle < 10)
            {
                _machineGun.Fire();
            }
        }
    }

    private void TurnTurretTowards(Vector3 targetPos)
    {
        _turret.rotation = Quaternion.RotateTowards(_turret.rotation, Quaternion.LookRotation(targetPos - _turret.position, Vector3.up), Time.deltaTime * _turretSpeed);
    }

    private Vector3 ComputLeadPoint(GameObject obj)
    {
        var dist = Vector3.Distance(_turret.position, obj.transform.position);
        var velocity = 400;
        var bulletTimeToTarget = velocity / dist * Time.fixedDeltaTime;
        var leadPoint = obj.transform.position + (obj.rigidbody.velocity * bulletTimeToTarget);

        return leadPoint;
    }

    private GameObject GetNearestThreat()
    {
        var threat = GameObject.FindGameObjectWithTag("Player");
        var dist = Vector3.Distance(transform.position, threat.transform.position);
        if (dist < 100)
        {
            
            if (!Physics.Linecast(_turret.position, threat.transform.position, _visibilityTestLayerMask))
            {
                // nothing blocking my view
                return threat;
            }
        }
        return null;
    }

    private GameObject GetNearestTarget()
    {
        var targets = GameObject.FindGameObjectsWithTag("FriendlyStructure");
        if (targets.Length == 0)
        {
            return null;
        }

        var searchArea = GetTerrainRect();

        // find the closest one
        GameObject closest = null;
        float dist = 0;
        for (var i = 0; i < targets.Length; i++)
        {
            var t = targets[i];
            if(!searchArea.Contains(new Vector2(t.transform.position.x, t.transform.position.z)))
                continue;

            if (closest == null)
            {
                closest = t;
                dist = Vector3.Distance(transform.position, t.transform.position);
                continue;
            }

            var d = Vector3.Distance(transform.position, t.transform.position);
            if (d < dist)
            {
                closest = t;
                dist = Vector3.Distance(transform.position, t.transform.position);
            }
        }
        return closest;
    }

    private Vector3 GetRandomDestination()
    {
        var rect = GetTerrainRect();
        var x = Random.Range(rect.xMin, rect.xMax);
        var z = Random.Range(rect.yMin, rect.yMax);
        var dest = new Vector3(x, 0, z);
        dest.y = _terrain.SampleHeight(dest);

        return dest;
    }

    private Vector3 GetRandomDestination(Vector3 center, float radius)
    {
        // define a circle around the center and find a point on it
        var angle = Random.Range(0, 360);
        var x = Mathf.Sin(angle) * radius;
        var z = Mathf.Cos(angle) * radius;

        var dest = center + new Vector3(x, 0, z);
        dest.y = _terrain.SampleHeight(dest);

        return dest;
    }

    private Rect GetTerrainRect()
    {
        var rect = new Rect(
            _terrain.transform.position.x,
            _terrain.transform.position.z,
            _terrain.terrainData.heightmapWidth,
            _terrain.terrainData.heightmapHeight);

        return rect;
    }

    void  OnDrawGizmos()
    {
        if (_agent != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, _agent.destination);
        }
    }

    private enum BaddieState
    {
        Idle,
        Wandering,
        MovingToTarget,
        Attacking,
        Defending
    }
}
