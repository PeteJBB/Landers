using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class MachineGun : MonoBehaviour
{
    public GameObject BulletPrefab;
    public Quaternion AimRotation;
    public float Ammo = 200;
    public float MaxAmmo = 200;

    private const float _fireDelay = 0.06f;
    private float _lastFireTime = 0;
    private bool _isNextBulletOnLeft = false;
    
    
    void Start()
    {
        
    }

    void Update()
    {
        if (InputManager.GetButton(InputMapping.Fire) && Time.fixedTime - _lastFireTime > _fireDelay)
        {
            Fire();
        }
    }

    private void Fire()
    {
        if (Ammo > 0)
        {
            var pos = transform.position + transform.forward - transform.up;
            pos += _isNextBulletOnLeft ? -transform.right : transform.right;

            var b = (GameObject) Instantiate(BulletPrefab, pos, AimRotation);
            b.rigidbody.velocity = rigidbody.velocity;
            b.rigidbody.AddForce(b.transform.forward * 400);

            var projectile = b.GetComponent<Projectile>();
            projectile.Team = 1;

            Ammo--;
        }

        _isNextBulletOnLeft = !_isNextBulletOnLeft;
        _lastFireTime = Time.fixedTime;
    }

    private void OnGUI()
    {
        GUI.TextArea(new Rect(120, 80, 100, 20), "Ammo: " + Ammo, Utility.BasicGuiStyle);
    }
}
