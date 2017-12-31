using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Player Controller is for player input and collisions.
public class PlayerController1 : MonoBehaviour {

    private AudioSource m_Source;
    private PlayerMovement1 m_PlayerMovement;
    private GameObject m_InvulnerabilityConveyance;
    private bool m_CanFire;
    private bool m_Invincible;
    public static bool Player1Dead;
    private bool m_IsColliding;
    private bool m_TakingDamage;
    private bool m_BossDamageCD;

    [Header ("General Properties")]
    public float m_HealthP1 = 5;
    public static float HealthP1;
    public float m_ShotDelay = 0.15f;
    public float m_RapidFireShotDelay = 0.13f;
    [Header ("PowerUp Properties")]
    public float m_ShotgunDuration = 5.0f;
    public float m_AoEDuration = 5.0f;
    public float m_RapidFireDuration = 5.0f;
    public float m_BouncingDuration = 5.0f;
    public float m_SpeedBoostDuration = 5.0f;
    public float m_SpeedBoostMultiplier = 2.0f;
    public float m_InvulnerabilityDuration = 5.0f;
    [Header ("Ammo Type References")]
    public GameObject m_CurrentAmmoType;
    public GameObject m_DefaultAmmoType;
    public GameObject m_ShotgunAmmoType;
    public GameObject m_AoEAmmoType;
    public GameObject m_RapidFireAmmoType;
    public GameObject m_BouncingAmmoType;
    [Header("Audio")]
    public AudioClip m_GunShotClip;
    public AudioClip m_OnHitMaleClip;
    public AudioClip m_DeathMaleClip;
    public AudioClip m_PowerUpClip;

    //private SpriteRenderer m_SpriteRenderer;
    //public Sprite m_FemaleSprite;

	// Use this for initialization
	void Awake () {
        m_Source = GetComponent<AudioSource>();
        m_PlayerMovement = GetComponent<PlayerMovement1>();
        m_InvulnerabilityConveyance = transform.GetChild(0).gameObject;
        m_InvulnerabilityConveyance.SetActive(false);
        Player1Dead = false;
        m_CanFire = true;
        m_Invincible = false;
        m_IsColliding = false;
        m_TakingDamage = false;
        m_BossDamageCD = true;

        if (GameController.TwoPlayer == false)
        {
            if (MainMenuButtons.characterChoice == 1)
            {
                gameObject.SetActive(false);
                HealthP1 = 0;
            }
        }

        HealthP1 = m_HealthP1;

        /*m_SpriteRenderer = GetComponent<SpriteRenderer>();
        if (MainMenuButtons.characterChoice == 1)
        {
            m_SpriteRenderer.sprite = m_FemaleSprite;
        }*/
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetButton("Fire1") && m_CanFire && m_CurrentAmmoType != m_RapidFireAmmoType && !Player1Dead)
        {
            m_CanFire = false;
            StartCoroutine(ShotDelay());
            Instantiate(m_CurrentAmmoType, transform.position, Quaternion.identity, transform);
        }
        else if (Input.GetButton("Fire1") && m_CanFire && m_CurrentAmmoType == m_RapidFireAmmoType && !Player1Dead)
        {
            m_CanFire = false;
            StartCoroutine(RapidFireShotDelay());
            Instantiate(m_CurrentAmmoType, transform.position, Quaternion.identity);
        }
        else if (Input.GetButtonDown("Share1") && HealthP1 > 1 && PlayerController2.HealthP2 < 5)
        {
            HealthP1--;
            PlayerController2.HealthP2++;
        }

