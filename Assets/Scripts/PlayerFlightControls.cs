using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

public class PlayerFlightControls : MonoBehaviour
{
	const float _pitchStrength = 24;
	const float _yawStrength = 30;
	const float _rollStrength = 12;

	const float _enginePower = 400;
	
	private float _throttle; // what the pilot has set
	private Vector3 dragVector; // the calculated drag this frame
	private float deltaMultiplier;
	private float _lastHitTime;

    private bool _isJetMode = false;
	
	
	// Use this for initialization
	void Start()
	{
		
	}
	
	// Update is called once per frame
	void Update()
	{
		deltaMultiplier = Time.deltaTime / 0.02f;
		UpdateFlightControls();
		
		// reset
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.LoadLevel(Application.loadedLevelName);
		}
	}
	
	void UpdateFlightControls()
	{
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _isJetMode = !_isJetMode;
        }


	    if (Input.GetKey(KeyCode.W))
	    {
	        _throttle = 1;
	    }
	    else if (Input.GetKey(KeyCode.S))
	    {
	        _throttle = 0;
	    }
	    else
	    {
	        // hover
	        var thrustAngle = Mathf.Deg2Rad * Vector3.Angle(Physics.gravity, -transform.up);
	        var energy = (rigidbody.mass * Physics.gravity.magnitude) -
	                        (rigidbody.velocity.y * Mathf.Abs(rigidbody.velocity.y));
	        var thrustNeeded = energy / Mathf.Cos(thrustAngle);
	        _throttle = Mathf.Clamp(thrustNeeded / _enginePower, 0, 1);
	    }

        this.rigidbody.AddRelativeForce(new Vector3(0, _enginePower * _throttle * deltaMultiplier, 0));

	    if(_isJetMode)
	    {
	        var thrust = 30000 * Time.fixedDeltaTime;
            this.rigidbody.AddRelativeForce(new Vector3(0,0, thrust));
	    }

	    
		// pitch
		if (Input.GetKey(KeyCode.UpArrow))
		{
			this.rigidbody.AddRelativeTorque(new Vector3(_pitchStrength * deltaMultiplier, 0, 0));
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			this.rigidbody.AddRelativeTorque(new Vector3(-_pitchStrength * deltaMultiplier, 0, 0));
		}
		
		// yaw
		if (Input.GetKey(KeyCode.D))
		{
			this.rigidbody.AddRelativeTorque(new Vector3(0, _yawStrength * deltaMultiplier, -_rollStrength * 0.5f  * Time.deltaTime));
		}
		if (Input.GetKey(KeyCode.A))
		{
			this.rigidbody.AddRelativeTorque(new Vector3(0, -_yawStrength * deltaMultiplier, _rollStrength * 0.5f * Time.deltaTime));
		}
		
		// roll
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			this.rigidbody.AddRelativeTorque(new Vector3(0, 0, _rollStrength * deltaMultiplier));
		}
		if (Input.GetKey(KeyCode.RightArrow))
		{
			this.rigidbody.AddRelativeTorque(new Vector3(0, 0, -_rollStrength * deltaMultiplier));
		}
		
		// drag by facing area
		var relativeVel = rigidbody.transform.worldToLocalMatrix * rigidbody.velocity;
		dragVector = new Vector3(
			-relativeVel.x * Mathf.Abs(relativeVel.x) * 0.5f,
			-relativeVel.y * Mathf.Abs(relativeVel.y) * 0.5f,
			-relativeVel.z * Mathf.Abs(relativeVel.z) * 0.15f
			);
		rigidbody.AddRelativeForce(dragVector * deltaMultiplier);
		
		// weathervaning
	    var correctionVector = new Vector3(
	        -relativeVel.y * Mathf.Abs(relativeVel.y) * 0.01f,
            relativeVel.x * Mathf.Abs(relativeVel.x) * 0.05f, //0.1f,
            0
            );
        rigidbody.AddRelativeTorque(correctionVector * deltaMultiplier);
	}
	
	void ApplyDamage(float amount)
	{
		print("You've been hit! (" + amount + ")");
		_lastHitTime = Time.fixedTime;
	}
	
	void OnGUI()
	{
		GUI.TextArea(new Rect(20, 20, 100, 20), "Alt: " + transform.position.y, Utility.BasicGuiStyle );
		GUI.TextArea(new Rect(20, 50, 100, 20), "Throttle: " + _throttle * 100 + "%", Utility.BasicGuiStyle);

	    var mode = _isJetMode ? "On" : "Off";
        GUI.TextArea(new Rect(20, 80, 100, 20), "Jets: " + mode, Utility.BasicGuiStyle);
		
	}
}
