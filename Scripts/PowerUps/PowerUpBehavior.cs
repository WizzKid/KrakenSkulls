using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpBehavior : MonoBehaviour {

    private Rigidbody2D rb;

	// Use this for initialization
	void Awake () {
        rb = GetComponent<Rigidbody2D>();
	}   
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "BottomBoundary")
        {
            Invoke("Destroy", 1);
        }

        if (collision.tag == "TopBoundary")
        {
            rb.gravityScale = 0;
            rb.drag = 2.1f;
        }
    }

    void Destroy()
    {
        
    }
}
