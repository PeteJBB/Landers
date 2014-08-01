using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class MissileLauncher : MonoBehaviour
{
    public GameObject MissilePrefab;
    public int Ammo = 200;
    public int MaxAmmo = 200;
    public float Innaccuracy = 0;

    public float FireDelay = 0.06f;
    private float _lastFireTime = 0;

    public Vector3[] Offsets = new Vector3[1];
    private int _offsetIndex = 0;
    
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
