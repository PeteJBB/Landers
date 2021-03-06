﻿using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;

public class MissileLauncher : MonoBehaviour, IPlayerWeapon
{
    public GameObject MissilePrefab;
    public int Ammo = 6;
    public int MaxAmmo = 6;

    public Texture LockTexture;
    public Texture HudTexture;

    public float ScanDelay = 0.2f;
    public float MaxDetectionAngle = 60;
    public GameObject LockTarget;

    public float FireDelay = 0.06f;
    private float _lastFireTime = 0;

    public Vector3[] Offsets = new Vector3[1];
    private int _offsetIndex = 0;

    private float _lastScan;
    private int _visibilityTestLayerMask;

    void Start()
    {
        _visibilityTestLayerMask = LayerMask.GetMask(new[] { "Default", "Terrain", "Buildings" });
    }

    public void Fire()
    {
        if (Time.fixedTime - _lastFireTime > FireDelay)
        {
            if (Ammo == -1 || Ammo > 0)
            {
                var offset = Offsets[_offsetIndex];
                var pos = transform.TransformPoint(offset);

                var b = (GameObject)Instantiate(MissilePrefab, pos, transform.rotation);
                b.GetComponent<Projectile>().Originator = gameObject;
                b.GetComponent<HeatSeeker>().Target = LockTarget;
                b.rigidbody.velocity = rigidbody != null ? rigidbody.velocity : Vector3.zero;
                b.rigidbody.AddForce(b.transform.forward * 400);
                
                b.SetTeam(gameObject.GetTeam());

                if (Ammo > 0)
                    Ammo--;

                _offsetIndex = (_offsetIndex + 1) % Offsets.Length;
            }

            _lastFireTime = Time.fixedTime;
        }
    }

    void Update()
    {
        if (Time.fixedTime - _lastScan > ScanDelay)
        {
            LockTarget = DetectTarget();
            _lastScan = Time.fixedTime;
        }
    }

    private GameObject DetectTarget()
    {
        var detections = new List<Detection>();
        foreach (var heatSource in FindObjectsOfType<HeatSeekerTarget>())
        {
            var dist = Vector3.Distance(transform.position, heatSource.transform.position);
            if (dist < 500 * heatSource.Intensity)
            {
                // check if obj is in front of me
                var angle = Vector3.Angle(transform.forward, heatSource.transform.position - transform.position);
                if (angle < MaxDetectionAngle)
                {
                    // check if i a within heat source's visible cone
                    var orientation = Vector3.Angle(heatSource.transform.forward, transform.position - heatSource.transform.position);
                    if (orientation < heatSource.Angle)
                    {
                        var strength = angle * orientation * dist * heatSource.Intensity;
                        detections.Add(new Detection() { strength = strength, gameObject = heatSource.gameObject });
                    }
                }
            }
        }
        detections = detections.OrderBy(x => x.strength).ToList();

        foreach (var d in detections)
        {
            if (!Physics.Linecast(transform.position, d.gameObject.transform.position, _visibilityTestLayerMask))
            {
                return d.gameObject;
            }
        }

        return null;
    }

    private class Detection
    {
        public float strength;
        public GameObject gameObject;
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

    public void DrawHud()
    {
        GUI.DrawTexture(Utility.GetCenteredRectangle(new Vector2(Screen.width / 2, Screen.height / 2), 350, 350), HudTexture);
        if (LockTarget != null)
        {
            var point = Camera.main.WorldToScreenPoint(LockTarget.transform.position);
            if (point.z > 0)
            {
                var rect = Utility.GetCenteredRectangle(new Vector2(point.x, Screen.height - point.y), 64, 64);
                Utility.DrawRotatedGuiTexture(rect, (Time.fixedTime % 3) / 3 * 360, LockTexture);
            }

        }
    }
}
