using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using System.Collections;

public class InputManager
{
    public static InputMapping[] inputDictionary = GetDefaultMappings();

    private static InputMapping[] GetDefaultMappings()
    {
        var arr = new InputMapping[Enum.GetValues(typeof (InputAxis)).Length];

        arr[(int)InputAxis.Pitch] = new InputMapping()
        {
            JoystickAxis = "5",
            PositiveKey = KeyCode.UpArrow,
            NegativeKey = KeyCode.DownArrow,
            InvertJoystick = true,
        };

        arr[(int) InputAxis.Yaw] = new InputMapping()
        {
            JoystickAxis = "X",
            PositiveKey = KeyCode.D,
            NegativeKey = KeyCode.A,
        };

        arr[(int) InputAxis.Roll] = new InputMapping()
        {
            JoystickAxis = "4",
            PositiveKey = KeyCode.LeftArrow,
            NegativeKey = KeyCode.RightArrow,
            InvertJoystick = true,
        };

        arr[(int) InputAxis.Throttle] = new InputMapping()
        {
            JoystickAxis = "Y",
            PositiveKey = KeyCode.W,
            NegativeKey = KeyCode.S,
            InvertJoystick = true,
        };

        arr[(int) InputAxis.Fire] = new InputMapping()
        {
            JoystickAxis = "3",
            InvertJoystick = true,
            PositiveKey = KeyCode.Space
        };

        arr[(int)InputAxis.Transform] = new InputMapping()
        {
            JoystickButton = 2,
            PositiveKey = KeyCode.Return
        };

        return arr;
    }

    public static float GetAxis(InputAxis axis)
    {
        var map = inputDictionary[(int)axis];
        if (map == null)
            return 0;

        if (map.PositiveKey != null && Input.GetKey(map.PositiveKey.Value))
        {
            return 1;
        }
        if (map.NegativeKey != null && Input.GetKey(map.NegativeKey.Value))
        {
            return -1;
        }

        if (map.JoystickAxis != null)
        {
            float val = Input.GetAxis(map.JoystickAxis);
            return map.InvertJoystick
                ? -val
                : val;
        }

        return 0;
    }

    public static bool GetButtonDown(InputAxis axis)
    {
        var map = inputDictionary[(int)axis];
        if (map == null)
            return false;

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

    public static bool GetButton(InputAxis axis)
    {
        var map = inputDictionary[(int)axis];
        if (map == null)
            return false;

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
}

public enum InputAxis
{
    Pitch = 0,
    Yaw = 1,
    Roll = 2,
    Throttle = 3,
    Fire = 4,
    Transform = 5
}
