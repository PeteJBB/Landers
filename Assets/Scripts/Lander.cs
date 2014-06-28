using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Lander : MonoBehaviour
{
    private bool _hasLanded = false;
    private Transform _smoke;

    const float SmokeLifeTimeMin = 5;
    const float SmokeLifeTimeMax = 15;

    public GameObject ImpactExplosion;
    public GameObject PyramidPrefab;

    void Start()
    {
        _smoke = transform.FindChild("smoke");
    }

    void Update()
    {
        _smoke.particleSystem.startLifetime = Random.Range(SmokeLifeTimeMin, SmokeLifeTimeMax);
        print(rigidbody.velocity);
    }

    public void OnCollisionEnter(Collision col)
    {
        if (!_hasLanded)
        {
            _hasLanded = true;

            DetatchParticles();
            Explode();
            CreatePyramid(col.contacts[0].point);

            Destroy(gameObject);
        }
        
    }

    private void DetatchParticles()
    {
        var smoke = transform.FindChild("smoke");
        smoke.parent = null;
        smoke.particleSystem.Stop();
        Destroy(smoke.gameObject, SmokeLifeTimeMax);

        var fire = transform.FindChild("fire");
        fire.parent = null;
        fire.particleSystem.Stop();
        Destroy(fire.gameObject, fire.particleSystem.startLifetime);
    }

    private void Explode()
    {
        // explosion
        var exp = (GameObject)Instantiate(ImpactExplosion);
        exp.transform.position = transform.position;
    }

    private void CreatePyramid(Vector3 point)
    {
        var obj = (GameObject)Instantiate(PyramidPrefab);
        obj.transform.position = transform.position;
    }
}
