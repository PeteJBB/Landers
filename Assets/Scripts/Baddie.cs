﻿using System;
using System.Linq;
using System.Runtime.InteropServices;
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

    
    void Start()
    {
        _state = BaddieState.Idle;
        _agent = GetComponent<NavMeshAgent>();
        _beam = GetComponent<LineRenderer>();
    }

    void Update()
    {
        _terrain = Utility.GetTerrainByWorldPos(transform.position);
        if (_terrain == null)
            return;

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

            if (!_beam.enabled)
            {
                if (Time.fixedTime - _lastAttackTime > _attackDelay + _beamTime)
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
                    // turn beam off and move
                    _beam.enabled = false;

                    // define a circle around the target and move to a point on it
                    var radius = _attackRange - 2;
                    var angle = Random.Range(0, 360);
                    var x = Mathf.Sin(angle) * radius;
                    var z = Mathf.Cos(angle) * radius;

                    var point = new Vector3(x, 0, z) + _target.transform.position;
                    point.y = _terrain.SampleHeight(point);
                    _agent.SetDestination(point);
                }
                else
                {
                    // update beam pos
                    var start = transform.position + (Vector3.up * 1.3f);
                    var end = _target.transform.position;
                    end.y = start.y;
                    _beam.SetPosition(0, start);
                    _beam.SetPosition(1, end);
                }
            }
        }
    }

    private void Fire()
    {
        _beam.enabled = true;
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
        for (var i = 1; i < targets.Length; i++)
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