        if (HealthP1 > 0 && Player1Dead)
        {
            Player1Dead = false;
        }
        m_IsColliding = false;
    }

    void OnTriggerEnter2D(Collider2D trig) {

        if (m_IsColliding)
        {
            return;
        }

        m_IsColliding = true;

        if (trig.tag == "Centipede" || trig.tag == "Enemy" || trig.tag == "EnemyProjectile")
        {
            if (!m_Invincible && !m_TakingDamage)
            {
                Destroy(trig.gameObject);
                m_TakingDamage = true;
                TakeDamage();
            }
        }
        else if (trig.tag == "BossRightArm" || trig.tag == "BossLeftArm")
        {
            if (!m_Invincible && !m_TakingDamage && m_BossDamageCD)
            {
                m_TakingDamage = true;
                TakeDamage();
                m_BossDamageCD = false;
                StartCoroutine(BossDamageDelay());
            }
        }
        else if (trig.tag == "ShotgunPowerUp")
        {
            m_Source.PlayOneShot(m_PowerUpClip, 0.35f);
            m_CurrentAmmoType = m_ShotgunAmmoType;
            Destroy(trig.gameObject);
            StartCoroutine(ShotgunDuration());
        }
        else if (trig.tag == "AoEPowerUp")
        {
            m_Source.PlayOneShot(m_PowerUpClip, 0.35f);
            m_CurrentAmmoType = m_AoEAmmoType;
            Destroy(trig.gameObject);
            StartCoroutine(AoEDuration());
        }
        else if (trig.tag == "RapidFirePowerUp")
        {
            m_Source.PlayOneShot(m_PowerUpClip, 0.35f);
            m_CurrentAmmoType = m_RapidFireAmmoType;
            Destroy(trig.gameObject);
            StartCoroutine(RapidFireDuration());
        }
        else if (trig.tag == "BouncingPowerUp")
        {
            m_Source.PlayOneShot(m_PowerUpClip, 0.35f);
            m_CurrentAmmoType = m_BouncingAmmoType;
            Destroy(trig.gameObject);
            StartCoroutine(BouncingDuration());
        }
        else if (trig.tag == "SpeedBoostPowerUp")
        {
            m_Source.PlayOneShot(m_PowerUpClip, 0.35f);
            m_PlayerMovement.XYSpeed *= m_SpeedBoostMultiplier;
            Destroy(trig.gameObject);
            StartCoroutine(SpeedBoostDuration());
        }
        else if (trig.tag == "InvulnerabilityPowerUp")
        {
            m_Source.PlayOneShot(m_PowerUpClip, 0.35f);
            m_Invincible = true;
            m_InvulnerabilityConveyance.SetActive(true);
            Destroy(trig.gameObject);
            StartCoroutine(InvulnerabilityDuration());
        }
        else if (trig.tag == "HealthPowerUp")
        {
            m_Source.PlayOneShot(m_PowerUpClip, 0.35f);
            if (HealthP1 < 5)
            {
                HealthP1++;
            }
            Destroy(trig.gameObject);
        }
    }

    private void TakeDamage()
    {
        if (HealthP1 > 0)
        {
            m_Source.PlayOneShot(m_OnHitMaleClip, 0.8f);
            HealthP1--;
        }

        if (HealthP1 == 0)
        {
            Player1Dead = true;            
            m_Source.PlayOneShot(m_DeathMaleClip, 1f);
        }

        m_TakingDamage = false;
        //print(Health);
    }

    IEnumerator ShotDelay()
    {
        m_Source.PlayOneShot(m_GunShotClip, 0.045f);
        yield return new WaitForSeconds(m_ShotDelay);
        m_CanFire = true;
    }

    IEnumerator RapidFireShotDelay()
    {
        m_Source.PlayOneShot(m_GunShotClip, 0.045f);
        yield return new WaitForSeconds(m_RapidFireShotDelay);
        m_CanFire = true;
    }

    IEnumerator ShotgunDuration()
    {
        yield return new WaitForSeconds(m_ShotgunDuration);
        m_CurrentAmmoType = m_DefaultAmmoType;
    }

    IEnumerator AoEDuration()
    {
        yield return new WaitForSeconds(m_AoEDuration);
        m_CurrentAmmoType = m_DefaultAmmoType;
    }

    IEnumerator RapidFireDuration()
    {
        yield return new WaitForSeconds(m_RapidFireDuration);
        m_CurrentAmmoType = m_DefaultAmmoType;
    }

    IEnumerator BouncingDuration()
    {
        yield return new WaitForSeconds(m_BouncingDuration);
        m_CurrentAmmoType = m_DefaultAmmoType;
    }

    IEnumerator SpeedBoostDuration()
    {
        yield return new WaitForSeconds(m_SpeedBoostDuration);
        m_PlayerMovement.XYSpeed /= m_SpeedBoostMultiplier;
    }

    IEnumerator InvulnerabilityDuration()
    {
        yield return new WaitForSeconds(m_InvulnerabilityDuration);
        m_Invincible = false;
        m_InvulnerabilityConveyance.SetActive(false);
    }

    IEnumerator BossDamageDelay()
    {
        yield return new WaitForSeconds(2f);
        m_BossDamageCD = true;
    }
}
