using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {

    public float m_RestartDelay = 2f;
    public static bool GameOverStart;

    // Use this for initialization
    void Start () {
        GameOverStart = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (GameController.TwoPlayer && PlayerController1.Player1Dead && PlayerController2.Player2Dead)
        {
            StartCoroutine(GameOverDelay());
        }
        else if (!GameController.TwoPlayer)
        {
            if (MainMenuButtons.characterChoice == 0 && PlayerController1.Player1Dead)
            {
                StartCoroutine(GameOverDelay());
            }
            else if (MainMenuButtons.characterChoice == 1 && PlayerController2.Player2Dead)
            {
                StartCoroutine(GameOverDelay());
            }
        }
	}

    IEnumerator GameOverDelay()
    {
        GameOverStart = true;
        yield return new WaitForSeconds(m_RestartDelay);
        SceneManager.LoadScene("GameOver");
    }
}
