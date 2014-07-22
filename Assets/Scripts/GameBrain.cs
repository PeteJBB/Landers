using UnityEngine;
using System.Collections;

public class GameBrain : MonoBehaviour
{
    public static GameView CurrentView = GameView.Internal;

    public GameObject ScoutPrefab;

    private int _currentWave = 0;
    private float _waveCompleteTime;
    private bool _isWaveComplete = true;

    private const float _timeBetweenWaves = 10;

    private const float LanderSpawnDelay = 20;
    
    private int _spawnCount = 0;
    private float _lastSpawn;

    void Start()
    {
        _waveCompleteTime = Time.fixedTime - 5;
        _lastSpawn = Time.fixedTime;
    }


    void Update()
    {
        if (Time.fixedTime - _lastSpawn > LanderSpawnDelay)
        {
            //GetComponent<LanderFactory>().SpawnLander();
            _lastSpawn = Time.fixedTime;
        }

        if (_isWaveComplete)
        {
            if (Time.fixedTime - _waveCompleteTime > _timeBetweenWaves)
            {
                // next wave
                _currentWave++;
                _isWaveComplete = false;

                // spawn ships
                Instantiate(ScoutPrefab, new Vector3(1201, 180, -1499), Quaternion.Euler(0, 317, 0));
                Instantiate(ScoutPrefab, new Vector3(1220, 180, -1505), Quaternion.Euler(0, 317, 0));
                Instantiate(ScoutPrefab, new Vector3(1205, 180, -1519), Quaternion.Euler(0, 317, 0));
                Instantiate(ScoutPrefab, new Vector3(1207, 180, -1538), Quaternion.Euler(0, 317, 0));
            }
        }
        else if (FindObjectsOfType<Scout>().Length == 0)
        {
            _isWaveComplete = true;
            _waveCompleteTime = Time.fixedTime;
        }
    }

    private void OnGUI()
    {
        if (_isWaveComplete)
        {
            var message = _currentWave == 0
                ? "Get ready for first wave"
                : "You got 'em! Prepare for Wave " + (_currentWave + 1) + "!";

            var rext = Utility.GetCenteredRectangle(new Vector2(Screen.width / 2f, Screen.height / 4f), 320, 64);
            GUI.TextArea(rext, message, Utility.BigMessageGuiStyle);
        }
    }

}

public enum GameView
{
    Internal,
    External,
    Map,
    Menu
}