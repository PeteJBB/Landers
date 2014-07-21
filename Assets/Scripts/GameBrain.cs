using UnityEngine;
using System.Collections;

public class GameBrain : MonoBehaviour
{
    public static GameView CurrentView = GameView.Internal;

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
            GetComponent<LanderFactory>().SpawnLander();
            _lastSpawn = Time.fixedTime;
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