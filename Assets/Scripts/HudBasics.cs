using UnityEngine;
using System.Collections;

public class HudBasics : MonoBehaviour
{
    public Texture GunSightTexture;
    public Texture HorizonTexture;
    public Texture CompassTexture;
    public Texture FlightPathTexture;

    Vector2 _horizonSize = new Vector2(400, 256);
    Rect _hudBoundary;

    public Vector3 FlightPathIndicatorPoint;

    void Start()
    {
        
    }

    void OnGUI()
    {
        _hudBoundary = new Rect(Screen.width / 5, (Screen.height / 5), (Screen.width / 5) * 3, (Screen.height / 5) * 3);

        if (GameBrain.CurrentView == GameView.Internal && !GameBrain.HideHud)
        {
            // center point
            GUI.DrawTexture(Utility.GetCenteredRectangle(Utility.ScreenCenter(), GunSightTexture.width, GunSightTexture.height), GunSightTexture);

            DrawHorizon();
            DrawCompass();
            DrawFlightPathIndicator();

            DrawThrottleIndicator();
            DrawSpeedIndicator();
            DrawAltimeter();

            DrawWaveInfo();
            DrawAmmo();
            DrawWorldInfo();
            
            if (GetComponent<Damageable>().Health <= 0)
            {
                // you're dead messsage
                var rext = Utility.GetCenteredRectangle(new Vector2(Screen.width / 2f, Screen.height / 2f), 320, 64);
                GUI.TextArea(rext, "YOU'RE DEAD, DUDE", GuiStyles.BigMessageGuiStyle);
            }
        }

        //DrawDebugInfo();
    }

    void DrawThrottleIndicator()
    {
        var fullHeight = 200;
        var rect = new Rect(_hudBoundary.xMin, Screen.height / 2 - fullHeight / 2, 1, fullHeight);
        GUI.DrawTexture(rect, Utility.CreatePlainColorTexture(Color.white));

        var throttle = GetComponent<PlayerFlightControlsJet>().ThrottleControl;
        var setHeight = ((fullHeight - 1) * throttle) + 1;
        var rSet = new Rect(rect.xMin - 2, rect.yMax - setHeight, 5, setHeight);
        GUI.DrawTexture(rSet, Utility.CreatePlainColorTexture(Color.white));

        var labelRect = new Rect(_hudBoundary.xMin - 9, rect.yMax, 0, 0);
        Utility.DrawRotatedTextField(labelRect, "THROTTLE", -90, GuiStyles.Hud_Label_Small);
    }

    void DrawSpeedIndicator()
    {
        var w = 60;
        var h = 28;
        var textRect = new Rect(_hudBoundary.xMin - w - 10, (Screen.height / 2) - (h / 2), w, h);
        
        var speed = Mathf.RoundToInt(rigidbody.velocity.magnitude);
        GUI.TextField(textRect, speed.ToString(), GuiStyles.Hud_Speed);
        Utility.DrawRectOutline(textRect, Color.white, 2);

        var labelRect = new Rect(textRect.xMin, textRect.yMin - 14, textRect.width, 14);
        GUI.TextField(labelRect, "SPEED", GuiStyles.Hud_Label_Small);

        // line to match the one on the other side (right now shows nothing)
        var fullHeight = 200;
        var rect = new Rect(_hudBoundary.xMax, Screen.height / 2 - fullHeight / 2, 1, fullHeight);
        GUI.DrawTexture(rect, Utility.CreatePlainColorTexture(Color.white));
    }

    void DrawAltimeter()
    {
        var w = 60;
        var h = 28;
        var textRect = new Rect(_hudBoundary.xMax + 10, (Screen.height / 2) - (h / 2), w, h);

        var alt = Mathf.RoundToInt(transform.position.y);
        GUI.TextField(textRect, alt.ToString(), GuiStyles.Hud_Speed);
        Utility.DrawRectOutline(textRect, Color.white, 2);

        var labelRect = new Rect(textRect.xMin, textRect.yMin - 14, textRect.width, 14);
        GUI.TextField(labelRect, "ALT", GuiStyles.Hud_Label_Small);
    }

    void DrawWaveInfo()
    {
        var wave = FindObjectOfType<GameBrain>().CurrentWave;
        var waveRect = new Rect(_hudBoundary.xMin, _hudBoundary.yMin, 0, 0);
        GUI.TextField(waveRect, "WAVE " + wave, GuiStyles.Hud_Label);

        var enemies = FindObjectOfType<GameBrain>().CurrentEnemies;
        var enyRect = new Rect(_hudBoundary.xMin, _hudBoundary.yMin + 20, 0, 0);
        GUI.TextField(enyRect, "ENEMY " + enemies, GuiStyles.Hud_Label);
    }

