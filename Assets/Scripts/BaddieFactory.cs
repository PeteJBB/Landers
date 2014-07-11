using System.Linq;
using UnityEngine;
using System.Collections;

public class BaddieFactory : MonoBehaviour
{
    public GameObject[] Prefabs;
    public int[] SpawnWeighting;
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
    }

    public void SpawnBaddie(Vector3 position)
    {
        // get a random prefab to spawn
        var i = Random.Range(0, _prefabPool.Length);
        var prefab = _prefabPool[i];
        var baddie = (GameObject)Instantiate(prefab, transform.position, Quaternion.identity);
    }
}
