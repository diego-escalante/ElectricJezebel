using UnityEngine;
using System.Collections;

public class titleScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown (KeyCode.Return)) {
			GameObject g;
			g = GameObject.Find("text_ottersden");
			if(g) g.GetComponent<GUIText>().enabled = false;
			g = GameObject.Find("text_ottersden2");
			if(g) g.GetComponent<GUIText>().enabled = false;
			g = GameObject.Find("text_forLD");
			if(g) g.GetComponent<GUIText>().enabled = false;
			g = GameObject.Find("text_prompt");
			if(g) g.GetComponent<GUIText>().enabled = false;
			g = GameObject.Find("text_prompt2");
			if(g) g.GetComponent<GUIText>().enabled = false;
			g = GameObject.Find("goal");
			if(g) g.GetComponent<levelSwitcher>().startFadeOut();
		}
	
	}
}
