using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class Pyramid : MonoBehaviour
{
    private Transform _structure;
    
    private float _birthday;
    private float _buildTime = 10;

    private float _paintDelay = 1;
    private float _lastPaintTime = 0;
    private int _nextPaintSize = 3;

    

    void Start()
    {
        _structure = transform.FindChild("structure");
        _birthday = Time.fixedTime;
        //PaintTerrain();
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

        //if(Time.fixedTime - _lastPaintTime > _paintDelay)
        //{
        //    PaintTerrain();
        //    _nextPaintSize += 2;
        //    _lastPaintTime = Time.fixedTime;
        //}

    }

    private void PaintTerrain()
    {
        // paint terrain
        var terrain = Terrain.activeTerrain;
        var alphaCoords = Utility.GetAlphaMapCoords(transform.position, terrain);

        var mapX = (int)Mathf.Round(alphaCoords.x - (_nextPaintSize / 2f));
        var mapY = (int)Mathf.Round(alphaCoords.y - (_nextPaintSize / 2f));

        var data = terrain.terrainData.GetAlphamaps(mapX, mapY, _nextPaintSize, _nextPaintSize);
        for (var x = 0; x < _nextPaintSize; x++)
        {
            for (var y = 0; y < _nextPaintSize; y++)
            {
                data[x, y, 0] = 0;
                data[x, y, 1] = 1;
            }
        }
        terrain.terrainData.SetAlphamaps(mapX, mapY, data);
    }
}
