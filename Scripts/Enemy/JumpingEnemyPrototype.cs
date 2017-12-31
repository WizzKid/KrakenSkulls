using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingEnemyPrototype : MonoBehaviour {

    private Rigidbody2D rb;
    public float m_JumpForce = 10.0f;
    public float m_MoveSpeed = 5.0f;
    [Header("Sprites")]
    private SpriteRenderer m_SpriteRenderer;
    public Sprite m_Seagull1;
    public Sprite m_Seagull2;

    // Use this for initialization
    void Awake () {
        rb = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();

        var x = Random.Range(0, 2);

        if (x == 0)
        {
            m_SpriteRenderer.sprite = m_Seagull1;
        }
        else
        {
            m_SpriteRenderer.sprite = m_Seagull2;
        }

        if (transform.position.x < 0f)
        {
            rb.AddForce(new Vector2(m_MoveSpeed, 0), ForceMode2D.Impulse);
        }
        else {
            rb.AddForce(new Vector2(-m_MoveSpeed, 0), ForceMode2D.Impulse);
        }
        
	}

	// FixedUpdate is called once per physics cycle
	void FixedUpdate () {
		
	}

    void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.CompareTag("SideBoundary"))
        {
            rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
        }
        else if (trig.CompareTag("BottomBoundary"))
        {
            rb.AddForce(new Vector2(0, m_JumpForce), ForceMode2D.Impulse);
        }
        else if (trig.CompareTag("Obstacle"))
        {
            Destroy(trig.gameObject);
        }
    }
}