    void DrawWorldInfo()
    {
        var time = Mathf.FloorToInt(Time.timeSinceLevelLoad);
        var minutes = time / 60;
        var seconds = time % 60;

        var timeFormatted = minutes.ToString("0") + ":" + seconds.ToString("00");
        var timeRect = new Rect(_hudBoundary.xMax, _hudBoundary.yMin, 0, 0);
        GUI.TextField(timeRect, "TIME " + timeFormatted, GuiStyles.Hud_Label_Right);

        var buildings = GameObject.FindGameObjectsWithTag("FriendlyStructure").Length;
        var buildingsRect = new Rect(_hudBoundary.xMax, _hudBoundary.yMin + 20, 0, 0);
        GUI.TextField(buildingsRect, "BUILDINGS " + buildings, GuiStyles.Hud_Label_Right);
    }

    void DrawAmmo()
    {
        var missiles = GetComponent<MissileLauncher>().Ammo;
        var mslRect = new Rect(_hudBoundary.xMin, _hudBoundary.yMax - 40, 0, 0);
        GUI.TextField(mslRect, "MSL " + missiles, GuiStyles.Hud_Label);

        var gun = GetComponent<MachineGun>().Ammo;
        var gunRect = new Rect(_hudBoundary.xMin, _hudBoundary.yMax - 20, 0, 0);
        GUI.TextField(gunRect, "GUN " + gun, GuiStyles.Hud_Label);
    }

    void DrawDebugInfo()
    {
        GUI.TextArea(new Rect(20, 20, 100, 20), "Alt: " + transform.position.y, GuiStyles.BasicGuiStyle);

        var relativeVel = rigidbody.transform.worldToLocalMatrix * rigidbody.velocity;
        var speed = relativeVel.z;

        GUI.TextArea(new Rect(20, 80, 100, 20), "Airspeed: " + Mathf.Round(speed), GuiStyles.BasicGuiStyle);

        var damage = GetComponent<Damageable>();
        var health = damage.Health / damage.MaxHealth * 100;
        GUI.TextArea(new Rect(120, 50, 100, 20), "Health: " + health.ToString("0"), GuiStyles.BasicGuiStyle);
        GUI.TextArea(new Rect(120, 80, 100, 20), "MG Ammo: " + GetComponent<MachineGun>().Ammo, GuiStyles.BasicGuiStyle);
        GUI.TextArea(new Rect(120, 110, 100, 20), "Missiles: " + GetComponent<MissileLauncher>().Ammo, GuiStyles.BasicGuiStyle);
    }

    void DrawHorizon()
    {
        var center = Utility.ScreenCenter();
        var destRect = Utility.GetCenteredRectangle(center, _horizonSize.x, _horizonSize.y);

        var degrees = _horizonSize.y / Screen.height * Camera.main.fieldOfView;
        var scale = degrees / 180;
        var pitch = transform.localEulerAngles.x;
        var roll = transform.localEulerAngles.z; ;

        var offset = ((-pitch % 180) / 180);
        var pan = 0.5f + offset - (scale / 2);
        var srcRect = new Rect(0, pan, 1, scale);
        Utility.DrawRotatedGuiTexture(destRect, roll, HorizonTexture, srcRect, null);
    }

    void DrawCompass()
    {
        // compass
        var heading = transform.eulerAngles.y;
        var fov = Camera.main.fieldOfView * Camera.main.aspect; // horizontal FOV (camera fov is vertical)
        var pipDegrees = 6;
        var pipCount = 360 / pipDegrees;
        
        var scale = Screen.width / fov;
        var center = Screen.width / 2;
        
        for (var i = 0; i < pipCount; i++)
        {
            var bearing = (i * pipDegrees);
            var diff = bearing - heading;
            if (diff > 180)
                diff -= 360;
            else if (diff < -180)
                diff += 360;

            var x = center + (diff * scale);
            if (x > _hudBoundary.xMin && x < _hudBoundary.xMax)
            {
                if (bearing % 30 == 0)
                {
                    // large marker with label
                    var rect = new Rect(x, 30, 2, 20);
                    GUI.DrawTexture(rect, Utility.CreatePlainColorTexture(Color.white));

                    var textRect = new Rect(x, 10, 1, 15);
                    GUI.TextField(textRect, bearing.ToString(), GuiStyles.BasicGuiStyleCentered);
                }
                else
                {
                    // small marker
                    var rect = new Rect(x, 30, 1, 15);
                    GUI.DrawTexture(rect, Utility.CreatePlainColorTexture(Color.white));
                }
            }
        }
    }

    void DrawFlightPathIndicator()
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
        FlightPathIndicatorPoint = velPoint;
    }
}
