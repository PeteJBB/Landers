using UnityEngine;
using System.Collections;

public class DestroyAfterTime : MonoBehaviour
{
    public float LifeTime;

    void Start()
    {
        Destroy(gameObject, LifeTime);
    }
}
