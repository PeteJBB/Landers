using System.Linq;
using System.Security.Policy;
using UnityEngine;
using System.Collections;

public class LanderFactory : MonoBehaviour 
{
	public GameObject LanderPrefab;
    public GameObject DropshipPrefab;

    private const float _spawnHeight = 1000;

	public void SpawnLander() 
	{
		// look for available landing site
        var sites = FindObjectsOfType<LandingSite>().Where(x => !x.IsEngaged).ToArray();
	    if (sites.Length > 0)
	    {
	        var selectedSite = sites[Random.Range(0, sites.Length - 1)];

	        if (Random.Range(0f, 1f) > 0.5f)
	        {
                print(Time.fixedTime + ": Spawned a lander");
	            // spawn a new lander
	            var lander = (GameObject) Instantiate(LanderPrefab);
	            lander.transform.position = new Vector3(selectedSite.transform.position.x, _spawnHeight, selectedSite.transform.position.z);
	            lander.transform.eulerAngles = new Vector3(-90, 0, 0);
	            lander.rigidbody.velocity = new Vector3(0, -50, 0);

	            lander.GetComponent<Lander>().LandingSite = selectedSite;
	            selectedSite.IsEngaged = true;
	        }
	        else
	        {
                print(Time.fixedTime + ": Spawned a dropship");
                // spawn a dropship
                var dropship = (GameObject)Instantiate(DropshipPrefab);
	            var pos = selectedSite.transform.position + (selectedSite.ApproachVector.normalized * 2000);
	            pos.y = _spawnHeight;

	            dropship.transform.position = pos;
                dropship.GetComponent<Dropship>().LandingSite = selectedSite;
                selectedSite.IsEngaged = true;
	        }
	    }
	}
}
