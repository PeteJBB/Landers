using UnityEngine;
using System.Collections;

public class DetachParticlesOnDestroy : MonoBehaviour {

	// Use this for initialization
    void OnDestroy()
    {
        foreach (var p in GetComponentsInChildren<ParticleSystem>(true))
        {
            p.transform.parent = null;
            Destroy(p.gameObject, p.time);
        }

        foreach (var t in GetComponentsInChildren<TrailRenderer>(true))
        {
            t.transform.parent = null;
            Destroy(t.gameObject, t.time);
        }
    }
}
