using UnityEngine;
using System.Collections;

public class Damageable : MonoBehaviour
{
    public float Health;
    public GameObject ExplosionPrefab;

    public void ApplyDamage(float amount)
    {
        Health -= amount;
        print("Baddie hit - remaining health: " + Health);
        if (Health < 0)
        {
            if (ExplosionPrefab != null)
            {
                var exp = Instantiate(ExplosionPrefab, transform.position, transform.rotation);
            }
            Destroy(gameObject);
        }
    }
}
