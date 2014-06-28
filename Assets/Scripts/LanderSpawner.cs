using UnityEngine;
using System.Collections;

public class LanderSpawner : MonoBehaviour 
{
	public GameObject Prefab;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.L))
		{
			var obj = (GameObject)Instantiate(Prefab);
			obj.transform.position = transform.position + transform.forward * 5;
			obj.rigidbody.AddForce(transform.forward * 10000);

		}
	}
}
