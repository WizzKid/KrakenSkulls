using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingAmmoType : MonoBehaviour {

    // Default Ammo Damage
    public float m_defaultAmmoDamage;
    public GameObject[] explosionPrefab;
    private bool isColliding;
    public int m_numBounces = 3;
    private int m_currentBounces;

    public float m_StartAngle = 45.0f;
    public float m_SpreadModifier = 1.0f;

    private Rigidbody2D rb;
    public float speed;

    // Use this for initialization
    void Start()
    {
        Instantiate(explosionPrefab[0], transform.position, Quaternion.identity);
        m_currentBounces = 0;

        rb = GetComponent<Rigidbody2D>();

        float velocity = transform.parent.GetComponent<Rigidbody2D>().velocity.x;
            //.Range(-m_SpreadModifier, m_SpreadModifier);

        transform.rotation = Quaternion.Euler(0, 0, -m_StartAngle + (velocity * -m_SpreadModifier));
        rb.AddForce(transform.up * speed, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        isColliding = false;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (isColliding) return;
        isColliding = true;

        if (collider.tag == "Obstacle")
        {
            collider.gameObject.GetComponent<ObstacleHealth>().DoObstacleDamage(m_defaultAmmoDamage);
            Instantiate(explosionPrefab[1], transform.position, Quaternion.identity);
        }

        if (collider.tag == "Centipede")
        {
            collider.gameObject.GetComponent<EnemyHealth>().DoDamage(m_defaultAmmoDamage);
            Score.score += 100;
            Instantiate(explosionPrefab[1], transform.position, Quaternion.identity);
        }

        if (collider.tag == "Enemy")
        {
            collider.gameObject.GetComponent<EnemyHealth>().DoDamage(m_defaultAmmoDamage);
            Score.score += 10;
            Instantiate(explosionPrefab[1], transform.position, Quaternion.identity);
        }

        if (collider.tag == "BossHead")
        {
            collider.GetComponent<OctopusController>().TakeDamage(m_defaultAmmoDamage, 0);
            Instantiate(explosionPrefab[1], transform.position, Quaternion.identity);
        }

        if (collider.tag == "BossRightArm")
        {
            collider.GetComponentInParent<OctopusController>().TakeDamage(m_defaultAmmoDamage, 1);
            Instantiate(explosionPrefab[1], transform.position, Quaternion.identity);
        }

        if (collider.tag == "BossLeftArm")
        {
            collider.GetComponentInParent<OctopusController>().TakeDamage(m_defaultAmmoDamage, 2);
            Instantiate(explosionPrefab[1], transform.position, Quaternion.identity);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        m_currentBounces++;
        if (m_currentBounces >= m_numBounces)
        {
            Instantiate(explosionPrefab[1], transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
