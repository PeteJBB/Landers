using UnityEngine;
using System.Collections;

public class LandingSite : MonoBehaviour
{
    public Vector3 ApproachVector;
    public Vector3 ExitVector;

    public bool IsEngaged;

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 1, 0.1f);
        Gizmos.DrawCube(transform.position, new Vector3(5, 0.4f, 5));
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(5, 0.4f, 5));

        //Gizmos.color = Color.green;
        //Gizmos.DrawLine(transform.position, transform.position + (ApproachVector.normalized * 100));
        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(transform.position, transform.position + (ExitVector.normalized * 100));
    }
}
