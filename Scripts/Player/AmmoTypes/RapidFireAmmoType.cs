using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFireAmmoType : MonoBehaviour {

    // Default Ammo Damage
    public float m_RapidFireAmmoDamage;
    public GameObject[] explosionPrefab;
    private bool isColliding;

    public float m_StartAngle = 45.0f;
    public float m_SpreadModifier = 5.0f;

    private Rigidbody2D rb;
    public float speed;

    // Use this for initialization
    void Start()
    {
        Instantiate(explosionPrefab[0], transform.position, Quaternion.identity);

        float i = Random.Range(-m_SpreadModifier, m_SpreadModifier);
        
        rb = GetComponent<Rigidbody2D>();
        transform.rotation = Quaternion.Euler(0, 0, -m_StartAngle + (i * m_SpreadModifier));
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
            collider.gameObject.GetComponent<ObstacleHealth>().DoObstacleDamage(m_RapidFireAmmoDamage);

            Instantiate(explosionPrefab[1], transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        if (collider.tag == "Centipede")
        {
            collider.gameObject.GetComponent<EnemyHealth>().DoDamage(m_RapidFireAmmoDamage);
            Score.score += 100;
            Destroy(gameObject);
        }

        if (collider.tag == "Enemy")
        {
            collider.gameObject.GetComponent<EnemyHealth>().DoDamage(m_RapidFireAmmoDamage);
            Score.score += 10;
            Destroy(gameObject);
        }

        if (collider.tag == "BossHead")
        {
            collider.GetComponent<OctopusController>().TakeDamage(m_RapidFireAmmoDamage, 0);
            Instantiate(explosionPrefab[1], transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        if (collider.tag == "BossRightArm")
        {
            collider.GetComponentInParent<OctopusController>().TakeDamage(m_RapidFireAmmoDamage, 1);
            Instantiate(explosionPrefab[1], transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        if (collider.tag == "BossLeftArm")
        {
            collider.GetComponentInParent<OctopusController>().TakeDamage(m_RapidFireAmmoDamage, 2);
            Instantiate(explosionPrefab[1], transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
