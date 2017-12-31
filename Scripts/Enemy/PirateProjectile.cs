using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PirateProjectile : MonoBehaviour {

    public float m_Speed = 2.2f;
    private Rigidbody2D rb;

	// Use this for initialization
	void Awake () {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(0f, -m_Speed));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
