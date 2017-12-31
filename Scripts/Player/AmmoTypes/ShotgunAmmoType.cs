using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunAmmoType : MonoBehaviour {

    public int m_ShotCount;
    public GameObject m_DefaultAmmoTypePrefab;
    public float m_SpreadModifier = 5.0f;
    public float m_StartAngle = 45.0f;
    private Transform m_NewRotation;

	// Use this for initialization
	void Awake () {
        for (var i = 0; i < m_ShotCount; i++)
        {
            Instantiate(m_DefaultAmmoTypePrefab, transform.position, Quaternion.Euler(0, 0, -m_StartAngle + (i * m_SpreadModifier)));
        }
        Destroy(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
