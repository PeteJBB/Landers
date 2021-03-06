﻿using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ResupplyArea : MonoBehaviour
{
    private const float _resupplyDelay = 0.1f;
    private float _lastResupplyTime;

    private const float _mgResupplyRate = 15;
    private const float _missileResupplyRate = 1;
    private const float _healthResupplyRate = 5;

    public readonly List<GameObject> CurrentlyResupplying = new List<GameObject>();
 
    void Start()
    {

    }

    void Update()
    {
        if (Time.fixedTime - _lastResupplyTime > _resupplyDelay)
        {
            foreach (var g in CurrentlyResupplying)
            {
                if (g.rigidbody.velocity.magnitude <= 0.1f)
                {
                    var mg = g.GetComponent<MachineGun>();
                    if (mg.Ammo < mg.MaxAmmo)
                        mg.Ammo = (int) Mathf.Min(mg.MaxAmmo, mg.Ammo + _mgResupplyRate);

                    var msl = g.GetComponent<MissileLauncher>();
                    if (msl.Ammo < msl.MaxAmmo)
                        msl.Ammo = (int)Mathf.Min(msl.MaxAmmo, msl.Ammo + _missileResupplyRate);


                    var dam = g.GetComponent<Damageable>();
                    dam.Health = (int) Mathf.Min(dam.MaxHealth, dam.Health + _healthResupplyRate);
                }
            }

            _lastResupplyTime = Time.fixedTime;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player" && !CurrentlyResupplying.Contains(col.gameObject))
            CurrentlyResupplying.Add(col.gameObject);
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player" && CurrentlyResupplying.Contains(col.gameObject))
            CurrentlyResupplying.Remove(col.gameObject);
    }
}
