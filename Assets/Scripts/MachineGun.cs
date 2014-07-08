using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class MachineGun : MonoBehaviour
{
    public GameObject BulletPrefab;
    public int Ammo = 200;
    public int MaxAmmo = 200;

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

                var b = (GameObject) Instantiate(BulletPrefab, pos, transform.rotation);
                b.rigidbody.velocity = rigidbody != null ? rigidbody.velocity : Vector3.zero;
                b.rigidbody.AddForce(b.transform.forward * 400);

                b.SetTeam(gameObject.GetTeam());

                if (Ammo != -1)
                    Ammo--;

                _offsetIndex = (_offsetIndex + 1) % Offsets.Length;
            }

            _lastFireTime = Time.fixedTime;
        }
    }

    private void OnGUI()
    {
        GUI.TextArea(new Rect(120, 80, 100, 20), "Ammo: " + Ammo, Utility.BasicGuiStyle);
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
