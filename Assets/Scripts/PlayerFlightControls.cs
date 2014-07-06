using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

public class PlayerFlightControls : MonoBehaviour
{
    public Texture GunsightTexture;

    private const float _pitchStrength = 40;//24;
    private const float _yawStrength = 45;//30;
    private const float _rollStrength = 12;//12;

    const float _enginePower = 400;
    const float _jetPower = 500;

    private float _pitchControl = 0;
    private float _yawControl = 0;
    private float _rollControl = 0;

	private float _throttle;
	private Vector3 dragVector;
	private float deltaMultiplier;

    private bool _isJetMode = false;
	
	// Use this for initialization
	void Start()
	{
	    rigidbody.centerOfMass = new Vector3(0, -1, 0);
	}
	
	// Update is called once per frame
	void Update()
	{
		deltaMultiplier = Time.deltaTime / Time.fixedDeltaTime;
		
		// reset
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.LoadLevel(Application.loadedLevelName);
		}

        // jet mode
        if (InputManager.GetButtonDown(InputAxis.Transform))
        {
            _isJetMode = !_isJetMode;
            _throttle = 1;
        }

	    UpdateFlightControls();
	    UpdatePhysics();
	}

    void UpdateFlightControls()
	{
        if (!_isJetMode)
        {
            // calculate hover throttle
            var thrustAngle = Mathf.Deg2Rad * Vector3.Angle(Physics.gravity, -transform.up);
            var energy = (rigidbody.mass * Physics.gravity.magnitude);
            energy -= (rigidbody.velocity.y * Mathf.Abs(rigidbody.velocity.y));

            var thrustNeeded = energy / Mathf.Cos(thrustAngle);
            _throttle = Mathf.Clamp(thrustNeeded / _enginePower, 0, 1);
        }

        _throttle += InputManager.GetAxis(InputAxis.Throttle);
        _throttle = Mathf.Clamp(_throttle, 0, 1);

        _pitchControl = InputManager.GetAxis(InputAxis.Pitch);
        _yawControl = InputManager.GetAxis(InputAxis.Yaw);
        _rollControl = InputManager.GetAxis(InputAxis.Roll);

        // aim MG
        var mg = GetComponent<MachineGun>();
        mg.AimRotation = Quaternion.LookRotation(transform.forward, Vector3.up);
	}

    private void UpdatePhysics()
    {
        // pitch / yaw / roll
        rigidbody.AddRelativeTorque(new Vector3(_pitchStrength * _pitchControl * deltaMultiplier, 0, 0));
        rigidbody.AddRelativeTorque(new Vector3(0, _yawStrength * _yawControl * deltaMultiplier, 0));
        rigidbody.AddRelativeTorque(new Vector3(0, 0, _rollStrength * _rollControl * deltaMultiplier));

        // thrust / lift
        var relativeVel = rigidbody.transform.worldToLocalMatrix * rigidbody.velocity;
        if (_isJetMode)
        {
            var thrust = _throttle * _jetPower * deltaMultiplier;
            rigidbody.AddRelativeForce(new Vector3(0, 0, thrust));

            var liftZero = rigidbody.mass * Physics.gravity.magnitude;
            var lift = Mathf.Clamp(relativeVel.z * 3, 0, liftZero * 1.5f);
            rigidbody.AddRelativeForce(new Vector3(0, lift * deltaMultiplier, 0));

            // drag by facing area
            dragVector = new Vector3(
                -relativeVel.x * Mathf.Abs(relativeVel.x) * 0.5f,
                -relativeVel.y * Mathf.Abs(relativeVel.y) * 1f,
                -relativeVel.z * Mathf.Abs(relativeVel.z) * 0.05f
                );
            rigidbody.AddRelativeForce(dragVector * deltaMultiplier);

            // weathervaning
            var correctionVector = new Vector3(
                -relativeVel.y * Mathf.Abs(relativeVel.y) * 0.03f,
                relativeVel.x * Mathf.Abs(relativeVel.x) * 0.05f, //0.1f,
                0
            );
            rigidbody.AddRelativeTorque(correctionVector * deltaMultiplier);
        }
        else
        {
            rigidbody.AddRelativeForce(new Vector3(0, _enginePower * _throttle * deltaMultiplier, 0));

            // drag by facing area
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

        
    }
	
	void OnGUI()
	{
		GUI.TextArea(new Rect(20, 20, 100, 20), "Alt: " + transform.position.y, Utility.BasicGuiStyle );
		GUI.TextArea(new Rect(20, 50, 100, 20), "Throttle: " + Mathf.Round(_throttle * 100) + "%", Utility.BasicGuiStyle);
        GUI.TextArea(new Rect(20, 80, 100, 20), "Airspeed: " + Mathf.Round(rigidbody.velocity.magnitude), Utility.BasicGuiStyle);

	    var mode = _isJetMode ? "On" : "Off";
        GUI.TextArea(new Rect(20, 110, 100, 20), "Jets: " + mode, Utility.BasicGuiStyle);

        // gunsight
	    var rect = Utility.GetCenteredRectangle(new Vector2(Screen.width / 2f, Screen.height / 2f), 32, 32);
        GUI.DrawTexture(rect, GunsightTexture);


        GUI.TextArea(new Rect(20, 140, 100, 20), "Pitch: " + InputManager.GetAxis(InputAxis.Pitch), Utility.BasicGuiStyle);

	}
}
