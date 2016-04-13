using UnityEngine;
using System.Collections;

public class spikeBehavior : MonoBehaviour {

	//Collisions and Player
	private GameObject player;		//The player.
	private bool caput = false;

	//Sound
	public AudioClip deathSound;

	void Start () {
		player = GameObject.Find("player");
	}

	//Use OnTriggerEnter and Exit to check if the player is there.	
	void OnTriggerEnter2D(Collider2D other){
		if(other.gameObject == player && !caput){
			caput = true;
			GameObject.FindWithTag("MainCamera").GetComponent<AudioSource>().PlayOneShot(deathSound);
			StartCoroutine(death());
		}
	}

	IEnumerator death() {
		//Disable the player.
		player.GetComponent<Renderer>().enabled = false;
		player.GetComponent<playerMovement>().enabled = false;
		player.GetComponent<BoxCollider2D>().enabled = false;
		//Wait 2 seconds.
		yield return new WaitForSeconds(2);
		//Restart the level.
		GameObject.Find("goal").GetComponent<levelSwitcher>().restart = true;
		GameObject.Find("goal").GetComponent<levelSwitcher>().startFadeOut();
	}
}
