using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;

public class MissileLauncher : MonoBehaviour
{
    public bool IsGuiActive;
    public Texture LockTexture;
    
    public GameObject MissilePrefab;
    public int Ammo = 200;
    public int MaxAmmo = 200;
    public float Innaccuracy = 0;

    public float ScanDelay = 0.2f;
    public float MaxDetectionAngle = 45;
    public GameObject LockTarget;

    public float FireDelay = 0.06f;
    private float _lastFireTime = 0;

    public Vector3[] Offsets = new Vector3[1];
    private int _offsetIndex = 0;

    private float _lastScan;
    private int _visibilityTestLayerMask;

    void Start()
    {
        _visibilityTestLayerMask = LayerMask.GetMask(new[] { "Terrain", "Buildings" });
    }

    public void Fire()
    {
        if (Time.fixedTime - _lastFireTime > FireDelay)
        {
            if (Ammo == -1 || Ammo > 0)
            {
                var offset = Offsets[_offsetIndex];
                var pos = transform.TransformPoint(offset);

                var inaccuracyAdjustment = Quaternion.Euler(
                    Random.Range(-Innaccuracy, Innaccuracy),
                    Random.Range(-Innaccuracy, Innaccuracy), 
                    Random.Range(-Innaccuracy, Innaccuracy));
                var dir = inaccuracyAdjustment * transform.forward;
                var rotation = Quaternion.LookRotation(dir);

                var b = (GameObject)Instantiate(MissilePrefab, pos, rotation);
                b.rigidbody.velocity = rigidbody != null ? rigidbody.velocity : Vector3.zero;
                b.rigidbody.AddForce(b.transform.forward * 400);
                b.GetComponent<Projectile>().Originator = gameObject;

                b.SetTeam(gameObject.GetTeam());

                if (Ammo != -1)
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

    void OnGUI()
    {
        if (IsGuiActive && LockTarget != null)
        {
            var point = Camera.main.WorldToScreenPoint(LockTarget.transform.position);
            if (point.z > 0)
            {
                var rect = Utility.GetCenteredRectangle(new Vector2(point.x, Screen.height - point.y), 64, 64);
                Utility.DrawRotatedGuiTexture(rect, (Time.fixedTime % 3) / 3 * 360, LockTexture);
            }
            
        }
    }

    private GameObject DetectTarget()
    {
        var detections = new List<Detection>();
        foreach (var obj in GameObject.FindGameObjectsWithTag("EnemyPlane"))
        {
            var angle = Vector3.Angle(transform.forward, obj.transform.position - transform.position);
            if (angle < MaxDetectionAngle)
            {
                detections.Add(new Detection() { angle = angle, gameObject = obj });
            }
        }
        detections = detections.OrderBy(x => x.angle).ToList();

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
        public float angle;
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
    }
}
