using UnityEngine;
using System.Collections;

public class GameBrain : MonoBehaviour
{
    public static GameView CurrentView = GameView.Internal;

    public GameObject ScoutPrefab;

    private const float LanderSpawnDelay = 20;
    
    private int _spawnCount = 0;
    private float _lastSpawn;

    void Start()
    {
        _lastSpawn = Time.fixedTime;
    }


    void Update()
    {
        if (Time.fixedTime - _lastSpawn > LanderSpawnDelay)
        {
            //GetComponent<LanderFactory>().SpawnLander();
            _lastSpawn = Time.fixedTime;
        }

        if (FindObjectsOfType<Scout>().Length == 0)
        {
            // spawn more
            Instantiate(ScoutPrefab, new Vector3(201, 180, -499), Quaternion.Euler(0, 317, 0));
            Instantiate(ScoutPrefab, new Vector3(220, 180, -505), Quaternion.Euler(0, 317, 0));
            Instantiate(ScoutPrefab, new Vector3(205, 180, -519), Quaternion.Euler(0, 317, 0));
            Instantiate(ScoutPrefab, new Vector3(207, 180, -538), Quaternion.Euler(0, 317, 0));
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