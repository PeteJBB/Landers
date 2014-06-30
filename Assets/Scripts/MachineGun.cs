using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class MachineGun : MonoBehaviour
{
    public GameObject BulletPrefab;

    private float _fireDelay = 0.1f;
    private float _lastFireTime = 0;
    private bool _isNextBulletOnLeft = false;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space) && Time.fixedTime - _lastFireTime > _fireDelay)
        {
            Fire();
        }
    }

    private void Fire()
    {
        var pos = transform.position + transform.forward - transform.up;
        pos += _isNextBulletOnLeft ? -transform.right : transform.right;
        
        var b = (GameObject)Instantiate(BulletPrefab, pos, transform.rotation);
        b.rigidbody.velocity = rigidbody.velocity;
        b.rigidbody.AddForce(transform.forward * 200);

        _isNextBulletOnLeft = !_isNextBulletOnLeft;
        _lastFireTime = Time.fixedTime;
    }
}
