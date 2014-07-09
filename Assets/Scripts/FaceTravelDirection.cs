using UnityEngine;
using System.Collections;

public class FaceTravelDirection : MonoBehaviour
{
    void Update()
    {
        if(rigidbody.velocity.magnitude > 0)
            transform.rotation = Quaternion.LookRotation(rigidbody.velocity);
    }
}
