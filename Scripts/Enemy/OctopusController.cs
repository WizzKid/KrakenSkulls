using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OctopusController : MonoBehaviour {

    private GameController m_GameController;
    private Animator m_Animator;

    public float m_DefaultHeadHealth = 300;
    public float m_DefaultArmHealth;
    private float m_RightArmHealth;
    private float m_LeftArmHealth;
    private float m_HeadHealth;

    public int m_NumStates = 3;

    private bool m_State_Idle;
    private bool m_State_ArmAttacks;
    private bool m_State_Projectiles;
    private bool m_State_SpawnSeagulls;
    private bool m_Rage;
    public float m_RageAnimSpeed = 2.0f;

    private int m_CurrentState;    

    public float m_AttackDelay = 10.0f;
    public float m_Default_Speed_Scaler = 1.0f;
    public static float Speed_Scaler = 1.0f;

    public static bool BossDead;

    private Transform m_RightArm;
    public static bool RightArmDead;
    private Transform m_LeftArm;
    public static bool LeftArmDead;

    private Vector2 m_AttackPos;
    private Transform m_PlayerTransform1;
    private Transform m_PlayerTransform2;
    private Transform m_Target;
    private bool m_TargetFlipFlop = false;

    public Transform startMarker;
    public float m_ArmSpeed = 1.0F;

    private GameObject m_ArmAttackConveyance;
    private bool m_ConveyArmAttack;
    private bool m_RightArmAttack;
    private bool m_LeftArmAttack;

    private Vector3 m_RightArmStartPos;
    private Vector3 m_LeftArmStartPos;

    public GameObject m_Projectile;

    public float m_PirateSpawnRate = 5.0f;

    private bool isApplicationQuitting = false;

    public GameObject m_DeathParticle;
    public int m_DeathParticleCount = 25;

    public AudioClip m_BossDeathAudio;
    public AudioClip m_ExplodeAudio;

    private Image m_HealthbarUI;
    private Image m_Healthbar;
    private Image[] m_UIList;

    private BackgroundChanger m_BackgroundChanger;

    // Use this for initialization - lots of initialization
    void Awake () {
        BossDead = false;
        RightArmDead = false;
        LeftArmDead = false;
        m_RightArmHealth = m_DefaultArmHealth *= Speed_Scaler;
        m_LeftArmHealth = m_DefaultArmHealth *= Speed_Scaler;
        m_HeadHealth = m_DefaultHeadHealth *= Speed_Scaler;

        if (!GameController.TwoPlayer && MainMenuButtons.characterChoice == 0)
        {
            m_PlayerTransform1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<Transform>();
        }
        if (!GameController.TwoPlayer && MainMenuButtons.characterChoice == 1)
        {
            m_PlayerTransform2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<Transform>();
        }
        if (GameController.TwoPlayer)
        {
            m_PlayerTransform1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<Transform>();
            m_PlayerTransform2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<Transform>();
        }
        m_RightArm = transform.GetChild(0);
        m_RightArmStartPos = m_RightArm.position;
        m_LeftArm = transform.GetChild(1);
        m_LeftArmStartPos = m_LeftArm.position;

        m_ArmAttackConveyance = transform.GetChild(2).gameObject;
        m_ArmAttackConveyance.SetActive(false);
        m_ConveyArmAttack = false;
        m_RightArmAttack = false;
        m_LeftArmAttack = false;

        m_GameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        m_Animator = GetComponent<Animator>();

        m_HealthbarUI = GameObject.FindGameObjectWithTag("OctopusUI").GetComponent<Image>();
        // Get all image components of OctopusUI's children
        m_UIList = m_HealthbarUI.GetComponentsInChildren<Image>(true);
        for (var i = 0; i < m_UIList.Length; i++)
        {
            // Turn on healthbarUI component
            m_UIList[i].enabled = true;
        }
        m_Healthbar = GameObject.FindGameObjectWithTag("OctopusHealthUI").GetComponent<Image>();
        m_Healthbar.fillAmount = m_HeadHealth / m_DefaultHeadHealth; // Set healthbar fill amount

        m_CurrentState = 1; // Set state

        StartCoroutine(Idle()); // Start fight
        InvokeRepeating("SpawnPirates", 2, m_PirateSpawnRate); // Spawn pirates

        if (!GameController.TwoPlayer)
        {
            if (MainMenuButtons.characterChoice == 0)
            {
                m_Target = m_PlayerTransform1;
            }
            else if (MainMenuButtons.characterChoice == 1)
            {
                m_Target = m_PlayerTransform2;
            }
        }
        // Set new background
        m_BackgroundChanger = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundChanger>();
        m_BackgroundChanger.CycleBG();
    }
	
	// FixedUpdate is called once per physics frame
	void FixedUpdate () {
        LerpArmPosition();
	}

    public void TakeDamage(float damage, int part)
    {
        switch (part)
        {
            case 0: // head
                m_HeadHealth -= damage; // Take damage
                m_Healthbar.fillAmount = m_HeadHealth/m_DefaultHeadHealth; // Update healthbar
                if (m_HeadHealth <= m_DefaultHeadHealth/2)
                {
                    m_Rage = true;
                    m_Animator.speed = m_RageAnimSpeed;
                    if (m_HeadHealth <= 0 && !BossDead && !isApplicationQuitting)
                    {
                        BossDead = true;
                        StartCoroutine(BossDeadCoroutine());
                    }
                }
                break;
            case 1: // right arm
                m_RightArmHealth -= damage;
                if (m_RightArmHealth <= 0)
                {
                    RightArmDead = true;
                    // Animation of arm dying.
                    // Instantiation of particles.
                    m_RightArm.gameObject.SetActive(false);
                }
                break;
            case 2: // left arm
                m_LeftArmHealth -= damage;
                if (m_LeftArmHealth <= 0)
                {
                    LeftArmDead = true;
                    // Animation of arm dying.
                    // Instantiation of particles.
                    m_LeftArm.gameObject.SetActive(false);
                }
                break;
            default:
                print("TakeDamage(part) out of range");
                break;
        }
    }

    IEnumerator Idle()
    {
        m_Animator.CrossFade("OctopusIdle", 0.04f);
        yield return new WaitForSeconds(m_AttackDelay / Speed_Scaler);

        switch (m_CurrentState)
        {
            case 1:
                if (!RightArmDead || !LeftArmDead)
                {
                    StartCoroutine(Arm_Attacks());
                }
                else
                {
                    StartCoroutine(RegrowArms());
                }
                break;
            case 2:
                StartCoroutine(Projectiles());
                break;
            case 3:
                StartCoroutine(Spawn_Enemies());
                break;
            default:
                print("StateExecution int out of range");
                break;
        }
        if (m_CurrentState == m_NumStates)
        {
            m_CurrentState = 0;
        }
    }

    IEnumerator Arm_Attacks()
    {
        if (!RightArmDead || !LeftArmDead)
        {
            m_Animator.CrossFade("OctopusAttacking", 0.2f);
        }
        // Right arm attack
        if (!RightArmDead)
        {
            if (GameController.TwoPlayer)
            {
                //var temp = Random.Range(0, 2);
                /*if (temp == 0)
                {
                    m_Target = m_PlayerTransform1;
                }
                else
                {
                    m_Target = m_PlayerTransform2;
                }*/
                if (m_TargetFlipFlop)
                {
                    m_Target = m_PlayerTransform1;
                }
                else
                {
                    m_Target = m_PlayerTransform2;
                }
                m_TargetFlipFlop = !m_TargetFlipFlop;
            }
            m_Animator.CrossFade("OctopusRightArmAttack", 0.08f);
            m_ConveyArmAttack = true;
            m_ArmAttackConveyance.SetActive(true);
            yield return new WaitForSeconds(m_AttackDelay / Speed_Scaler / 3);
            m_ConveyArmAttack = false;
            // Play sound
            yield return new WaitForSeconds(m_AttackDelay / Speed_Scaler / 10);
            m_RightArmAttack = true;
            yield return new WaitForSeconds(m_AttackDelay / Speed_Scaler / 5);
            m_RightArmAttack = false;
            m_Animator.CrossFade("OctopusAttacking", 0.08f);
        }
        // Left arm attack
        if (!LeftArmDead)
        {
            if (GameController.TwoPlayer)
            {
                /*var temp = Random.Range(0, 2);
                if (temp == 0)
                {
                    m_Target = m_PlayerTransform1;
                }
                else
                {
                    m_Target = m_PlayerTransform2;
                }*/
                if (m_TargetFlipFlop)
                {
                    m_Target = m_PlayerTransform1;
                }
                else
                {
                    m_Target = m_PlayerTransform2;
                }
                m_TargetFlipFlop = !m_TargetFlipFlop;
            }
            m_Animator.CrossFade("OctopusLeftArmAttack", 0.08f);
            m_ConveyArmAttack = true;
            yield return new WaitForSeconds(m_AttackDelay / Speed_Scaler / 3);
            m_ConveyArmAttack = false;
            // Play sound
            yield return new WaitForSeconds(m_AttackDelay / Speed_Scaler / 10);
            m_LeftArmAttack = true;
            yield return new WaitForSeconds(m_AttackDelay / Speed_Scaler / 5);
            m_LeftArmAttack = false;
            m_Animator.CrossFade("OctopusAttacking", 0.08f);
        }
        // Resets arm positions
        m_ArmAttackConveyance.SetActive(false);
        m_ConveyArmAttack = true;
        yield return new WaitForSeconds(m_AttackDelay / Speed_Scaler / 5);
        m_ConveyArmAttack = false;
        // No arms dead, continue
        if (!RightArmDead || !LeftArmDead)
        {
            // Attacks done, next state
            m_CurrentState++;
            StartCoroutine(Idle());
        }
        // Some arm dead, regrow arms
        else {
            StartCoroutine(RegrowArms());
        }
    }

    void LerpArmPosition()
    {
        if (m_ConveyArmAttack)
        {
            m_ArmAttackConveyance.transform.position = Vector2.MoveTowards(m_ArmAttackConveyance.transform.position, m_Target.position, m_ArmSpeed * Speed_Scaler);
            m_RightArm.transform.position = Vector2.MoveTowards(m_RightArm.transform.position, m_RightArmStartPos, m_ArmSpeed * Speed_Scaler);
            m_LeftArm.transform.position = Vector2.MoveTowards(m_LeftArm.transform.position, m_LeftArmStartPos, m_ArmSpeed * Speed_Scaler);
        }
        else if (m_RightArmAttack)
        {
            m_RightArm.transform.position = Vector2.MoveTowards(m_RightArm.transform.position, m_ArmAttackConveyance.transform.position, m_ArmSpeed * Speed_Scaler);
        }
        else if (m_LeftArmAttack)
        {
            m_LeftArm.transform.position = Vector2.MoveTowards(m_LeftArm.transform.position, m_ArmAttackConveyance.transform.position, m_ArmSpeed * Speed_Scaler);
        }
    }

    IEnumerator RegrowArms()
    {
        m_Animator.CrossFade("OctopusRegrowArms", 0.05f);
        yield return new WaitForSeconds(3);
        if (RightArmDead)
        {
            m_RightArmHealth = m_DefaultArmHealth;
            m_RightArm.gameObject.SetActive(true);
            RightArmDead = false;
            yield return new WaitForSeconds(3);
        }
        if (LeftArmDead)
        {
            m_LeftArmHealth = m_DefaultArmHealth;
            m_LeftArm.gameObject.SetActive(true);
            LeftArmDead = false;
            yield return new WaitForSeconds(3);
        }
        if (m_Rage)
        {
            m_GameController.SpawnFallingGuys();
            m_GameController.SpawnSpiders();
            m_GameController.SpawnFallingGuys();
            m_GameController.SpawnSpiders();
        }
        m_CurrentState++;
        StartCoroutine(Idle());
    }

    IEnumerator Projectiles()
    {
        m_Animator.CrossFade("OctopusProjectileAnim", 0.05f);
        if (m_Rage)
        {
            m_GameController.SpawnFallingGuys();
        }
        Instantiate(m_Projectile, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(m_AttackDelay / Speed_Scaler / 10);
        Instantiate(m_Projectile, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(m_AttackDelay / Speed_Scaler / 10);
        if (m_Rage)
        {
            m_GameController.SpawnFallingGuys();
        }
        Instantiate(m_Projectile, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(m_AttackDelay / Speed_Scaler / 10);
        Instantiate(m_Projectile, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(m_AttackDelay / Speed_Scaler / 10);
        if (m_Rage)
        {
            m_GameController.SpawnFallingGuys();
        }
        Instantiate(m_Projectile, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(m_AttackDelay / Speed_Scaler / 10);
        Instantiate(m_Projectile, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(m_AttackDelay / Speed_Scaler / 10);
        if (m_Rage)
        {
            m_GameController.SpawnFallingGuys();
        }
        Instantiate(m_Projectile, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(m_AttackDelay / Speed_Scaler / 10);
        Instantiate(m_Projectile, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(m_AttackDelay / Speed_Scaler / 10);
        if (m_Rage)
        {
            m_GameController.SpawnFallingGuys();
        }
        Instantiate(m_Projectile, transform.position, Quaternion.identity);
        m_CurrentState++;
        StartCoroutine(Idle());
    }

    IEnumerator Spawn_Enemies()
    {
        m_GameController.SpawnSpiders();
        yield return new WaitForSeconds(m_AttackDelay / Speed_Scaler / 10);
        m_GameController.SpawnSpiders();
        m_GameController.SpawnFallingGuys();
        yield return new WaitForSeconds(m_AttackDelay / Speed_Scaler / 10);
        m_GameController.SpawnSpiders();
        yield return new WaitForSeconds(m_AttackDelay / Speed_Scaler / 10);
        m_GameController.SpawnSpiders();
        m_GameController.SpawnFallingGuys();
        yield return new WaitForSeconds(m_AttackDelay / Speed_Scaler / 10);
        m_GameController.SpawnSpiders();
        yield return new WaitForSeconds(m_AttackDelay / Speed_Scaler / 10);
        m_GameController.SpawnSpiders();
        m_GameController.SpawnFallingGuys();
        yield return new WaitForSeconds(m_AttackDelay / Speed_Scaler / 10);
        m_GameController.SpawnSpiders();
        yield return new WaitForSeconds(m_AttackDelay / Speed_Scaler / 10);
        if (m_Rage)
        {
            m_GameController.SpawnFallingGuys();
            m_GameController.SpawnFallingGuys();
            m_GameController.SpawnFallingGuys();
        }
        m_CurrentState++;
        StartCoroutine(Idle());
    }

    void SpawnPirates()
    {
        m_GameController.SpawnPirates();
    }

    void OnApplicationQuit()
    {
        isApplicationQuitting = true;
    }

    IEnumerator BossDeadCoroutine()
    {
        for (var i = 0; i < m_UIList.Length; i++)
        {
            // Turn off healthbarUI component
            m_UIList[i].enabled = false;
        }
        CancelInvoke();
        m_Animator.speed = 0.5f;
        m_CurrentState = 5;
        for (var i = 0; i < m_DeathParticleCount; i++)
        {
            if (m_Animator.speed > 0f)
            {
                m_Animator.speed -= 0.1f;
            }
            yield return new WaitForSeconds(0.1f);
            Vector3 pos = new Vector2(transform.position.x + Random.Range(-1.7f, 1.7f), transform.position.y + Random.Range(-1.7f, 1.7f));
            Instantiate(m_DeathParticle, pos, Quaternion.identity);
            AudioSource.PlayClipAtPoint(m_ExplodeAudio, transform.position);
        }
        AudioSource.PlayClipAtPoint(m_BossDeathAudio, transform.position, 0.8f);
        yield return new WaitForSeconds(0.25f);
        m_GameController.BossDead();
        Speed_Scaler += 0.15f;

        Destroy(gameObject);
    }
}