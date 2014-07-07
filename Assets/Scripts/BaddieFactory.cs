using UnityEngine;
using System.Collections;

public class BaddieFactory : MonoBehaviour
{
    public GameObject BaddiePrefab;

    private const float SpawnTime = 15;

    private float _nextSpawnTime;

    void Start()
    {
        _nextSpawnTime = Time.fixedTime + SpawnTime;
    }

    void Update()
    {
        if (Time.fixedTime >= _nextSpawnTime)
        {
            var baddie = (GameObject)Instantiate(BaddiePrefab, transform.position, Quaternion.Euler(0, Random.Range(0, 359), 0));
            _nextSpawnTime = Time.fixedTime + SpawnTime;
        }
    }
}
