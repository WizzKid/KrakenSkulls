using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObstacleHealth : MonoBehaviour {

    public float m_startingHealth;
    private float m_currentHealth;
    private SpriteRenderer m_SpriteRenderer;
    public Sprite m_RockOption1;
    public Sprite m_Rock1Damaged1;
    public Sprite m_Rock1Damaged2;
    public Sprite m_RockOption2;
    public Sprite m_Rock2Damaged1;
    public Sprite m_Rock2Damaged2;

    public AudioClip m_RockBreakClip;

    private int RockType;

    private bool m_SpawnAnimation;
    public float m_OpacityScaler = 0.1f;

    private Color tempcolor;

    public GameObject m_WaterParticle;
    public GameObject m_DestroyParticle;

    private bool isApplicationQuitting = false;

    // Use this for initialization
    void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();

        RockType = Random.Range(0, 2);
        if (RockType == 0)
        {
            m_SpriteRenderer.sprite = m_RockOption1;
        }
        else {
            m_SpriteRenderer.sprite = m_RockOption2;
        }

        m_currentHealth = m_startingHealth;

        tempcolor = m_SpriteRenderer.color;
        tempcolor.a = 0.0f;
        m_SpawnAnimation = true;
        Instantiate(m_WaterParticle, new Vector3(transform.position.x, transform.position.y - 0.12f, transform.position.z), Quaternion.Euler(0,0,210));
    }

    // FixedUpdate runs every fixed frame
    void FixedUpdate()
    {
     if (m_SpawnAnimation)
        {
            tempcolor.a = Mathf.MoveTowards(tempcolor.a, 1.0f, (m_OpacityScaler * Time.deltaTime));
            m_SpriteRenderer.color = tempcolor;
            if (m_SpriteRenderer.color.a >= 0.99f)
            {
                m_SpawnAnimation = false;
            }
        }
    }

    // Subtract health on collision with projectile based on incoming damage
    public void DoObstacleDamage(float m_damage) {
        m_currentHealth -= m_damage;

        if (RockType == 0)
        {
            SetRock1DamageSprite();
        }
        else {
            SetRock2DamageSprite();
        }

        if (m_currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    void SetRock1DamageSprite()
    {
        if (m_currentHealth == 2)
        {
            m_SpriteRenderer.sprite = m_Rock1Damaged1;
        }
        if (m_currentHealth == 1)
        {
            m_SpriteRenderer.sprite = m_Rock1Damaged2;
        }
    }

    void SetRock2DamageSprite()
    {
        if (m_currentHealth == 2)
        {
            m_SpriteRenderer.sprite = m_Rock2Damaged1;
        }
        if (m_currentHealth == 1)
        {
            m_SpriteRenderer.sprite = m_Rock2Damaged2;
        }
    }

    void OnApplicationQuit()
    {
        isApplicationQuitting = true;
    }

    void OnDestroy()
    {
        if (!isApplicationQuitting && !GameOver.GameOverStart && !MainMenuButtons.loadingScene)
        {
            AudioSource.PlayClipAtPoint(m_RockBreakClip, transform.position);
            Instantiate(m_DestroyParticle, transform.position, Quaternion.identity);
        }
        else if (MainMenuButtons.loadingScene)
        {
            AudioSource.PlayClipAtPoint(m_RockBreakClip, transform.position, 0.15f);
            Instantiate(m_DestroyParticle, transform.position, Quaternion.identity);
        }
    }
}
