using System.Linq;
using UnityEngine;
using System.Collections;

public class BaddieFactory : MonoBehaviour
{
    public GameObject[] Prefabs;
    public int[] SpawnWeighting;

    private const float SpawnTime = 10;

    private float _nextSpawnTime;
    private GameObject[] _prefabPool;

    void Start()
    {
        var totalCount = SpawnWeighting.Sum(x => x);
        _prefabPool = new GameObject[totalCount];

        var index = 0;
        for (var p = 0; p < Prefabs.Length; p++)
        {
            for (var i=0; i<SpawnWeighting[p]; i++)
            {
                _prefabPool[index] = Prefabs[p];
                index++;
            }
        }

        _nextSpawnTime = Time.fixedTime + SpawnTime;
    }

    void Update()
    {
        if (Time.fixedTime >= _nextSpawnTime)
        {
            // get a random prefab to spawn
            var i = Random.Range(0, _prefabPool.Length);
            var prefab = _prefabPool[i];
            var baddie = (GameObject)Instantiate(prefab, transform.position, Quaternion.identity);
            _nextSpawnTime = Time.fixedTime + SpawnTime;
        }
    }
}
