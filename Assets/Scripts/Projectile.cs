using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    public float DirectDamage;
    public GameObject ExplosionPrefab;
    public float ExplosionRadius;
    public float ExplosionDamage;

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        var ray = new Ray(transform.position - (rigidbody.velocity * Time.fixedDeltaTime), rigidbody.velocity.normalized);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, rigidbody.velocity.magnitude * Time.fixedDeltaTime))
        {
            // collision
            var myTeam = gameObject.GetTeam();
            var damageable = hitInfo.collider.transform.root.GetComponent<Damageable>();
            if (damageable != null && (myTeam == 0 || myTeam != damageable.gameObject.GetTeam()))
                damageable.ApplyDamage(DirectDamage);

            Explode(hitInfo.point);
            Destroy(gameObject);
        }
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
                var myTeam = gameObject.GetTeam();
                var damageable = c.transform.root.GetComponent<Damageable>();
                if (damageable != null && (myTeam == 0 || myTeam != damageable.gameObject.GetTeam()))
                {
                    var dist = Vector3.Distance(c.ClosestPointOnBounds(point), point);
                    var damage = Mathf.Lerp(0, ExplosionDamage, 1 - (dist / ExplosionRadius));
                    damageable.ApplyDamage(damage);
                }
            }
        }
    }
}
