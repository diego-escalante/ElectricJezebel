using UnityEngine;
using System.Collections;

public class flashText : MonoBehaviour {

	private GUIText fader;			//Used to fade in and out scenes.
	public float fadeSpeed = 1.5f;
	private bool toClear = true;

	// Use this for initialization
	void Start () {
		fader = GetComponent<GUIText>();
	}
	
	// Update is called once per frame
	void Update () {

		if(fader.color.a >= 0.95f) toClear = true;
		else if (fader.color.a <= 0.5f) toClear = false;

		if (toClear) fadeToClear ();
		else fadeToWhite();
	
	}

	void fadeToClear ()
	{
		// Lerp the colour of the texture between itself and transparent.
		fader.color = Color.Lerp(fader.color, Color.clear, fadeSpeed * Time.deltaTime);
	}
	
	
	void fadeToWhite ()
	{
		// Lerp the colour of the texture between itself and black.
		fader.color = Color.Lerp(fader.color, Color.white, fadeSpeed * Time.deltaTime);
	}
}
