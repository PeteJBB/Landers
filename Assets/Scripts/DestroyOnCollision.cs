using UnityEngine;
using System.Collections;

public class DestroyOnCollision : MonoBehaviour
{
    public GameObject ExplosionPrefab;
    public float ExplosionDamage;
    public float ExplosionRadius;

    private bool _isDestroyed;
    
    public void OnCollisionEnter(Collision col)
    {
        if (!_isDestroyed)
        {
            var point = GetExplosionPoint(col.collider);
            Explode(point);
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter(Collider col)
    {
        if (!_isDestroyed)
        {
            var point = GetExplosionPoint(col);
            Explode(point);
            Destroy(gameObject);
        }
    }

    private Vector3 GetExplosionPoint(Collider col)
    {
        var point = transform.position;
        if (col.gameObject.tag == "Terrain")
        {
            var t = col.gameObject.GetComponent<Terrain>();
            var h = t.SampleHeight(point) + t.GetPosition().y;
            var i = 0;
            while (h > point.y)
            {
                point -= rigidbody.velocity * Time.deltaTime;
                h = t.SampleHeight(point) + t.GetPosition().y;

                if (++i > 10) break;
            }
        }
        return point;
    }


    private void Explode(Vector3 point)
    {
        if (ExplosionPrefab != null)
        {
            var exp = (GameObject)Instantiate(ExplosionPrefab);
            exp.transform.position = point;
        }

        if (ExplosionRadius > 0 && ExplosionDamage > 0)
        {
            foreach (var c in Physics.OverlapSphere(point, ExplosionRadius))
            {
                var damageable = c.GetComponent<Damageable>();
                if (damageable != null)
                {
                    var dist = Vector3.Distance(c.ClosestPointOnBounds(point), point);
                    var damage = Mathf.Lerp(0, ExplosionDamage, 1 - (dist / ExplosionRadius));
                    damageable.ApplyDamage(damage);
                }
            }
        }

        _isDestroyed = true;
    }
}
