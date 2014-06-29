using UnityEngine;
using System.Collections;

public class DestroyOnCollision : MonoBehaviour
{
    private bool _hasExploded;
    public GameObject ExplosionPrefab;

    public void OnCollisionEnter(Collision col)
    {
        if (!_hasExploded)
        {
            Explode(col.contacts[0].point);
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!_hasExploded)
        {
            Explode(transform.position - (rigidbody.velocity * Time.fixedDeltaTime));
            Destroy(gameObject);
        }
    }

    private void Explode(Vector3 point)
    {
        if (ExplosionPrefab != null)
        {
            var exp = (GameObject)Instantiate(ExplosionPrefab);

            exp.transform.position = point;
            _hasExploded = true;
        }
    }
}
