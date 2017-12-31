using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageCollector : MonoBehaviour {

    public float lifeSpan;

    // Use this for initialization
    void Start() {
        StartCoroutine(Lifespan());
    }

    IEnumerator Lifespan() {
        yield return new WaitForSeconds(lifeSpan);
        Destroy(gameObject);
    }
}
