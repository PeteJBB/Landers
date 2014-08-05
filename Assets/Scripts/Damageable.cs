using UnityEngine;
using System.Collections;

public class Damageable : MonoBehaviour
{
    public float Health;
    public float MaxHealth;
    public GameObject ExplosionPrefab;
    public GameObject DamageEffectPrefab;
    public bool DontDestroy;
    
    private void Start()
    {
       
    }

    public void ApplyDamage(float amount, GameObject obj)
    {
        Health -= amount;

        if(obj.name.StartsWith("Missile"))
            print(string.Format("{0} does {1} damage to {2} ({3} health left)", obj, amount, name, Health));

        if (DamageEffectPrefab != null)
        {
            Instantiate(ExplosionPrefab, transform.position, transform.rotation);
        }

        if (Health < 0)
        {
            if (ExplosionPrefab != null)
            {
                var exp = Instantiate(ExplosionPrefab, transform.position, transform.rotation);
            }

            if(!DontDestroy)
                Destroy(gameObject);
        }
        
    }

    void OnGUI()
    {
        if (GameBrain.CurrentView == GameView.Internal && gameObject.GetTeam() == 1)
        {
            var dist = Vector3.Distance(Camera.main.transform.position, transform.position);
            if (dist < 200)
            {
                var point = Camera.main.WorldToScreenPoint(transform.position);
                point.y -= 10;
                if (point.z > 0)
                {
                    var amt = Health / MaxHealth;
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
}
