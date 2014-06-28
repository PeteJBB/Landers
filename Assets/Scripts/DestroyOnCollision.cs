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
            print(col.contacts.Length);
            Explode(col.contacts[0].point);
            Destroy(gameObject);
        }
    }

    private void Explode(Vector3 point)
    {
        if (ExplosionPrefab != null)
        {
            var exp = (GameObject)Instantiate(ExplosionPrefab);
            
            exp.transform.position = transform.position + (transform.forward * rigidbody.velocity.magnitude * Time.deltaTime / 2);
            _hasExploded = true;
        }
    }
}
