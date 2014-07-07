using UnityEngine;
using System.Collections;

public class Damageable : MonoBehaviour
{
    public float MaxHealth;
    public GameObject ExplosionPrefab;

    /// <summary>
    /// Team 0 means anyone can damage me, otherwise only take damage from other teams
    /// </summary>
    public int Team;

    private float _health;
    private void Start()
    {
        _health = MaxHealth;
    }

    public void ApplyDamage(float amount)
    {
        _health -= amount;
        if (_health < 0)
        {
            if (ExplosionPrefab != null)
            {
                var exp = Instantiate(ExplosionPrefab, transform.position, transform.rotation);
            }
            Destroy(gameObject);
        }
    }

    void OnGUI()
    {
        var dist = Vector3.Distance(Camera.main.transform.position, transform.position);
        if (dist < 200)
        {
            //var combinedBounds = renderer != null ? renderer.bounds : new Bounds(transform.position, Vector3.zero);
            //foreach(var r in GetComponentsInChildren<Renderer>()) 
            //{
            //    if (r != renderer) 
            //        combinedBounds.Encapsulate(r.bounds);
            //}

            //var pos = new Vector3(combinedBounds.center.x, combinedBounds.max.y + 1, combinedBounds.center.z);
            var point = Camera.main.WorldToScreenPoint(transform.position);
            point.y -= 10;
            if (point.z > 0)
            {
                var amt = _health / MaxHealth;
                var rect = Utility.GetCenteredRectangle(new Vector2(point.x, Screen.height - point.y), 30 * amt, 5);

                var c = Color.green;
                if (amt < 0.5f)
                    c = Color.yellow;
                if (amt < 0.25f)
                    c = Color.red;

                GUI.DrawTexture(rect, Utility.CreatePlainColorTexture(c), ScaleMode.StretchToFill);
            }
        }
    }
}
