﻿using System;
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

    public LandingSite LandingSite { get; set; }

    void Start()
    {
        _smoke = transform.FindChild("smoke");
    }

    void Update()
    {
        _smoke.particleSystem.startLifetime = Random.Range(SmokeLifeTimeMin, SmokeLifeTimeMax);
    }

    public void OnCollisionEnter(Collision col)
    {
        if (!_hasLanded)
        {
            _hasLanded = true;

            if (col.gameObject.tag == "Terrain")
            {
                 // create pyramid
                var obj = (GameObject)Instantiate(PyramidPrefab);

                var terrain = col.gameObject.GetComponent<Terrain>();
                var pos = transform.position;
                pos.y = terrain.SampleHeight(pos) + terrain.transform.position.y;
                obj.transform.position = pos;

                obj.GetComponent<Pyramid>().LandingSite = LandingSite;
            }

            DetatchParticles();
            Explode();

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
}
