using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleDisableScript : MonoBehaviour {

    public AudioClip m_DisableSound;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnDisable()
    {
        AudioSource.PlayClipAtPoint(m_DisableSound, transform.position);
    }
}
