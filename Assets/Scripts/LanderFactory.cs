using System.Security.Policy;
using UnityEngine;
using System.Collections;

public class LanderFactory : MonoBehaviour 
{
	public GameObject LanderPrefab;

    private const float SpawnDelay = 20;
    private const float SpawnHeight = 1000;

    private int _maxSpawnAtOnce = 4;
    private int _spawnCount = 0;
    private float _lastSpawn;

	void Start () 
	{
		//ResetTerrainColoring();
		_lastSpawn = Time.fixedTime;
	}

	void Update () 
	{
		if(Time.fixedTime - _lastSpawn > SpawnDelay)
		{
		    var count = GameObject.FindGameObjectsWithTag("Lander").Length +
		                GameObject.FindGameObjectsWithTag("Pyramid").Length;

		    if (count < _maxSpawnAtOnce)
		    {
		        // look for available landing site
		        var sites = GameObject.FindGameObjectsWithTag("LandingSite");
		        if (sites.Length > 0)
		        {
		            var selectedSite = sites[Random.Range(0, sites.Length - 1)];

                    // spawn a new lander
                    var lander = (GameObject)Instantiate(LanderPrefab);
                    lander.transform.position = new Vector3(selectedSite.transform.position.x, SpawnHeight, selectedSite.transform.position.z);
		            lander.transform.eulerAngles = new Vector3(-90, 0, 0);
		            lander.rigidbody.velocity = new Vector3(0, -50, 0);

		            Destroy(selectedSite);
		            _spawnCount++;
		        }
		    }

		    _lastSpawn = Time.fixedTime;
		}
	}

	void ResetTerrainColoring()
	{
		// reset splat maps
		var t = Terrain.activeTerrain;
		var map = new float[t.terrainData.alphamapWidth, t.terrainData.alphamapHeight, 2];
		for (var y = 0; y < t.terrainData.alphamapHeight; y++) {
			for (var x = 0; x < t.terrainData.alphamapWidth; x++) {
				map[x, y, 0] = 1;
				map[x, y, 1] = 0;
			}
		}
		
		t.terrainData.SetAlphamaps(0, 0, map);
	}
}
