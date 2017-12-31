using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthTrackingP1 : MonoBehaviour {

    public float thisHealth;
    private SpriteRenderer m_Renderer;

	// Use this for initialization
	void Awake () {
        m_Renderer = GetComponent<SpriteRenderer>();
        if (!GameController.TwoPlayer)
        {
            if (MainMenuButtons.characterChoice == 1)
            {
                gameObject.SetActive(false);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (PlayerController1.HealthP1 < thisHealth)
        {
            m_Renderer.enabled = false;
        }
        if (PlayerController1.HealthP1 >= thisHealth)
        {
            m_Renderer.enabled = true;
        }
	}
}
