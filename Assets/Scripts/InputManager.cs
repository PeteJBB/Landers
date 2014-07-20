using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using System.Collections;

public class InputManager
{
    public static float GetAxis(InputMapping map)
    {
        if (map.PositiveKey != null && Input.GetKey(map.PositiveKey.Value))
        {
            if (map.LastUpdateTime < Time.fixedTime)
            {
                map.AxisValue = Mathf.Clamp(map.AxisValue + map.KeySensitivity, -1, 1);
            }
            return map.AxisValue;
        }

        if (map.NegativeKey != null && Input.GetKey(map.NegativeKey.Value))
        {
            if (map.LastUpdateTime < Time.fixedTime)
            {
                map.AxisValue = Mathf.Clamp(map.AxisValue - map.KeySensitivity, -1, 1);
            }
            return map.AxisValue;
        }

        if (map.LastUpdateTime < Time.fixedTime)
        {
            var val = map.AxisValue > 0
                ? -map.KeySensitivity
                : map.KeySensitivity;

            map.AxisValue = Mathf.Clamp(map.AxisValue + val, -1, 1);
        }

        if (map.JoystickAxis != null)
        {
            var val = Input.GetAxis(map.JoystickAxis);
            if (map.InvertJoystick)
                val = -val;

            //if (val > map.KeySensitivity || val < -map.KeySensitivity)
            //    map.AxisValue = val;

            return val;
        }

        

        return map.AxisValue;
    }

    public static bool GetButtonDown(InputMapping map)
    {
        if (map.JoystickButton != null && Input.GetKeyDown("joystick 1 button " + map.JoystickButton))
            return true;

        if (map.PositiveKey != null && Input.GetKeyDown(map.PositiveKey.Value))
            return true;

        //if (map.JoystickAxis != null)
        //{
        //    var val = Input.GetAxis(map.JoystickAxis);
        //    if (map.InvertJoystick && val < 0)
        //        return true;
        //    if (!map.InvertJoystick && val > 0)
        //        return true;
        //}

        return false;
    }

    public static bool GetButton(InputMapping map)
    {
        if (map.JoystickButton != null && Input.GetKey("joystick 1 button " + map.JoystickButton))
            return true;

        if (map.PositiveKey != null && Input.GetKey(map.PositiveKey.Value))
            return true;

        if (map.JoystickAxis != null)
        {
            var val = Input.GetAxis(map.JoystickAxis);
            if (map.InvertJoystick && val < 0)
                return true;
            if (!map.InvertJoystick && val > 0)
                return true;
        }

        return false;
    }
}

public class InputMapping
{
    public int? JoystickButton;
    public string JoystickAxis;
    public bool InvertJoystick;
    public KeyCode? PositiveKey;
    public KeyCode? NegativeKey;

    private float _axisValue;
    public float AxisValue 
    {
        get { return _axisValue; }
        set 
        { 
            _axisValue = value;
            LastUpdateTime = Time.fixedTime;
        }
    }

    public float KeySensitivity = 0.2f;
    public float LastUpdateTime;

    public static InputMapping Pitch = new InputMapping()
    {
        JoystickAxis = XboxAxis.RightStickY,
        PositiveKey = KeyCode.UpArrow,
        NegativeKey = KeyCode.DownArrow,
        InvertJoystick = true,
    };

    public static InputMapping Yaw = new InputMapping()
    {
        JoystickAxis = XboxAxis.LeftStickX,
        PositiveKey = KeyCode.D,
        NegativeKey = KeyCode.A,
    };

    public static InputMapping Roll = new InputMapping()
    {
        JoystickAxis = XboxAxis.RightStickX,
        PositiveKey = KeyCode.LeftArrow,
        NegativeKey = KeyCode.RightArrow,
        InvertJoystick = true,
    };

    public static InputMapping Throttle = new InputMapping()
    {
        JoystickAxis = XboxAxis.LeftStickY,
        PositiveKey = KeyCode.W,
        NegativeKey = KeyCode.S,
        InvertJoystick = true,
    };

    public static InputMapping Fire = new InputMapping()
    {
        JoystickAxis = XboxAxis.Triggers,
        InvertJoystick = true,
        PositiveKey = KeyCode.Space
    };

    public static InputMapping Transform = new InputMapping()
    {
        JoystickButton = XboxButtons.X,
        PositiveKey = KeyCode.Return
    };

    public static InputMapping Map = new InputMapping()
    {
        JoystickButton = XboxButtons.Back,
        PositiveKey = KeyCode.M
    };
}

public class XboxButtons
{
    public const int A = 0;
    public const int B = 1;
    public const int X = 2; 
    public const int Y = 3;
    public const int LeftBumper = 4;
    public const int RightBumper = 5;
    public const int Back = 6;
    public const int Start = 7;
    public const int LeftStick = 8;
    public const int RightStick = 9;
}

public class XboxAxis
{
    public const string LeftStickX = "X";
    public const string LeftStickY = "Y";
    public const string Triggers = "3";
    public const string RightStickX = "4";
    public const string RightStickY = "5";
    public const string DPadX = "6";
    public const string DPadY = "7";
}