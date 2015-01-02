using UnityEngine;
using System.Collections;

public class stickyText : MonoBehaviour {

	private Vector3 viewPos;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		viewPos = Camera.main.WorldToScreenPoint (transform.parent.position);
		guiText.pixelOffset = new Vector2 (viewPos.x, viewPos.y);
	}
}
