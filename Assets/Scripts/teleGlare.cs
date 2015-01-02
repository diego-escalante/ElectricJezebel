using UnityEngine;
using System.Collections;

public class teleGlare : MonoBehaviour {

	private float fadeSpeed = 2f;
	private SpriteRenderer myrenderer;

	// Use this for initialization
	void Start () {
		myrenderer = GetComponent<SpriteRenderer> ();
	
	}
	
	// Update is called once per frame
	void Update () {
		if(myrenderer.color.a < 0.05) Destroy(this.gameObject);
		myrenderer.color = Color.Lerp(myrenderer.color, Color.clear, fadeSpeed * Time.deltaTime);
	}
}
