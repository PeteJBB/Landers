using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BombLauncher : MonoBehaviour, IPlayerWeapon
{
    public GameObject BombPrefab;
    public int Ammo = 6;
    public int MaxAmmo = 6;

    public Texture ImpactPointTexture;
    
    public Vector3[] Offsets = new Vector3[1];
    private int _offsetIndex = 0;

    public float FireDelay = 0.06f;
    private float _lastFireTime = 0;

    void Start()
    {

    }

    void Update()
    {
        
    }

    public void Fire()
    {
        if (Time.fixedTime - _lastFireTime > FireDelay)
        {
            if (Ammo == -1 || Ammo > 0)
            {
                var offset = Offsets[_offsetIndex];
                var pos = transform.TransformPoint(offset);

                var b = (GameObject)Instantiate(BombPrefab, pos, transform.rotation);
                b.rigidbody.velocity = rigidbody != null ? rigidbody.velocity : Vector3.zero;
                b.GetComponent<Projectile>().Originator = gameObject;

                b.SetTeam(gameObject.GetTeam());

                if (Ammo > 0)
                    Ammo--;

                _offsetIndex = (_offsetIndex + 1) % Offsets.Length;
            }

            _lastFireTime = Time.fixedTime;
        }
    }

    public void DrawHud()
    {
        if (Event.current.type == EventType.Repaint)
        {
            // caluclate impact point
            var impactPoint = transform.position;
            var vel = rigidbody.velocity;
            float t = 0;
            for (var i = 0; i < 1000; i++)
            {
                vel *= 1 - (0.2f * Time.fixedDeltaTime);
                vel += Physics.gravity * Time.fixedDeltaTime;
                impactPoint += vel * Time.fixedDeltaTime;

                t = Utility.GetTerrainHeight(impactPoint);
                if (impactPoint.y < t)
                    break;
            }
            impactPoint.y = t;

            // draw path on HUD
            var screenPoint = Camera.main.WorldToScreenPoint(impactPoint);
            screenPoint.y = Screen.height - screenPoint.y;
            if (screenPoint.z > 0)
            {
                var rect = Utility.GetCenteredRectangle(screenPoint, 24, 24);
                GUI.DrawTexture(rect, ImpactPointTexture);
                var start = GetComponent<HudBasics>().FlightPathIndicatorPoint; //new Vector2(Screen.width / 2, (Screen.height / 2) + 50)
                Utility.DrawLine(start, screenPoint, 1, Color.white);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var size = new Vector3(0.1f, 0.1f, 0.5f);
        foreach (var o in Offsets)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(o, new Vector3(0.1f, 0.1f, 0.5f));
        }
        Gizmos.matrix = Matrix4x4.identity;
    }
}
