using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;


/// <summary>
/// Modified from original controls
/// This class is a Jet-only version to see how it plays
/// </summary>
public class PlayerFlightControlsJet : MonoBehaviour
{
    public Texture GunsightTexture;
    public Texture FlightPathTexture;

    public AnimationCurve SurfaceControlBySpeed;
    private const float _surfaceControlTopSpeed = 100f;

    public AnimationCurve LiftBySpeed;
    private const float _liftTopSpeed = 90f;

    private const float _pitchStrength = 40;//24;
    private const float _yawStrength = 17;//30;
    private const float _rollStrength = 12;//12;
    private const float _brakeStrength = 0.25f;
    private const float _wheelBrakesMultiplier = 40f;

    const float _enginePower = 400;
    const float _jetPower = 550;

    private float _pitchControl = 0;
    private float _yawControl = 0;
    private float _rollControl = 0;
    private float _brakesControl = 0;

	private float _throttle;
	private Vector3 dragVector;
	private float deltaMultiplier;
    private float controlCurve;

    private WheelCollider[] _wheels;

    public float PitchControl { get { return _pitchControl; } }
    public float YawControl { get { return _yawControl; } }
    public float RollControl { get { return _rollControl; } }
    public float ThrottleControl { get { return _throttle; } }
	

    // Use this for initialization
	void Start()
	{
	    rigidbody.centerOfMass = new Vector3(0, -0.25f, 0f);
	    _wheels = GetComponentsInChildren<WheelCollider>();
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

	    UpdateFlightControls();
	    UpdatePhysics();
	}

    void UpdateFlightControls()
	{
        // throttle
        var throt = InputManager.GetAxis(InputMapping.Throttle);
        _throttle += throt * Time.deltaTime;
        _throttle = Mathf.Clamp(_throttle, 0, 1);

        // surfaces
        _pitchControl = InputManager.GetAxis(InputMapping.Pitch);
        _yawControl = InputManager.GetAxis(InputMapping.Yaw);
        _rollControl = InputManager.GetAxis(InputMapping.Roll);

        // brakes
        _brakesControl = -InputManager.GetAxis(InputMapping.Throttle);
        _brakesControl = Mathf.Clamp(_brakesControl, 0, 1);

        // weapons
        if (InputManager.GetButton(InputMapping.Fire))
        {
            var mg = GetComponent<MachineGun>();
            mg.Fire();
        }
        if (InputManager.GetButtonDown(InputMapping.FireSecondary))
        {
            var ml = GetComponent<MissileLauncher>();
            ml.Fire();
        }
	}

    private void UpdatePhysics()
    {
        var relativeVel = rigidbody.transform.worldToLocalMatrix * rigidbody.velocity;
        var speed = relativeVel.z;
        
        // pitch / yaw / roll
        controlCurve = SurfaceControlBySpeed.Evaluate(speed / _surfaceControlTopSpeed);
        var pitch = _pitchControl * _pitchStrength * controlCurve;
        rigidbody.AddRelativeTorque(new Vector3(pitch * deltaMultiplier, 0, 0));

        var yaw = _yawControl * _yawStrength * controlCurve;
        rigidbody.AddRelativeTorque(new Vector3(0, yaw * deltaMultiplier, 0));

        var roll = _rollControl * _rollStrength * controlCurve;
        rigidbody.AddRelativeTorque(new Vector3(0, 0, roll * deltaMultiplier));

        // thrust
        var thrust = _throttle * _jetPower * deltaMultiplier;
        rigidbody.AddRelativeForce(new Vector3(0, 0, thrust));

        // air brakes
        var brakeForce = (_brakesControl * -speed * _brakeStrength);

        // wheel brakes
        if (_brakesControl > 0 && _wheels.All(x => x.isGrounded))
        {
            brakeForce *= _wheelBrakesMultiplier;
            brakeForce += brakeForce > 0
                ? 50
                : -50;
        }
        rigidbody.AddRelativeForce(0, 0, brakeForce * deltaMultiplier);

        // wheel steering (taxi)
        Transform _wheel_frontleft = transform.FindChild("wheel_frontleft");
        Transform _wheel_frontright = transform.FindChild("wheel_frontright");
        var steer = _yawControl * 10f;
        _wheel_frontleft.localRotation = Quaternion.Euler(0, steer, 0);
        _wheel_frontright.localRotation = Quaternion.Euler(0, steer, 0);

        // lift
        var liftZero = rigidbody.mass * Physics.gravity.magnitude * 1.05f; // enough lift for takeoff / level flight
        var lift = LiftBySpeed.Evaluate(speed / _liftTopSpeed) * liftZero;
        rigidbody.AddRelativeForce(new Vector3(0, lift * deltaMultiplier, 0));

        // drag by facing area
        dragVector = new Vector3(
            -relativeVel.x * Mathf.Abs(relativeVel.x) * 0.75f,
            -relativeVel.y * Mathf.Abs(relativeVel.y) * 1f,
            -relativeVel.z * Mathf.Abs(relativeVel.z) * 0.025f
        );
        rigidbody.AddRelativeForce(dragVector * deltaMultiplier);

        // extra lift due to Angle Of Attack
        var aoa = Vector3.Angle(transform.forward.IgnoreX(), rigidbody.velocity.IgnoreX());
        if (aoa < 90)
        {
            if (aoa > 45)
                aoa = 45 + (45 - aoa);
            if (relativeVel.y > 0)
                aoa = -aoa;

            var extraLift = Mathf.Clamp(aoa / 45f, -1, 1) * lift;
            
            var aoaVector = new Vector3(
                0,
                extraLift,
                0
                );
            rigidbody.AddRelativeForce(aoaVector * deltaMultiplier);
        }
        // weathervaning
        var correctionVector = new Vector3(
            -relativeVel.y * Mathf.Abs(relativeVel.y) * 0.03f,
            relativeVel.x * Mathf.Abs(relativeVel.x) * 0.05f, //0.1f,
            0
        );
        rigidbody.AddRelativeTorque(correctionVector * deltaMultiplier);
    }
	
	void OnGUI()
	{

        //GUI.TextArea(new Rect(20, 50, 100, 20), "Throttle: " + Mathf.Round(_throttle * 100) + "%", GuiStyles.BasicGuiStyle);

        //// control curve
        //GUI.TextArea(new Rect(20, 140, 100, 20), "Control Curve: " + controlCurve, GuiStyles.BasicGuiStyle);
        //GUI.TextArea(new Rect(20, 170, 100, 20), "Brakes: " + _brakesControl, GuiStyles.BasicGuiStyle);

	}

    private bool IsLandedOnHelipad()
    {
        var pads = FindObjectsOfType<ResupplyArea>();
        foreach (var p in pads)
        {
            if (p.CurrentlyResupplying.Contains(gameObject))
                return true;
        }
        return false;
    }
}
