using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    public float DirectDamage;
    public GameObject ExplosionPrefab;
    public float ExplosionRadius;
    public float ExplosionDamage;
    public float EnginePower;
    public Vector3 FreefallDrag;

    public GameObject Originator;

    private Damageable _directDamageObject;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (EnginePower > 0)
        {
            rigidbody.AddRelativeForce(0, 0, EnginePower);
        }

        // drag by axis
        var vel = rigidbody.velocity;
        vel.x *= 1 - (FreefallDrag.x * Time.fixedDeltaTime);
        vel.y *= 1 - (FreefallDrag.y * Time.fixedDeltaTime);
        vel.z *= 1 - (FreefallDrag.z * Time.fixedDeltaTime);
        rigidbody.velocity = vel;

        var ray = new Ray(transform.position - (rigidbody.velocity * Time.fixedDeltaTime), rigidbody.velocity.normalized);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, rigidbody.velocity.magnitude * Time.fixedDeltaTime))
        {
            if (hitInfo.collider.gameObject != Originator
                && hitInfo.collider.transform.root.gameObject != Originator)
            {
                // collision
                var myTeam = gameObject.GetTeam();
                var damageable = hitInfo.collider.transform.root.GetComponent<Damageable>();
                if (damageable != null && (myTeam == 0 || myTeam != damageable.gameObject.GetTeam()))
                {
                    _directDamageObject = damageable;
                    damageable.ApplyDamage(DirectDamage, gameObject);
                }

                Explode(hitInfo.point);
                Destroy(gameObject);
            }
        }
    }

    public void Explode(Vector3 point)
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
                if (damageable != null && damageable != _directDamageObject 
                    && (myTeam == 0 || myTeam != damageable.gameObject.GetTeam()))
                {
                    var dist = Vector3.Distance(c.ClosestPointOnBounds(point), point);
                    var damage = Mathf.Lerp(0, ExplosionDamage, 1 - (dist / ExplosionRadius));
                    damageable.ApplyDamage(damage, gameObject);
                }
            }
        }
    }
}
