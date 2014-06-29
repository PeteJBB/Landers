using UnityEngine;
using System.Collections;

public class LandingSite : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 1, 0.1f);
        Gizmos.DrawCube(transform.position, new Vector3(5, 0.4f, 5));
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(5, 0.4f, 5));
    }
}
