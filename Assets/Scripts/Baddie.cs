using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Baddie : MonoBehaviour
{
    private const float TurnRate = 20;
    private const float MoveSpeed = 1f;

    private float _nextTurn;
    private Quaternion _desiredRotation;
    private Terrain _myTerrain;

    void Start()
    {
        _desiredRotation = transform.rotation;
        _nextTurn = Random.Range(2, 8);
        _myTerrain = transform.root.Find("Terrain").GetComponent<Terrain>();
    }

    void Update()
    {
        if (Time.fixedTime >= _nextTurn)
        {
            ChangeDirection();
        }

        // turn
        transform.rotation = Quaternion.RotateTowards(transform.rotation, _desiredRotation, TurnRate * Time.deltaTime);

        // move forward
        var pos = transform.position + (transform.forward * MoveSpeed * Time.deltaTime);
        pos.y = _myTerrain.SampleHeight(pos) + _myTerrain.GetPosition().y;
        transform.position = pos;

        if (pos.y <= 0)
            ChangeDirection();
    }

    private void ChangeDirection()
    {
        if (transform.position.y <= 0)
        {
            // move back to island
            var dest = transform.root.position;
            _desiredRotation = Quaternion.LookRotation(dest - transform.position, Vector3.up);
        }
        else
        {
            var newAngle = transform.eulerAngles.y + Random.Range(-45, 45);
            _desiredRotation = Quaternion.Euler(0, newAngle, 0);
        }
        _nextTurn = Time.fixedTime + Random.Range(4, 8);
    }
}
