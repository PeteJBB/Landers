using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ResupplyArea : MonoBehaviour
{
    private const float _mgResupplyDelay = 0.02f;
    private float _lastMgResupplyTime;

    private readonly List<GameObject> _guysToResupply = new List<GameObject>();
 
    void Start()
    {

    }

    void Update()
    {
        foreach (var g in _guysToResupply)
        {
            if (Time.fixedTime - _lastMgResupplyTime > _mgResupplyDelay)
            {
                var mg = g.GetComponent<MachineGun>();
                if (mg.Ammo < mg.MaxAmmo)
                    mg.Ammo++;

                _lastMgResupplyTime = Time.fixedTime;
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player" && !_guysToResupply.Contains(col.gameObject))
            _guysToResupply.Add(col.gameObject);
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player" && _guysToResupply.Contains(col.gameObject))
            _guysToResupply.Remove(col.gameObject);
    }
}
