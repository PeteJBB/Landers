using UnityEngine;
using System.Collections;

public class HudBasics : MonoBehaviour 
{
	public Texture GunSightTexture;
	public Texture HorizonTexture;
	public Texture CompassTexture;
	public Texture FlightPathTexture;

	Vector2 _horizonSize = new Vector2(400, 256);

	void OnGUI()
	{
	    if (GameBrain.CurrentView == GameView.Internal)
	    {
	        // center point
	        GUI.DrawTexture(Utility.GetCenteredRectangle(Utility.ScreenCenter(), GunSightTexture.width, GunSightTexture.height), GunSightTexture);

	        RenderHorizon();
	        RenderCompass();
	        RenderFlightPathIndicator();

            if (GetComponent<Damageable>().Health <= 0)
            {
                // you're dead messsage
                var rext = Utility.GetCenteredRectangle(new Vector2(Screen.width / 2f, Screen.height / 2f), 320, 64);
                GUI.TextArea(rext, "YOU'RE DEAD, DUDE", Utility.BigMessageGuiStyle);
            }
	    }

        DrawDebugInfo();
	}

    void DrawDebugInfo()
    {
        GUI.TextArea(new Rect(20, 20, 100, 20), "Alt: " + transform.position.y, Utility.BasicGuiStyle);
        
        var relativeVel = rigidbody.transform.worldToLocalMatrix * rigidbody.velocity;
        var speed = relativeVel.z;

        GUI.TextArea(new Rect(20, 80, 100, 20), "Airspeed: " + Mathf.Round(speed), Utility.BasicGuiStyle);

        var damage = GetComponent<Damageable>();
        var health = damage.Health / damage.MaxHealth * 100;
        GUI.TextArea(new Rect(120, 50, 100, 20), "Health: " + health.ToString("0"), Utility.BasicGuiStyle);
        GUI.TextArea(new Rect(120, 80, 100, 20), "MG Ammo: " + GetComponent<MachineGun>().Ammo, Utility.BasicGuiStyle);
        GUI.TextArea(new Rect(120, 110, 100, 20), "Missiles: " + GetComponent<MissileLauncher>().Ammo, Utility.BasicGuiStyle);
    }

	void RenderHorizon()
	{
		var center = Utility.ScreenCenter();
		var destRect = Utility.GetCenteredRectangle(center, _horizonSize.x, _horizonSize.y);
		
		var degrees = _horizonSize.y / Screen.height * Camera.main.fieldOfView;
		var scale = degrees / 180;
		var pitch = transform.localEulerAngles.x;
		var roll = transform.localEulerAngles.z;;
		
		var offset = ((-pitch % 180) / 180);
		var pan = 0.5f + offset - (scale/2);
		var srcRect = new Rect(0, pan, 1, scale);
		Utility.DrawRotatedGuiTexture(destRect, roll, HorizonTexture, srcRect, null);
	}

	void RenderCompass()
	{
		// compass
		var center = new Vector2(Screen.width / 2, 25);
		var widthScale = 0.75f;
		var destRect = Utility.GetCenteredRectangle(center, Screen.width * widthScale, 30);
		
		var degrees = Camera.main.fieldOfView * Camera.main.aspect * 0.75f;
		var scale = degrees / 360;
		
		var heading = transform.localEulerAngles.y;
		var offset = (heading % 360) / 360;
		
		var pan = 0.5f + offset - (scale / 2);
		var srcRect = new Rect(pan, 0, scale, 1);
		Utility.DrawRotatedGuiTexture(destRect, 0, CompassTexture, srcRect, null);
	}

    void RenderFlightPathIndicator()
	{
        // flight path indicator
        var velPoint = rigidbody.velocity.magnitude > 1f
            ? Camera.main.WorldToScreenPoint(Camera.main.transform.position + (rigidbody.velocity.normalized * 1000))
            : new Vector3(Screen.width / 2f, Screen.height / 2f, 1);

        if (velPoint.z > 0)
        {
            velPoint.y = Screen.height - velPoint.y;
            var velRect = Utility.GetCenteredRectangle(velPoint, 32, 32);
            GUI.DrawTexture(velRect, FlightPathTexture);
        }
	}
}
