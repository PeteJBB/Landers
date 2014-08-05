using UnityEngine;
using System.Collections;

public class StatsDisplay : MonoBehaviour
{
    private float[] _fpsAverages = new float[30];
    private int _fpsIndex = 0;
    private float _fps;

    void Start()
    {
        //for (var i = 0; i < _fpsAverages.Length; i++)
        //    _fpsAverages[i] = 1;
    }

    void Update()
    {
        var avg = _fpsAverages[_fpsIndex];
        var diff = Time.deltaTime - avg;
        avg += diff / _fpsAverages.Length;

        _fpsIndex = (_fpsIndex + 1) % _fpsAverages.Length;
        _fpsAverages[_fpsIndex] = avg;
        _fps = 1 / avg;
    }

    private void OnGUI()
    {
        // FPS
        GUI.TextArea(new Rect(120, 20, 100, 20), "FPS: " + _fps.ToString("0"), Utility.BasicGuiStyle);
    }
}
