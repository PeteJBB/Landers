using UnityEngine;
using System.Collections;

public class PlayerCameraLook : MonoBehaviour 
{
    HudBasics hud;

	// Use this for initialization
	void Start () 
    {
        hud = transform.parent.GetComponent<HudBasics>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        var forward = InputManager.GetAxis(InputMapping.LookForward) > 0;
        var back = InputManager.GetAxis(InputMapping.LookBehind) > 0;
        var left = InputManager.GetAxis(InputMapping.LookLeft) > 0;
        var right = InputManager.GetAxis(InputMapping.LookRight) > 0;

        var angle = 0;
        if (forward)
        {
            angle = left ? -45
                : right ? 45
                : 0;
        }
        else if (back)
        {
            angle = left ? -135
                : right ? 135
                : 180;
        }
        else
        {
            angle = left ? -90
                : right ? 90
                : 0;
        }

        GameBrain.HideHud = angle != 0;
        transform.localRotation = Quaternion.Euler(0, angle, 0);
	}
}
