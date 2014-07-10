using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

public class Dropship : MonoBehaviour
{
    private Vector3 _landingSite;

    private const float _crusingSpeed = 100;
    private const float _approachDist = 500;
    private const float _flareStartDist = 200;
    private const float _flareEndDist = 30;

    private float _landTime;
    private const float _timeOnGround = 5;

    private Vector3 _exitPoint;
    private const float _exitTime = 8;
    private const float _takeoffSpeed = 30;
    private float _exitSpeed = 200;

    private Vector3 _destXZ;
    private DropShipState _state;

    void Start()
    {
        _state = DropShipState.Cruising;
        
        _landingSite = new Vector3(-1608.959f, 0, -57.52983f);
        _landingSite.y = Utility.GetTerrainHeight(_landingSite);
    }

    void Update()
    {
        _destXZ = _landingSite;
        _destXZ.y = transform.position.y;

        switch(_state)
        {
            case DropShipState.Cruising:
                Cruising();
                break;
            case DropShipState.OnApproach:
                OnApproach();
                break;
            case DropShipState.Landed:
                Landed();
                break;
            case DropShipState.Leaving:
                Leaving();
                break;
        }
    }

    /// <summary>
    /// Flying level at high speed towards landing site
    /// </summary>
    private void Cruising()
    {
        // move
        var moveDir = _landingSite - transform.position;
        transform.position += moveDir.normalized * _crusingSpeed * Time.deltaTime;

        // rotation
        transform.rotation = Quaternion.LookRotation(moveDir);

        var dist = Vector3.Distance(transform.position, _destXZ);
        if (dist < _approachDist)
        {
            print("Approach");
            _state = DropShipState.OnApproach;
        }
    }

    /// <summary>
    /// Descending towards landing site, flare at the end
    /// </summary>
    private void OnApproach()
    {
        var dir = _destXZ - transform.position;
        var dist = dir.magnitude;

        // rotation
        var yaw = Quaternion.LookRotation(dir).eulerAngles.y;
        var pitch = 0f;
        if (dist < _flareEndDist)
        {
            pitch = Mathf.Lerp(0, -20, dist / _flareEndDist);
        }
        else if (dist < _flareStartDist)
        {
            var d = dist - _flareEndDist;
            var f = _flareStartDist - _flareEndDist;
            pitch = Mathf.Lerp(-20, 0, d / f);
        }

        transform.rotation = Quaternion.Euler(pitch, yaw, 0);

        // calc speed
        var speed = _crusingSpeed;
        if (dist < _flareStartDist)
            speed = Mathf.Lerp(1, _crusingSpeed, dist / _flareStartDist);

        // move
        var moveDir = _landingSite - transform.position;
        transform.position += moveDir.normalized * speed * Time.deltaTime;

        if (dist < speed * Time.deltaTime)
        {
            print("Landed");
            _landTime = Time.fixedTime;
            _state = DropShipState.Landed;
        }
    }

    private void Landed()
    {
        if (Time.fixedTime - _landTime > _timeOnGround)
        {
            print("Leaving");
            _exitPoint = _landingSite + (transform.forward * -1000) + (transform.right * -500);
            _exitPoint.y = 1000;
            _state = DropShipState.Leaving;
        }
    }

    private void Leaving()
    {
        var time = Time.fixedTime - _landTime - _timeOnGround;
        if (time < _exitTime)
        {
            // ascend and rotate
            var speed = Mathf.Lerp(1, _takeoffSpeed, time / _exitTime);
            var moveDir = _exitPoint - transform.position;
            transform.position += moveDir.normalized * speed * Time.deltaTime;

            var dir = _exitPoint - transform.position;
            var rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, 20 * Time.deltaTime);
        }
        else
        {
            print("exit");
            // exit to space
            var moveDir = _exitPoint - transform.position;
            transform.position += moveDir.normalized * _crusingSpeed * Time.deltaTime;
        }

        var distToDest = Vector3.Distance(transform.position, _exitPoint);
        if (distToDest < 1)
        {
            Destroy(gameObject);
        }
    }

    private enum DropShipState
    {
        Cruising,
        OnApproach,
        Landed,
        Leaving
    }
}
