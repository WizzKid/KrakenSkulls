using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour {

	private GameObject characterSelect;
	private bool volumeOn;
	//private GameObject volumeOff;
	public static int characterChoice;

    public static bool loadingScene;

    private AudioSource clickAudio;

    // Use this for initialization
    void Awake () {
		Cursor.visible = false; 
		Cursor.lockState = CursorLockMode.Locked;
        volumeOn = true;
		//volumeOff = GameObject.Find ("Sound_off");

		characterSelect = GameObject.FindGameObjectWithTag("Characters");
        if (characterSelect)
        {
            characterSelect.SetActive(false);
        }

        clickAudio = GetComponent<AudioSource>();
        loadingScene = false;
    }

	void Update (){
		if (Input.GetButtonDown ("Quit")) {
            StartCoroutine(DelayLoadSceneOrQuit("Quit"));
            Debug.Log ("Quitting");
		}

		if (Input.GetButtonDown ("Player1")) {
			characterChoice = 0;
			//Debug.Log (characterChoice);
			GameController.TwoPlayer = false; // Switch with button for two player mode
			StartCoroutine(DelayLoadSceneOrQuit("HydraGame"));
		}

		if (Input.GetButtonDown ("Player2")) {
			characterChoice = 1;
			//Debug.Log (characterChoice);
			GameController.TwoPlayer = false; // Switch with button for two player mode
			StartCoroutine(DelayLoadSceneOrQuit("HydraGame"));
		}
	}

	public void TwoPlayerClick(){
		GameController.TwoPlayer = true;
        clickAudio.Play();
        StartCoroutine(DelayLoadSceneOrQuit("HydraGame"));
    }

    public void QuitClick(){
        clickAudio.Play();
        StartCoroutine(DelayLoadSceneOrQuit("Quit"));
		Debug.Log ("Quitting");
	}

	public void CharacterSelect(){
        clickAudio.Play();
        //GameObject.Find("Buttons").SetActive (false);
        print(characterSelect);
		characterSelect.SetActive (true);
	}

	public void MaleSelect() {
        clickAudio.Play();
        characterChoice = 0;
		//Debug.Log (characterChoice);
        GameController.TwoPlayer = false;
		StartCoroutine(DelayLoadSceneOrQuit("HydraGame"));
    }

    public void FemaleSelect() {
        clickAudio.Play();
        characterChoice = 1;
		//Debug.Log (characterChoice);
        GameController.TwoPlayer = false;
		StartCoroutine(DelayLoadSceneOrQuit("HydraGame"));
    }

    public void volumeClick() {
        clickAudio.Play();
        if (volumeOn == true) {
			AudioListener.volume = 0;
            volumeOn = false;
		} else {
			AudioListener.volume = 1;
            volumeOn = true;
		}
	}

	public void toMainMenu() {
        clickAudio.Play();
        StartCoroutine(DelayLoadSceneOrQuit("MainMenu"));
	}

    IEnumerator DelayLoadSceneOrQuit(string SceneName)
    {
        loadingScene = true;
        yield return new WaitForSeconds(0.5f);
        if (SceneName == "Quit")
        {
            Application.Quit();
        }
        SceneManager.LoadScene(SceneName);
    }
}