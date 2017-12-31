using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PirateMovement : MonoBehaviour {

    private Rigidbody2D rb;
    public float m_MoveSpeed = 3.5f;
    public float m_AttackRate = 0.8f;
    public GameObject m_Projectile;
    public GameObject m_Explosion;
    public GameObject[] m_PowerUps;
    private float m_DontDestroyOnSpawn = 2.0f;
    private bool m_CanDestroy = false;

	// Use this for initialization
	void Awake () {
        rb = GetComponent<Rigidbody2D>();

        if (transform.position.x > 0f)
        {
            rb.AddForce(new Vector2(-m_MoveSpeed, 0f), ForceMode2D.Impulse);
        }
        else {
            rb.AddForce(new Vector2(m_MoveSpeed, 0f), ForceMode2D.Impulse);
        }

        InvokeRepeating("Attack", 0f, m_AttackRate);
        StartCoroutine(SpawnDontDestroy());
	}

    // Update is called once per frame
    void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SideBoundary") && m_CanDestroy)
        {
            StartCoroutine(BoundaryDeathDelay());
            CancelInvoke();
        }
        else if (collision.CompareTag("PlayerProjectile"))
        {
            int temp = Random.Range(0, m_PowerUps.Length);
            Instantiate(m_PowerUps[temp], transform.position, Quaternion.identity);
        }
    }

    void Attack()
    {
        Instantiate(m_Projectile, transform.position, Quaternion.identity);
        Instantiate(m_Explosion, transform.position, Quaternion.identity);
    }

    IEnumerator BoundaryDeathDelay()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    IEnumerator SpawnDontDestroy()
    {
        yield return new WaitForSeconds(m_DontDestroyOnSpawn);
        m_CanDestroy = true;
    }
}
