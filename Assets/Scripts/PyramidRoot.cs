using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class PyramidRoot : MonoBehaviour
{
    public Vector3 StartDirection;

    private LineRenderer _lineRenderer;
    private int _lineVertexCount;
    private List<Vector3> _lineVertices;

    private float _nextLineBend;
    private Vector3 _lineDirection;

    // Use this for initialization
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
       
        _lineDirection = StartDirection.normalized * 0.01f;
        _nextLineBend = Time.fixedTime + Random.Range(1, 5);

        var v1 = transform.position;
        var v2 = v1 + _lineDirection;
        _lineVertices = new List<Vector3>() { v1, v2 };
        _lineRenderer.SetVertexCount(_lineVertices.Count);
        _lineRenderer.SetPosition(0, v1);
        _lineRenderer.SetPosition(1, v2);

        Update();
    }

    // Update is called once per frame
    void Update()
    {
        GrowRoots();
        //PaintTerrain();
    }

    private void GrowRoots()
    {
        if (Time.fixedTime > _nextLineBend)
        {
            // new vert and make a turn!
            _lineDirection = Quaternion.Euler(0, Random.Range(-45, 45), 0) * _lineDirection;
            var newV = _lineVertices.Last() + _lineDirection;
            newV.y = 0;
            newV.y = Terrain.activeTerrain.SampleHeight(newV) + Terrain.activeTerrain.GetPosition().y;

            _lineVertices.Add(newV);
            _lineRenderer.SetVertexCount(_lineVertices.Count);
            _nextLineBend = Time.fixedTime + Random.Range(1, 10);
            _lineRenderer.SetPosition(_lineVertices.Count - 1, newV);
        }
        else
        {
            // keep growing in same dir
            var v = _lineVertices.Last();
            v = v + _lineDirection;
            _lineVertices.RemoveAt(_lineVertices.Count - 1);
            _lineVertices.Add(v);
            _lineRenderer.SetPosition(_lineVertices.Count - 1, v);
        }
    }


    private int _lastPaintX, _lastPaintY;
    private void PaintTerrain()
    {
        // paint terrain
        var terrain = Terrain.activeTerrain;
        var alphaCoords = Utility.GetAlphaMapCoords(_lineVertices.Last(), terrain);

        var mapX = (int)Mathf.Round(alphaCoords.x);
        var mapY = (int)Mathf.Round(alphaCoords.y);

        if (_lastPaintX != mapX || _lastPaintY != mapY)
        {
            var data = terrain.terrainData.GetAlphamaps(mapX, mapY, 1, 1);
            data[0, 0, 0] = 0;
            data[0, 0, 1] = 1;
            terrain.terrainData.SetAlphamaps(mapX, mapY, data);

            _lastPaintX = mapX;
            _lastPaintY = mapY;
        }
    }
}
