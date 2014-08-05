using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Baddie : MonoBehaviour
{
    private const float _attackRange = 20;
    private const float _attackDelay = 4; // time between attacks
    private const float _beamTime = 2; // time beam is on during attack
    private const float _idleWaitTime = 3; // when idle, wait this long before looking for something to do

    private float _lastAttackTime;
    private float _idleStartTime;
    
    private LineRenderer _beam;
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
        _beam = GetComponent<LineRenderer>();

        _turret = transform.FindChild("Turret");
        _machineGun = _turret.GetComponent<MachineGun>();

        _visibilityTestLayerMask = LayerMask.GetMask(new[] { "Terrain", "Buildings" });
    }

    void Update()
    {
        _terrain = Utility.GetTerrainByWorldPos(transform.position);
        if (_terrain == null)
            return;

        //var threat = GetNearestThreat();
        //if (threat != null)
        //{
        //    _target = threat;
        //    _beam.enabled = false;
        //    _state = BaddieState.Defending;
        //}
        //else if (_state == BaddieState.Defending)
        //{
        //    // threat has passed
        //    _state = BaddieState.Idle;
        //}

        if (_state == BaddieState.Idle)
        {
            if (Time.fixedTime - _idleStartTime > _idleWaitTime)
            {
                // look for something to do
                _target = GetNearestTarget();
                if (_target != null)
                {
                    _agent.SetDestination(_target.transform.position);
                    _state = BaddieState.MovingToTarget;
                }
                else
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
        else if (_state == BaddieState.MovingToTarget)
        {
            if (_target == null)
            {
                // no target set or target destroyed
                _state = BaddieState.Idle;
                _idleStartTime = Time.fixedTime;
                return;
            }

            // look at target
            TurnTurretTowards(_target.transform.position);

            var distToTarget = Vector3.Distance(transform.position, _target.transform.position);
            if (distToTarget < _attackRange)
            {
                // in range, attack
                _agent.SetDestination(transform.position);
                _state = BaddieState.Attacking;
            }
        }
        else if (_state == BaddieState.Attacking)
        {
            if (_target == null)
            {
                // no target set or target destroyed
                _state = BaddieState.Idle;
                _idleStartTime = Time.fixedTime;
                _beam.enabled = false;
                return;
            }

            // look at target
            TurnTurretTowards(_target.transform.position);

            if (!_beam.enabled)
            {
                var angle = Vector3.Angle(_turret.forward, _target.transform.position - _turret.position);
            
                if (angle < 10 && Time.fixedTime - _lastAttackTime > _attackDelay + _beamTime)
                {
                    // turn beam on
                    _beam.useWorldSpace = true;
                    var start = transform.position + (Vector3.up * 1.3f);
                    var end = _target.transform.position;
                    end.y = start.y;
                    _beam.SetPosition(0, start);
                    _beam.SetPosition(1, end);
                    _beam.enabled = true;
                    _lastAttackTime = Time.fixedTime;
                }
            }
            else
            {
                if (Time.fixedTime > _lastAttackTime + _beamTime)
                {
                    // turn beam off and apply damage
                    _beam.enabled = false;
                    _target.GetComponent<Damageable>().ApplyDamage(50, gameObject);

                    // move
                    var dest = GetRandomDestination(_target.transform.position, _attackRange);
                    _agent.SetDestination(dest);
                }
                else
                {
                    // update beam pos
                    var start = _turret.position;
                    var end = _target.transform.position;
                    end.y = start.y;
                    _beam.SetPosition(0, start);
                    _beam.SetPosition(1, end);
                }
            }
        }
        else if (_state == BaddieState.Defending)
        {
            // move around
            if (Vector3.Distance(transform.position, _agent.destination) < 0.1f)
            {
                var dest = GetRandomDestination(transform.position, _attackRange);
                _agent.SetDestination(dest);
            }

            // shoot at player
            TurnTurretTowards(_target.transform.position);
            var angle = Vector3.Angle(_turret.forward, _target.transform.position - _turret.position);
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
