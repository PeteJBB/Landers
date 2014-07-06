using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class InputManager
{
    public static Dictionary<InputAxis, InputMapping> inputDictionary = GetDefaultMappings();

    private static Dictionary<InputAxis, InputMapping> GetDefaultMappings()
    {
        var dic = new Dictionary<InputAxis, InputMapping>
        {
            {
                InputAxis.Pitch, new InputMapping()
                {
                    JoystickAxis = "joystick axis 5",
                    PositiveKey = KeyCode.DownArrow,
                    NegativeKey = KeyCode.UpArrow,
                    JoystickButton = null
                }
            },
            {
                InputAxis.Yaw, new InputMapping()
                {
                    JoystickAxis = "x",
                    PositiveKey = KeyCode.D,
                    NegativeKey = KeyCode.A,
                    JoystickButton = null
                }
            },
            {
                InputAxis.Roll, new InputMapping()
                {
                    JoystickAxis = "joystick axis 4",
                    PositiveKey = KeyCode.RightArrow,
                    NegativeKey = KeyCode.LeftArrow,
                    JoystickButton = null
                }
            },
            {
                InputAxis.Throttle, new InputMapping()
                {
                    JoystickAxis = "y",
                    PositiveKey = KeyCode.S,
                    NegativeKey = KeyCode.W,
                    JoystickButton = null
                }
            }
        };

        return dic;
    }

    public static float GetAxis(InputAxis axis)
    {
        var map = inputDictionary[axis];
        return Input.GetAxis(map.JoystickAxis);
    }

    public static bool GetButtonDown(InputAxis axis)
    {
        var map = inputDictionary[axis];
        return Input.GetButtonDown("joystick button " + map.JoystickButton);
    }
}

public class InputMapping
{
    public int? JoystickButton { get; set; }
    public string JoystickAxis { get; set; }
    public KeyCode? PositiveKey { get; set; }
    public KeyCode? NegativeKey { get; set; }
}

public enum InputAxis
{
    Pitch,
    Yaw,
    Roll,
    Throttle,
    Fire,
    Transform
}
