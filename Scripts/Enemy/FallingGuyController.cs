using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingGuyController : MonoBehaviour {

    public float m_ObstacleSpawnRate = 0.5f;
    public GameObject m_ObstaclePrefab;

	// Use this for initialization
	void Start () {
        InvokeRepeating("SpawnObstacles", 0, m_ObstacleSpawnRate);
	}

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "BottomBoundary")
        {
            Destroy(gameObject);
        }
    }

    void SpawnObstacles()
    {
        Instantiate(m_ObstaclePrefab, transform.position, Quaternion.identity);
    }
}
