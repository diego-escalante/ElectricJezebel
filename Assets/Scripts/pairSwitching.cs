using UnityEngine;
using System.Collections;

public class pairSwitching : MonoBehaviour {

	//Variables
	public GameObject door;			//The door this switch opens.
	public GameObject pairSwitch;	//The counterpart to this switch.
	public Sprite onSprite;			//The switch when it is active.
	public Sprite offSprite;		//The switch when it is not active.
	private SpriteRenderer sprRen;	//The sprite renderer.
	public bool isActive = false;	//One switch in the pair must be set to active at the start.

	//Existence Stuff
	[HideInInspector]
	public bool exists = false;		//Keep track of the switch's existence.
	private Vector3 viewPosBL;		//The Bottom Left corner of the switch relative to the screen.
	private Vector3 viewPosTR;		//The Top Right corner of the switch relative to the screen.
	private bool newlyExisting = false; 	//Used to bypass a little logic quirk.
	
	//Collisions and Player
	private bool overlap = false;	//Used to check if the player is next to it.
	private GameObject player;		//The player.
	private BoxCollider2D box;		//The collider.
	private Vector2 boxSize;		//The bounding box size of switch.
	private bool firsttime = true;

	//Sounds
	public AudioClip switchonSound;


	void Start () {
		sprRen = GetComponent<SpriteRenderer> ();
		player = GameObject.Find("player");
		
		box = GetComponent<BoxCollider2D> ();
		boxSize = box.bounds.size;
	}

	void Update () {
		//Check if switch is on screen.
		checkExistance();

		//Update sprites.
		if(isActive) sprRen.sprite = onSprite;
		else sprRen.sprite = offSprite;

		//Preform action.
		if (overlap && Input.GetKeyDown(KeyCode.DownArrow)) {
			toggleSwitch();
		}

		if (door != null){
			if (isActive && !door.GetComponent<doorStatus>().isOpen) {
				door.GetComponent<doorStatus>().openDoor();
			}
			else if(!isActive && door.GetComponent<doorStatus>().isOpen) {
				door.GetComponent<doorStatus>().closeDoor();
			}
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

	void toggleSwitch(){
		GameObject.FindWithTag("MainCamera").GetComponent<AudioSource>().PlayOneShot(switchonSound);
		if(isActive){
			isActive = false;
			if(pairSwitch.GetComponent<pairSwitching>().exists)
				pairSwitch.GetComponent<pairSwitching>().isActive = true;
		}
		else {
			isActive = true;
			if(pairSwitch.GetComponent<pairSwitching>().exists)
				pairSwitch.GetComponent<pairSwitching>().isActive = false;
		}
		return;
	}

	//checkExistance updates the exists variable.
	void checkExistance(){
		//Get the position of the object, relative to the screen.
		viewPosBL = Camera.main.WorldToViewportPoint(transform.position - new Vector3(boxSize.x/2, boxSize.y/2,0));
		viewPosTR = Camera.main.WorldToViewportPoint(transform.position + new Vector3(boxSize.x/2, boxSize.y/2,0));
		
		//If the object is not visible in the screen...
		if(((viewPosTR.x <= 0) || (viewPosBL.x >= 1)) || 
		   ((viewPosTR.y <= 0) || (viewPosBL.y >= 1)))
			exists = false;
		else if(!exists){
			newlyExisting = true;
		}

		if(newlyExisting){
			exists = true;
			if (firsttime) firsttime = false;
			else GameObject.FindWithTag("MainCamera").GetComponent<AudioSource>().PlayOneShot(switchonSound);
			if(pairSwitch.GetComponent<pairSwitching>().isActive) isActive = false;
			else isActive = true;
			newlyExisting = false;

		}
		
	}
}
