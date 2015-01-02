using UnityEngine;
using System.Collections;

public class titleScript1 : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown (KeyCode.Return)) {

			GameObject.Find ("text_prompt2").guiText.enabled = false;
			GameObject.Find ("text_prompt").guiText.enabled = false;
			GameObject.Find("goal").GetComponent<levelSwitcher>().sceneEnding = true;
		}
	
	}
}
