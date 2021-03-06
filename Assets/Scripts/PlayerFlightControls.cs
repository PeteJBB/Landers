﻿using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

public class PlayerFlightControls : MonoBehaviour
{
    public Texture GunsightTexture;
    public Texture FlightPathTexture;

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
        if (InputManager.GetButtonDown(InputMapping.Transform))
        {
            _isJetMode = !_isJetMode;
            _throttle = 1;
        }

	    UpdateFlightControls();
	    UpdatePhysics();
	}

    void UpdateFlightControls()
	{
        if (!_isJetMode && !IsLandedOnHelipad())
        {
            // calculate hover throttle
            var thrustAngle = Mathf.Deg2Rad * Vector3.Angle(Physics.gravity, -transform.up);
            var energy = (rigidbody.mass * Physics.gravity.magnitude);
            energy -= (rigidbody.velocity.y * Mathf.Abs(rigidbody.velocity.y));

            var thrustNeeded = energy / Mathf.Cos(thrustAngle);
            _throttle = Mathf.Clamp(thrustNeeded / _enginePower, 0, 1);
        }

        var throt = InputManager.GetAxis(InputMapping.Throttle);
        if (_isJetMode)
            _throttle += throt * Time.deltaTime;
        else
            _throttle += throt;
        _throttle = Mathf.Clamp(_throttle, 0, 1);

        _pitchControl = InputManager.GetAxis(InputMapping.Pitch);
        _yawControl = InputManager.GetAxis(InputMapping.Yaw);
        _rollControl = InputManager.GetAxis(InputMapping.Roll);

        // MG
        if (InputManager.GetButton(InputMapping.Fire))
        {
            var mg = GetComponent<MachineGun>();
            mg.Fire();
        }
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
        GUI.TextArea(new Rect(20, 20, 100, 20), "Alt: " + transform.position.y, GuiStyles.BasicGuiStyle);
		GUI.TextArea(new Rect(20, 50, 100, 20), "Throttle: " + Mathf.Round(_throttle * 100) + "%", GuiStyles.BasicGuiStyle);
        GUI.TextArea(new Rect(20, 80, 100, 20), "Airspeed: " + Mathf.Round(rigidbody.velocity.magnitude), GuiStyles.BasicGuiStyle);
        
	    var mode = _isJetMode ? "On" : "Off";
        GUI.TextArea(new Rect(20, 110, 100, 20), "Jets: " + mode, GuiStyles.BasicGuiStyle);

	    var damage = GetComponent<Damageable>();
	    var health = damage.Health / damage.MaxHealth * 100;
        GUI.TextArea(new Rect(120, 50, 100, 20), "Health: " + health.ToString("0"), GuiStyles.BasicGuiStyle);
        GUI.TextArea(new Rect(120, 80, 100, 20), "Ammo: " + GetComponent<MachineGun>().Ammo, GuiStyles.BasicGuiStyle);


        GUI.TextArea(new Rect(120, 110, 100, 20), "Roll Axis: " + InputMapping.Roll.AxisValue, GuiStyles.BasicGuiStyle);

        // gunsight
	    if (GameBrain.CurrentView == GameView.Internal)
	    {
	        var rect = Utility.GetCenteredRectangle(new Vector2(Screen.width / 2f, Screen.height / 2f), 32, 32);
	        GUI.DrawTexture(rect, GunsightTexture);
	    }

	    if (GetComponent<Damageable>().Health <= 0)
	    {
	        // you're dead messsage
	        var rext = Utility.GetCenteredRectangle(new Vector2(Screen.width / 2f, Screen.height / 2f), 320, 64);
            GUI.TextArea(rext, "YOU'RE DEAD, DUDE", GuiStyles.BigMessageGuiStyle);
	    }

        // flight path indicator
	    var velPoint = Camera.main.WorldToScreenPoint(transform.position + (rigidbody.velocity * 1000));
	    if (velPoint.z > 0)
	    {
	        velPoint.y = Screen.height - velPoint.y;
	        var velRect = Utility.GetCenteredRectangle(velPoint, 32, 32);
	        GUI.DrawTexture(velRect, FlightPathTexture);
	    }
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
