using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Score : MonoBehaviour {

    public static int score;
    public string scoreString;
    public Text scoreText;

	// Use this for initialization
	void Start () {
        scoreText = GameObject.Find("Score").GetComponent<Text>();
		Scene scene = SceneManager.GetActiveScene(); 
		if (scene.name != "GameOver") {
			score = 0;
		}
        scoreString = score.ToString();
	}
	
	// Update is called once per frame
	void Update () {
        scoreText = GameObject.Find("Score").GetComponent<Text>();
        scoreString = score.ToString();
        //Debug.Log(Score.score);
        scoreText.text = ("Score: " + scoreString);
    }
}
