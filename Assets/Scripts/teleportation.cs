using UnityEngine;
using System.Collections;

public class teleportation : MonoBehaviour {

	public int networkId = 0;		//This ID indicates teleporters that are linked.
	public Transform glare;

	private float margin;

	//Existence Stuff
	[HideInInspector]
	public bool exists = false;		//Keep track of the teleport's existence.
	private Vector3 viewPosBL;		//The Bottom Left corner of the teleporter relative to the screen.
	private Vector3 viewPosTR;		//The Top Right corner of the teleporter relative to the screen.

	//Collisions and Player
	private bool overlap = false;	//Used to check if the player is next to it.
	private GameObject player;		//The player.
	private BoxCollider2D box;		//The collider.
	private Vector2 boxSize;		//The bounding box size of teleporter.

	//Sound
	public AudioClip teleportSound;
	public AudioClip nopeSound;


	void Start () {
		player = GameObject.Find("player");
		if(player) margin = player.GetComponent<BoxCollider2D>().bounds.size.y/4;

		box = GetComponent<BoxCollider2D> ();
		boxSize = box.bounds.size;
	}
	
	void Update () {
		//Check if teleporter is on screen.
		checkExistance();
		
		//Preform action.
		if (overlap && Input.GetKeyDown(KeyCode.DownArrow)) {
			teleport();
		}	
	}

	//Use OnTriggerEnter and Exit to check if the player is there.	
	void OnTriggerEnter2D(Collider2D other){
		if(other.gameObject == player){
			overlap = true;
		}
	}
	void OnTriggerExit2D(Collider2D other){
		if(other.gameObject == player){
			overlap = false;
		}
	}

	//Teleport figures out everything about teleporting. It finds the suitable conterpart
	//to this teleport and sends the player there. If may not a suitable one.
	void teleport(){
		//Get all the teleporters.
		GameObject[] allTeleporters = GameObject.FindGameObjectsWithTag ("teleporter");

		//Find the ones that currently exist, also count them up.
		GameObject[] existingTeleporters = new GameObject[15];
		int existingCount = 0;
		for(int i=0;i < allTeleporters.Length; i++) {
			if (allTeleporters[i].GetComponent<teleportation>().exists && 
			    allTeleporters[i].GetComponent<teleportation>().networkId == networkId) {
				existingTeleporters[existingCount] = allTeleporters[i];
				existingCount++;
				}
		}

		//Quit early if there are not exactly 2 teleporters available.
		if(existingCount != 2) {
			GameObject.FindWithTag("MainCamera").GetComponent<AudioSource>().PlayOneShot(nopeSound);
			return;
		}

		//Teleport the player.
		GameObject.FindWithTag("MainCamera").GetComponent<AudioSource>().PlayOneShot(teleportSound);
		Instantiate(glare, player.transform.position, Quaternion.identity);
		if (this.gameObject != existingTeleporters[0])
			player.transform.position = existingTeleporters[0].transform.position;
		else
			player.transform.position = existingTeleporters[1].transform.position;
		Instantiate(glare, player.transform.position + new Vector3(0, margin, 0), Quaternion.identity);

	}

	//checkExistance updates the exists variable.
	void checkExistance(){
		//Get the position of the object, relative to the screen.
		viewPosBL = Camera.main.WorldToViewportPoint(transform.position - new Vector3(boxSize.x/2, boxSize.y/2,0));
		viewPosTR = Camera.main.WorldToViewportPoint(transform.position + new Vector3(boxSize.x/2, 0, 0));

		//If the object is not visible in the screen...
		if(((viewPosTR.x <= 0) || (viewPosBL.x >= 1)) || 
		   ((viewPosTR.y <= 0) || (viewPosBL.y >= 1)))
			exists = false; 
		else 
			exists = true;
	}
}
