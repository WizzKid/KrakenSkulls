using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {

    public GameObject m_deathParticle;
    public GameObject m_ObstaclePrefab;
    public static float Health = 1.0f;
    private float m_currentHealth;

    public delegate void CentipedeDeath();
    public static event CentipedeDeath OnDeath;

    public AudioClip m_DeathAudio;

    private bool isApplicationQuitting = false;

    // Use this for initialization
    void Awake () {
        if (transform.tag == "Centipede")
        {
            m_currentHealth = Health;
        }
        else
        {
            m_currentHealth = 1.0f;
        }
	}

    // Handle health
    public void DoDamage(float damage) {
        m_currentHealth -= damage;

        if (m_currentHealth <= 0)
        {
            AudioSource.PlayClipAtPoint(m_DeathAudio, transform.position);
            if (gameObject.tag == "Centipede")
            {
                Instantiate(m_ObstaclePrefab, transform.position, Quaternion.identity);
            }
            Instantiate(m_deathParticle, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    void OnApplicationQuit()
    {
        isApplicationQuitting = true;
    }

    void OnDestroy()
    {
        if (gameObject.tag == "Centipede" && !isApplicationQuitting && !MainMenuButtons.loadingScene)
        {
            OnDeath();
        }
    }
}
