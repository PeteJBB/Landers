using UnityEngine;
using System.Collections;

public class BasicFlightPhysics : MonoBehaviour
{
    public Vector3 RelativeDragVector = new Vector3(2, 2, 0.025f);

    void Start()
    {

    }

    void Update()
    {
        var deltaMultiplier = Time.deltaTime / Time.fixedDeltaTime;
        var relativeVel = transform.worldToLocalMatrix * rigidbody.velocity;

        // drag by facing area
        var dragVector = new Vector3(
            -relativeVel.x * Mathf.Abs(relativeVel.x) * 2f,
            -relativeVel.y * Mathf.Abs(relativeVel.y) * 2f,
            -relativeVel.z * Mathf.Abs(relativeVel.z) * 0.025f
            );
        rigidbody.AddRelativeForce(dragVector * deltaMultiplier);
    }
}
