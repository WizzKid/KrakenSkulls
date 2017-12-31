using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public static bool TwoPlayer;

    [Header("Modes")]
    public bool m_Centipede;
    public bool m_Boss;
    public bool m_TwoPlayerOverride;

    [Header("Settings")]
    public int m_DefaultCentipedeLength = 7;
    private int m_CentipedeLength;
    public float m_CentipedeSpacing = 0.05f;

    private int m_CentipedeCount;
    public GameObject m_CentipedePrefab;
    public Sprite m_FirstCentipedeSprite;
    public Sprite m_MiddleCentipedeSprite1;
    public Sprite m_MiddleCentipedeSprite2;
    public Sprite m_LastCentipedeSprite;
    public GameObject m_EmptyObject;
    //private List<GameObject> m_CentipedeList;

    public int m_BossScorePreReq = 5000;
    public GameObject m_Boss1Prefab;
    private SpriteRenderer m_SpriteRenderer;
    private AudioSource m_BossSpawnAudio;

    public float m_NextLevelDelay = 4.0f;

    public bool SpawningSpiders;
    public GameObject m_SpiderPrefab;
    public float m_SpiderSpawnRate = 15.0f;
    public float m_SpiderDelay = 5.0f;
    public static bool SpawningCentipedes;
    public float m_CentipedeSpawnRate = 10.0f;
    public bool SpawningFallingGuys;
    public GameObject m_FallingGuyPrefab;
    public float m_FallingGuyDelay = 15.0f;
    public float m_FallingGuySpawnRate = 15.0f;
    public GameObject m_PiratePrefab;
    public float m_PirateDelay = 10.0f;
    public float m_PirateSpawnRate = 15.0f;

    private Text m_LevelUI;
    private int m_LevelNum = 1;

    public int m_NumberOfObstacles;

    public GameObject m_Obstacle;
    private float m_NextObstacle;
    public float m_ObstacleSpawnDelay;

    private float randx;
    private float randy;

    private int m_FinishedObstacles;

    private bool m_DestroyObstacles;
    private GameObject[] m_OldObs;
    private int m_ObsIterator;

    // Use this for initialization
    void Awake () {
        SpawningCentipedes = false;
        // Not necessary to track centipedes this way
        //m_CentipedeList = new List<GameObject>();
        m_CentipedeLength = m_DefaultCentipedeLength;
        m_CentipedeCount = m_CentipedeLength;
        EnemyHealth.Health = 1;
        m_LevelUI = GameObject.FindGameObjectWithTag("LevelText").GetComponent<Text>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_BossSpawnAudio = GetComponent<AudioSource>();

        m_FinishedObstacles = 0;
        m_NextObstacle = Time.time + m_ObstacleSpawnDelay;
        m_DestroyObstacles = false;
        m_ObsIterator = 0;

        // Centipede mode
        // Put centipede parts in a list for easy management of moving parts in different directions
        if (m_Centipede)
        {
            StartCentipedeLevel();
        }

        // Boss mode
        else if (m_Boss)
        {
            Instantiate(m_Boss1Prefab);
        }

        // Debug
        else
        {
            print("No mode selected");
        }

        // TwoPlayer PIE Testing
        if (m_TwoPlayerOverride)
        {
            TwoPlayer = true;
        }
        
	}

    // Event listeners
    void OnEnable()
    {
        EnemyHealth.OnDeath += PartDestroyed;
    }

    void OnDisable()
    {
        EnemyHealth.OnDeath -= PartDestroyed;
    }

    // Update is called once per frame
    void Update () {
        if (Time.time >= m_NextObstacle && m_FinishedObstacles < m_NumberOfObstacles)
        {
            m_NextObstacle = Time.time + m_ObstacleSpawnDelay;
            SpawnObstacles();
        }
        if (Time.time >= m_NextObstacle && m_DestroyObstacles && m_ObsIterator < m_OldObs.Length)
        {
            m_NextObstacle = Time.time + m_ObstacleSpawnDelay;
            Destroy(m_OldObs[m_ObsIterator]);
            m_ObsIterator++;
        }
    }

    // Event triggered when part of the centipede is killed - Handles when it should split or not.
    // Commented out code is array based splitting of centipede, which while functional, didn't scale up and was not necessary
    void PartDestroyed()
    {
        /*
        for (var i = 0; i < m_CentipedeList.Count; i++) // Search for destroyed part
        {
            if (m_CentipedeList[i] == null) // Found the destroyed part
            {
                m_CentipedeList[i] = Instantiate(m_EmptyObject); // Set list position to an empty game object to avoid it later
                if (i - 1 >= 0 && i + 1 < m_CentipedeList.Count) // List has capacity for part to be surrounded by other parts
                {
                    if (m_CentipedeList[i - 1].CompareTag("EmptyObject") == false && m_CentipedeList[i + 1].CompareTag("EmptyObject") == false) // The destroyed part was surrounded by other parts
                    {
                        i++; // Iterate past destroyed part
                        while (i < m_CentipedeList.Count) // Reverse velocity of parts past the destroyed part but exit the loop if a gap is found
                        {
                            if (m_CentipedeList[i] != null && m_CentipedeList[i].CompareTag("EmptyObject") == false) // Failsafe to skip empty or null spaces in list
                            {
                                m_CentipedeList[i].gameObject.GetComponent<Rigidbody2D>().velocity = -m_CentipedeList[i].gameObject.GetComponent<Rigidbody2D>().velocity;
                            }
                            i++;
                        }
                        //print("Split");
                        break;
                    }
                }
                else
                {
                    //print("No split");
                    break;
                }
            }
            else {
                //print(m_CentipedeList[i]);
            }
        }
        */

        m_CentipedeCount--;

        // All Centipedes cleared - Next level
        if (m_CentipedeCount <=0)
        {
            StartCoroutine(NextLevel());
            // Clear old obstacles and spawn new ones
            m_OldObs = GameObject.FindGameObjectsWithTag("Obstacle");
            m_ObsIterator = 0;
            m_DestroyObstacles = true;
            // Level Increment and update UI
            m_LevelNum++;
            m_LevelUI.text = "Level " + m_LevelNum;
        }
    }

    public void SpawnSpiders()
    {
        float x;
        float rand_y;
        float temp = Random.Range(0f, 1f);

        if (temp < 0.5f)
        {
            x = -8.5f;
        }
        else
        {
            x = 8.5f;
        }

        rand_y = Random.Range(-1f, -2.5f);

        Instantiate(m_SpiderPrefab, new Vector3(x, rand_y,0f), Quaternion.identity);
    }

    public void SpawnCentipedes()
    {
        if (SpawningCentipedes)
        {
            float x;
            float rand_y;
            float temp = Random.Range(0, 1);

            if (temp < 0.5f)
            {
                x = -8.5f;
            }
            else
            {
                x = 8.5f;
            }

            rand_y = Random.Range(-2f, -4.5f);

            Instantiate(m_CentipedePrefab, new Vector2(x, rand_y), Quaternion.identity);
            m_CentipedeCount++;
        }
    }

    public void SpawnFallingGuys()
    {
        float temp = Random.Range(-8.0f, 8.0f);

        Instantiate(m_FallingGuyPrefab, new Vector2(temp, 5.0f), Quaternion.identity);
    }

    public void SpawnPirates()
    {
        float x;
        float rand_y;
        float temp = Random.Range(0f, 1f);

        if (temp < 0.5f)
        {
            x = -8.5f;
        }
        else
        {
            x = 8.5f;
        }

        rand_y = Random.Range(2.5f, 3.7f);

        Instantiate(m_PiratePrefab, new Vector3(x, rand_y, 0f), Quaternion.identity);
    }

    // Chooses boss or new centipede level without cancelling enemy spawns
    IEnumerator NextLevel()
    {
        // Stop spawning stuff centipedes, fixes double level glitch with spawning centipedes function
        SpawningCentipedes = false;

        if (Score.score >= m_BossScorePreReq) // Boss score prerequisite met
        {
            m_SpriteRenderer.enabled = false;
            m_BossSpawnAudio.Play();
        }

        yield return new WaitForSeconds(m_NextLevelDelay);

        if (Score.score >= m_BossScorePreReq) // Boss score prerequisite met
        {
            CancelInvoke();
            m_Centipede = false;
            m_Boss = true;
            m_CentipedeLength = m_DefaultCentipedeLength;
            m_CentipedeCount = m_CentipedeLength;
            StartBossLevel(); // Spawn Boss
            m_BossScorePreReq += 5000;
        }
        else
        {
            m_CentipedeLength++;
            m_CentipedeCount = m_CentipedeLength;
            for (var i = 0; i < m_CentipedeLength; i++)
            {
                var temp = Instantiate(m_CentipedePrefab, new Vector2(m_CentipedeSpacing * i, 3.65f), Quaternion.identity);
                if (i == 0)
                {
                    temp.GetComponent<SpriteRenderer>().sprite = m_FirstCentipedeSprite;
                }
                else if (i == m_CentipedeLength - 1)
                {
                    temp.GetComponent<SpriteRenderer>().sprite = m_LastCentipedeSprite;
                }
                else
                {
                    var x = Random.Range(0, 2);
                    if (x == 1)
                    {
                        temp.GetComponent<SpriteRenderer>().sprite = m_MiddleCentipedeSprite1;
                    }
                    else
                    {
                        temp.GetComponent<SpriteRenderer>().sprite = m_MiddleCentipedeSprite2;
                    }
                }
            }
            // Spawn Obstacles
            m_FinishedObstacles = 0;
            
        }
    }

    // Spawns Boss
    void StartBossLevel()
    {
        Instantiate(m_Boss1Prefab);
    }

    // Increases difficulty and starts centipede level cycle
    public void BossDead()
    {
        m_Boss = false;
        m_Centipede = true;
        EnemyHealth.Health += 1.0f;
        HorizontalMovementEnemy.Speed += 1.0f;
        m_LevelNum++;
        m_LevelUI.text = "Level " + m_LevelNum;
        StartCoroutine(NextLevelAfterBoss());
    }

    // Starts centipede level
    IEnumerator NextLevelAfterBoss()
    {
        yield return new WaitForSeconds(m_NextLevelDelay);
        m_SpriteRenderer.enabled = true;
        StartCentipedeLevel();
    }

    // Starts centipede level with enemies and obstacles
    void StartCentipedeLevel()
    {
        for (var j = 0; j < m_CentipedeLength; j++)
        {
            var temp = Instantiate(m_CentipedePrefab, new Vector2(m_CentipedeSpacing * j, 3.65f), Quaternion.identity);
            if (j == 0)
            {
                temp.GetComponent<SpriteRenderer>().sprite = m_FirstCentipedeSprite;
            }
            else if (j == m_CentipedeLength - 1)
            {
                temp.GetComponent<SpriteRenderer>().sprite = m_LastCentipedeSprite;
            }
            else
            {
                var x = Random.Range(0, 2);
                if (x == 1)
                {
                    temp.GetComponent<SpriteRenderer>().sprite = m_MiddleCentipedeSprite1;
                }
                else
                {
                    temp.GetComponent<SpriteRenderer>().sprite = m_MiddleCentipedeSprite2;
                }
            }
        }

        InvokeRepeating("SpawnSpiders", m_SpiderDelay, m_SpiderSpawnRate);
        InvokeRepeating("SpawnCentipedes", 0, m_CentipedeSpawnRate);
        InvokeRepeating("SpawnFallingGuys", m_FallingGuyDelay, m_FallingGuySpawnRate);
        InvokeRepeating("SpawnPirates", m_PirateDelay, m_PirateSpawnRate);
    }

    // Spawns Obstacles
    void SpawnObstacles()
    {        
        randx = Random.Range(-8f, 8f);
        randy = Random.Range(-2.5f, 3.5f);

        Instantiate(m_Obstacle, new Vector2(randx, randy), Quaternion.identity);

        m_FinishedObstacles++;
    }
}
