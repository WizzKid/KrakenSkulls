using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalMovementEnemy : MonoBehaviour {

    public static float Speed = 6.0f;
    private bool m_HasHitBottom;
    private Rigidbody2D rb;
    private SpriteRenderer m_Renderer;

	// Use this for initialization
	void Awake () {
        m_Renderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(-Speed, 0);

        m_HasHitBottom = false;
	}
	
	// Collisions
	void OnTriggerEnter2D (Collider2D trig) {
        if (trig.CompareTag("SideBoundary") || trig.CompareTag("Obstacle"))
        {
            if (!m_HasHitBottom)
            {
                rb.position = (new Vector2(rb.position.x, rb.position.y - 0.3f));
            }
            else
            {
                rb.position = (new Vector2(rb.position.x, rb.position.y + 0.3f));
            }

            rb.velocity = -rb.velocity;
            if (m_Renderer.flipX == true)
            {
                m_Renderer.flipX = false;
            }
            else {
                m_Renderer.flipX = true;
            }
        }

        else if (trig.CompareTag("BottomBoundary"))
        {
            rb.position = (new Vector2(rb.position.x, rb.position.y + 0.5f));
            m_HasHitBottom = true;
            GameController.SpawningCentipedes = true;
        }

        else if (trig.CompareTag("TopBoundary"))
        {
            rb.position = (new Vector2(rb.position.x, rb.position.y - 0.5f));
            m_HasHitBottom = false;
        }
	}
}
