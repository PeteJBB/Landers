using UnityEngine;
using System.Collections;

public class FaceTravelDirection : MonoBehaviour
{
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(rigidbody.velocity);
    }
}
