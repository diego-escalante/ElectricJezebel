using UnityEngine;
using System.Collections;

public class doorStatus : MonoBehaviour {

	[HideInInspector]
	public bool isOpen = false;

	public void openDoor(){
		this.collider2D.enabled = false;
		this.renderer.enabled = false;
		isOpen = true;
	}

	public void closeDoor(){
		this.collider2D.enabled = true;
		this.renderer.enabled = true;
		isOpen = false;
	}
}
