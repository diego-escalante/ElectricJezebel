using UnityEngine;
using System.Collections;

public class timeSwitching : MonoBehaviour {
	
	//Variables
	public GameObject door;			//The door this switch opens.
	public GameObject door2;
	public float setTime = 10f;		//Amount of time given until switch resets.
	private float countdown;		//Current time left when active.
	private bool isActive = false;	//If the switch is currently active.

	//Existence Stuff
	[HideInInspector]
	public bool exists = false;		//Keep track of the switch's existence.
	private Vector3 viewPosBL;		//The Bottom Left corner of the switch relative to the screen.
	private Vector3 viewPosTR;		//The Top Right corner of the switch relative to the screen.
	public Sprite onSprite;			//The switch when it is active.
	public Sprite offSprite;		//The switch when it is not active.
	private SpriteRenderer sprRen;	//The sprite renderer.

	//Collisions and Player
	private bool overlap = false;	//Used to check if the player is next to it.
	private GameObject player;		//The player.
	private BoxCollider2D box;		//The collider.
	private Vector2 boxSize;		//The bounding box size of switch.

	//Sound stuff
	public AudioClip switchSound;
	public AudioClip beep1;
	public AudioClip beep2;
	public AudioClip beep3;
	private float marker;

	void Start () {
		player = GameObject.Find("player");
		sprRen = GetComponent<SpriteRenderer> ();
		
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
			AudioSource.PlayClipAtPoint(switchSound, transform.position);
			activateSwitch(setTime);
		}

		//Keep track of time if the switch is active and exists.
		if(isActive && exists) {
			countdown -= Time.deltaTime;
			if (countdown <= marker && marker >= 7) {
				AudioSource.PlayClipAtPoint(beep1, transform.position);
				marker--;
				}
			else if (countdown <= marker && marker >= 2) {
				AudioSource.PlayClipAtPoint(beep2, transform.position);
				marker -= 0.5f;
			}
			else if (countdown <= marker) {
				AudioSource.PlayClipAtPoint(beep3, transform.position);
				marker -= 0.25f;
			}
			if (countdown <= 0){
				door.GetComponent<doorStatus>().closeDoor();
				if(door2 != null) door2.GetComponent<doorStatus>().closeDoor();
				isActive = false;
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

	void activateSwitch(float time){
		isActive = true;
		door.GetComponent<doorStatus>().openDoor();
		if(door2 != null) door2.GetComponent<doorStatus>().openDoor();
		countdown = time;
		marker = time - 1;
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
		else 
			exists = true;
	}
}
