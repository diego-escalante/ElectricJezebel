using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class levelSwitcher : MonoBehaviour {

	public float fadeSpeed = 1.5f;		//Speed that the screen fades between black and clear.
	// private bool sceneStarting = true; 	//Whether or not the screen is still fading in.
	[HideInInspector]
	public bool sceneEnding = false;	//You get what I mean.
	private bool overlap = false;		//Used to check if the player is at the goal.
	private GUITexture fader;			//Used to fade in and out scenes.

	[HideInInspector]
	public bool restart = false;		//Used by other things to restart the scene when dead.

	void Awake(){
		//Find the fader texture.
		fader = GameObject.Find ("fader").GetComponent<GUITexture>();
		//Set the guiTexture to fill the entire screen.
		fader.transform.position = Vector3.zero;
		fader.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
		StartCoroutine(fade(true));
	}
	
	//Use OnTriggerEnter and OnTriggerExit to check if the player is at the goal.
	void OnTriggerEnter2D(Collider2D other){
		if(other.gameObject.name == "player"){
			overlap = true;
		}
	}
	void OnTriggerExit2D(Collider2D other){
		if(other.name == "player"){
			overlap = false;
		}
	}

	void Update () {

		//If player is at goal and presses the down arrow, we want to switch levels.
		if (overlap && Input.GetKeyDown(KeyCode.DownArrow)) {
			StartCoroutine(fade(false));
		}

		//Restart scene if player presses R.
		if(Input.GetKeyDown(KeyCode.R)) {
			restart = true;
			StartCoroutine(fade(false));
		}
	}

	void endScene(){
		int currentLevel = SceneManager.GetActiveScene().buildIndex;
		int levelAmount = SceneManager.sceneCountInBuildSettings;
		if(!restart){
			if (currentLevel + 1 < levelAmount) SceneManager.LoadScene(currentLevel + 1);
			else SceneManager.LoadScene(0);
		}
		else SceneManager.LoadScene(currentLevel);
	}

	public void startFadeOut() {
		StartCoroutine(fade(false));
	}

	IEnumerator fade(bool fin=true) {
		float timeTotal = 0.25f;
		float timeElapsed = 0f;
		fader.enabled = true;
		while(timeElapsed < timeTotal) {
			if(fin) fader.color = Color.Lerp(Color.black, Color.clear, timeElapsed/timeTotal);
			else fader.color = Color.Lerp(Color.clear, Color.black, timeElapsed/timeTotal);
			timeElapsed += Time.deltaTime;
			yield return null;
		}

		if(fin) {
			fader.color = Color.clear;
			fader.enabled = false;
		}
		else {
			fader.color = Color.black;
			endScene();
		}

	}
}
