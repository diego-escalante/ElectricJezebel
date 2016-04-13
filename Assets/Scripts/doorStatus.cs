using UnityEngine;
using System.Collections;

public class doorStatus : MonoBehaviour {

	[HideInInspector]
	public bool isOpen = false;

	public void openDoor(){
		this.GetComponent<Collider2D>().enabled = false;
		this.GetComponent<Renderer>().enabled = false;
		isOpen = true;
	}

	public void closeDoor(){
		this.GetComponent<Collider2D>().enabled = true;
		this.GetComponent<Renderer>().enabled = true;
		isOpen = false;
	}
}
