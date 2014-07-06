using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class MachineGun : MonoBehaviour
{
    public GameObject BulletPrefab;
    public Quaternion AimRotation;

    private float _fireDelay = 0.1f;
    private float _lastFireTime = 0;
    private bool _isNextBulletOnLeft = false;

    void Start()
    {
        
    }

    void Update()
    {
        if (InputManager.GetButton(InputAxis.Fire) && Time.fixedTime - _lastFireTime > _fireDelay)
        {
            //Screen.lockCursor = true;
            //Screen.showCursor = false;
            Fire();
        }
    }

    private void Fire()
    {
        var pos = transform.position + transform.forward - transform.up;
        pos += _isNextBulletOnLeft ? -transform.right : transform.right;
        
        var b = (GameObject)Instantiate(BulletPrefab, pos, AimRotation);
        b.rigidbody.velocity = rigidbody.velocity;
        b.rigidbody.AddForce(b.transform.forward * 400);

        _isNextBulletOnLeft = !_isNextBulletOnLeft;
        _lastFireTime = Time.fixedTime;
    }
}
