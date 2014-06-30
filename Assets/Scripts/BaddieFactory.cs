using UnityEngine;
using System.Collections;

public class BaddieFactory : MonoBehaviour
{
    public GameObject BaddiePrefab;

    private const float SpawnTimeMin = 3;
    private const float SpawnTimeMax = 10;

    private float _nextSpawnTime;

    void Start()
    {
        _nextSpawnTime = Time.fixedTime + Random.Range(SpawnTimeMin, SpawnTimeMax);
    }

    void Update()
    {
        if (Time.fixedTime >= _nextSpawnTime)
        {
            var baddie = (GameObject)Instantiate(BaddiePrefab, transform.position, Quaternion.Euler(0, Random.Range(0, 359), 0));
            baddie.transform.parent = transform.root;
            _nextSpawnTime = Time.fixedTime + Random.Range(SpawnTimeMin, SpawnTimeMax);
        }
    }
}
