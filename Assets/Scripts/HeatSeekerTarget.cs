using UnityEngine;
using System.Collections;

public class HeatSeekerTarget : MonoBehaviour
{
    public float Intensity = 1;
    public float Angle = 45;

    void Start()
    {

    }

    void Update()
    {

    }

    void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = new Color(1, 0.5f, 0);
        Gizmos.DrawFrustum(new Vector3(0, 0, 0), Angle, Intensity, 0, 1);
    }
}
