using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultAmmoType : MonoBehaviour {

    // Default Ammo Damage
    public float m_defaultAmmoDamage;
    public GameObject[] explosionPrefab;
    private bool isColliding;

	// Use this for initialization
	void Start () {
        Instantiate(explosionPrefab[0], transform.position, Quaternion.identity);
    }
	
	// Update is called once per frame
	void Update () {
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
            Destroy(gameObject);
        }

        if (collider.tag == "Centipede")
        {
            collider.gameObject.GetComponent<EnemyHealth>().DoDamage(m_defaultAmmoDamage);
            Score.score += 100;
            Destroy(gameObject);
        }

        if (collider.tag == "Enemy")
        {
            collider.gameObject.GetComponent<EnemyHealth>().DoDamage(m_defaultAmmoDamage);
            Score.score += 10;
            Destroy(gameObject);
        }

        if (collider.tag == "BossHead")
        {
            collider.GetComponent<OctopusController>().TakeDamage(m_defaultAmmoDamage, 0);
            Instantiate(explosionPrefab[1], transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        if (collider.tag == "BossRightArm")
        {
            collider.GetComponentInParent<OctopusController>().TakeDamage(m_defaultAmmoDamage, 1);
            Instantiate(explosionPrefab[1], transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        if (collider.tag == "BossLeftArm")
        {
            collider.GetComponentInParent<OctopusController>().TakeDamage(m_defaultAmmoDamage, 2);
            Instantiate(explosionPrefab[1], transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
