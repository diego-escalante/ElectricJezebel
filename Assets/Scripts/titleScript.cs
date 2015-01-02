using UnityEngine;
using System.Collections;

public class titleScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown (KeyCode.Return)) {

			GameObject.Find ("text_ottersden").guiText.enabled = false;
			GameObject.Find ("text_ottersden2").guiText.enabled = false;
			GameObject.Find ("text_forLD").guiText.enabled = false;
			GameObject.Find ("text_prompt").guiText.enabled = false;
			GameObject.Find("goal").GetComponent<levelSwitcher>().sceneEnding = true;
		}
	
	}
}
