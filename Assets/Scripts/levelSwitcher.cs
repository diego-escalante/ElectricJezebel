using UnityEngine;
using System.Collections;

public class levelSwitcher : MonoBehaviour {

	public float fadeSpeed = 1.5f;		//Speed that the screen fades between black and clear.
	private bool sceneStarting = true; 	//Whether or not the screen is still fading in.
	[HideInInspector]
	public bool sceneEnding = false;	//You get what I mean.
	private bool overlap = false;		//Used to check if the player is at the goal.
	private GUITexture fader;			//Used to fade in and out scenes.

	[HideInInspector]
	public bool restart = false;		//Used by other things to restart the scene when dead.

	void Awake(){
		//Find the fader texture.
		fader = GameObject.Find ("fader").guiTexture;
		//Set the guiTexture to fill the entire screen.
		fader.transform.position = Vector3.zero;
		fader.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
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
		//Fade in.
		if(sceneStarting) startScene();

		//If player is at goal and presses the down arrow, we want to switch levels.
		if (overlap && Input.GetKeyDown(KeyCode.DownArrow)) {
			sceneEnding = true;
		}

		//Fade out.
		if(sceneEnding) endScene();

		//Restart scene if player presses R.
		if(Input.GetKeyDown(KeyCode.R)) {
			restart = true;
			sceneEnding = true;
		}
	}

	void startScene(){
		fadeToClear();

		//If gui is almost clear, we are done fading in.
		if(fader.color.a <= 0.05f){
			fader.color = Color.clear;
			fader.enabled = false;
			sceneStarting = false;
		}

	}

	void endScene(){

		fader.enabled = true;
		fadeToBlack();

		//If gui is almost black, we are done fading out, switch scenes.
		if(fader.color.a >= 0.95f){
			int currentLevel = Application.loadedLevel;
			int levelAmount = Application.levelCount;
			if(!restart){
				if (currentLevel + 1 < levelAmount) Application.LoadLevel(currentLevel + 1);
				else Application.LoadLevel(0);
			}
			else Application.LoadLevel(currentLevel);
		}
	}

	void fadeToClear ()
	{
		// Lerp the colour of the texture between itself and transparent.
		fader.color = Color.Lerp(fader.color, Color.clear, fadeSpeed * Time.deltaTime);
	}
	
	
	void fadeToBlack ()
	{
		// Lerp the colour of the texture between itself and black.
		fader.color = Color.Lerp(fader.color, Color.black, fadeSpeed * Time.deltaTime);
	}
}
