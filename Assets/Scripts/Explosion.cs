using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour
{
    private Transform _smoke;
    private Transform _fireball;
    private Transform _dirt;
    private float _birthday;
    private bool _isSmoking = true;

    private const float LifeTime = 10;

    void Start()
    {
        _smoke = transform.FindChild("smoke");
        _fireball = transform.FindChild("fireball");

        _birthday = Time.fixedTime;
    }

    void Update()
    {
        if (_isSmoking && Time.fixedTime - _birthday > LifeTime)
        {
            _smoke.particleSystem.Stop();
            _isSmoking = false;
            Destroy(_smoke.gameObject, _smoke.particleSystem.startLifetime);
        }
    }
}
