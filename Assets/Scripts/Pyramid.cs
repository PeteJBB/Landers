using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class Pyramid : MonoBehaviour
{
    private Transform _structure;
    
    private float _birthday;
    private const float _buildTime = 15;

    private const float _baddieSpawnTime = 10;
    private float _nextSpawnTime;
    public LandingSite LandingSite;

    void Start()
    {
        _structure = transform.FindChild("structure");
        _birthday = Time.fixedTime;
        _nextSpawnTime = Time.fixedTime + _baddieSpawnTime;
    }

    void Update()
    {
        var age = Time.fixedTime - _birthday;
        if (age <= _buildTime)
        {
            // build structure
            var y = Mathf.Lerp(-_structure.transform.localScale.y, -0.2f, age/_buildTime);
            var pos = _structure.localPosition;
            pos.y = y;
            _structure.localPosition = pos;
        }

        if (Time.fixedTime > _nextSpawnTime)
        {
            GetComponent<BaddieFactory>().SpawnBaddie(transform.position);
            _nextSpawnTime = Time.fixedTime + _baddieSpawnTime;
        }
    }

    private void OnDestroy()
    {
        LandingSite.IsEngaged = false;
    }
}
