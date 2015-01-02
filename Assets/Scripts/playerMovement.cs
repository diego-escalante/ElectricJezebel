using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Need a Box Collider to run script.
[RequireComponent(typeof(BoxCollider2D))]

public class playerMovement : MonoBehaviour {
	//----------------------------------------------------------------------------------------------------
	//Variables
	//----------------------------------------------------------------------------------------------------
	//Kinematics
	private Vector2 velocity;
	public Vector2 maxSpeed = new Vector2(1f,0.1f);
	public Vector2 acceleration = new Vector2(0.1f, -0.01f);
	public float jumpStrength = 0.2f;
	private bool grounded = false;
	private bool jumpPressed = false;

	//Collision stuff
	public LayerMask collisionMask;
	private BoxCollider2D box;
	private Vector2 boxSize;
	private Vector2 boxCenter;

	//Screen Pushing
	private Vector3 viewPosBL;
	private Vector3 viewPosTR;
	private Vector3 moveToContactX;
	private Vector3 moveToContactY;
	public Vector2 screenPushMargin1 = new Vector2(38,38);	//Not sure why 38, 38 is the best fit. I though 64,64 was going to be...														//Minor bug, can't take up any more time. NEEEXT!
	private Vector2 screenPushMargin;
	public float screenPushSlowFactor = 0.5f;

	//Animation
	private Animator animator;

	//Sound Stuff
	public AudioClip jumpSound;
	public AudioClip landSound;
	private bool continuousGrounded = true;


	//----------------------------------------------------------------------------------------------------
	//Start is called only once at the beginning.
	//----------------------------------------------------------------------------------------------------
	void Start () {
		//Get the box collider.
		box = GetComponent<BoxCollider2D> ();
		boxSize = box.bounds.size;

		//Get the animator for easy access.
		animator = GetComponent<Animator>();

		//Conver the pixel value of screenPushMargin to viewport.
		screenPushMargin = Camera.main.ScreenToViewportPoint (screenPushMargin1);
	}
	//----------------------------------------------------------------------------------------------------
	//Update is called once per frame.
	//----------------------------------------------------------------------------------------------------
	void Update () {

		//Record the jump key if it is pressed.
		if(Input.GetKey(KeyCode.Space)) jumpPressed = true;

		//If the player is outside(ish) the screen, move the screen to the appropriate place.
		correctScreen();

		//Get the desired move of the player.
		velocity = move(velocity);

		//Figure out where the center of the collider is.
		boxCenter = box.bounds.center;

		//Do raycasting.
		if(velocity.x != 0) horizontalRaycasting();
		verticalRaycasting();

		//Handle visuals: Mainly animation.
		if(velocity.x != 0) {
			animator.SetBool("running", true);
			//Flip the character to the correct direction.
			if(velocity.x > 0) transform.localScale = new Vector3(1,transform.localScale.y,transform.localScale.z);
			else transform.localScale = new Vector3(-1,transform.localScale.y,transform.localScale.z);
		}
		else animator.SetBool("running",false);

		if(velocity.y < 0 && !animator.GetCurrentAnimatorStateInfo(0).IsName("Fall")) animator.SetTrigger("falling");

		//Finally, move the object.
		transform.Translate(velocity);

		//Push the screen when necessary.
		pushScreen();
	}

	void flip() {
		transform.localScale = new Vector3(-transform.localScale.x,transform.localScale.y,transform.localScale.z);
		}
	//----------------------------------------------------------------------------------------------------
	//Move calculates the desired velocity of the player, before collisions are checked.
	//----------------------------------------------------------------------------------------------------
	Vector2 move(Vector2 velocity){
		//Handle horizontal input.
		velocity.x = getSpeedXFromInput(velocity.x, maxSpeed.x, acceleration.x, "Horizontal");

		//Handle vertical input.
		if(jumpPressed){
			jumpPressed = false;
			if(grounded){
				if(viewPosTR.y > 0.8) animator.SetTrigger("pushingup");
				else animator.SetTrigger("jumping");
				velocity.y = jumpStrength;
				AudioSource.PlayClipAtPoint(jumpSound, transform.position);
			}
		}

		//Apply Gravity.
		velocity.y = Mathf.Max(velocity.y + acceleration.y, -maxSpeed.y);

		return velocity;
	}

	//----------------------------------------------------------------------------------------------------
	//correctScreen makes sure that if the character is suddently outside the screen, correct
	//this by moving the screen to the appropriate location.
	//----------------------------------------------------------------------------------------------------
	void correctScreen(){

		//TODO: CORRECT THE COORECTSCREEN FUNCTION. Maybe modify translate.position, rather than using translate.

		//Calculate the position of player relative to the screen.
		viewPosBL = Camera.main.WorldToViewportPoint(transform.position - new Vector3(boxSize.x/2, boxSize.y/2,0));
		viewPosTR = Camera.main.WorldToViewportPoint(transform.position + new Vector3(boxSize.x/2, boxSize.y/2,0));
		//Calculate the width and height of screen in world coordinates.
		Vector2 worldCamDim = Camera.main.ScreenToWorldPoint (new Vector2 (Screen.width, Screen.height));

		float moveAmount;	//Used to remember the distance the camera will need to move.

		if(viewPosTR.x > 1 - screenPushMargin.x){
			moveAmount = viewPosTR.x - 1f + screenPushMargin.x;
			Camera.main.transform.Translate(Vector2.right * (worldCamDim.x * moveAmount));
		}
		else if(viewPosBL.x < 0 + screenPushMargin.x){
			moveAmount = -viewPosBL.x + screenPushMargin.x;
			Camera.main.transform.Translate(-Vector2.right * (worldCamDim.x * moveAmount));
		}

		if(viewPosTR.y > 1 - screenPushMargin.y){
			moveAmount = viewPosTR.y - 1f + screenPushMargin.y;
			Camera.main.transform.Translate(Vector2.up * (worldCamDim.y * moveAmount));
		}
		else if(viewPosBL.y < 0 + screenPushMargin.y){
			moveAmount = -viewPosBL.y + screenPushMargin.y;
			Camera.main.transform.Translate(-Vector2.up * (worldCamDim.y * moveAmount));
		}

	}

	//----------------------------------------------------------------------------------------------------
	//PushCamera moves the camera with the player if the player is 10% from the edge of the screen.
	//Decreases velocity by half when doing this.
	//----------------------------------------------------------------------------------------------------
	void pushScreen(){
		//Disclaimer: There might be a minor bug in this. BUT YOLO.

		bool didPush = false;
		//bool didPushUp = false;
		bool didPushDown = false;

		//Calculate the position of the player (from the Bottom Left and Top Right corners) relative to the position on screen.
		viewPosBL = Camera.main.WorldToViewportPoint(transform.position - new Vector3(boxSize.x/2, boxSize.y/2,0));
		viewPosTR = Camera.main.WorldToViewportPoint(transform.position + new Vector3(boxSize.x/2, boxSize.y/2,0));

		//If the player is 10% away from the screen, move the screen with the player.
		if((viewPosBL.x < screenPushMargin.x) || (viewPosTR.x > 1 - screenPushMargin.x)) {
			didPush = true;
			Camera.main.transform.Translate(new Vector2(velocity.x, 0));
			Camera.main.transform.Translate(moveToContactX);	//Take into account minor translates that happen during collision.
		}
		if((viewPosBL.y < screenPushMargin.y) || (viewPosTR.y > 1 - screenPushMargin.y)) {
			if(viewPosBL.y < screenPushMargin.y) didPushDown = true;
			//else didPushDown = true;
			Camera.main.transform.Translate(new Vector2(0, velocity.y));
			Camera.main.transform.Translate(moveToContactY);	//Take into account minor translates that happen during collision.
		}
		//Slow down player when pushing.
		if((viewPosBL.x < screenPushMargin.x) || (viewPosTR.x > 1 - screenPushMargin.x)) velocity.x *= (1-screenPushSlowFactor);
		if((viewPosBL.y < screenPushMargin.y) /*|| (viewPosTR.y > 1 - screenPushMargin.y)*/) velocity.y *= (1-screenPushSlowFactor);
		if(viewPosTR.y > 1 - screenPushMargin.y) velocity.y *= (1-screenPushSlowFactor/4);

		animator.SetBool ("pushing", didPush);
		animator.SetBool ("pushingdown", didPushDown);
	}

	//----------------------------------------------------------------------------------------------------
	//HorizontalRaycasting does horizontal collision checking, and moves the player accordingly.
	//----------------------------------------------------------------------------------------------------
	void horizontalRaycasting(){
		float direction = Mathf.Sign(velocity.x);				//Direction of the vertical movement.
		float margin = boxSize.y/20;							//Padding in the collider for raycasting.
		float rayLength = boxSize.x/2 + Mathf.Abs(velocity.x);	//Length of the rays.
		int rayNumber = 5;
		moveToContactX = Vector3.zero;

		Vector2 rayStart = new Vector2(boxCenter.x, boxCenter.y - boxSize.y/2 + margin);	//Beginning of raycast lerp.
		Vector2 rayFinish = new Vector2(boxCenter.x, boxCenter.y + boxSize.y/2 - margin);	//Ending of raycast lerp.

		//Start raycasting.
		for (int i = 0; i < rayNumber; i++) {
			//Calculate t amount to interpolate with.
			float t = (float)i/(float)(rayNumber-1);
			//Set current ray's origin.
			Vector2 currentRay = Vector2.Lerp(rayStart, rayFinish, t);
			//Cast the ray.
			RaycastHit2D hit = Physics2D.Raycast (currentRay, Vector2.right * direction, rayLength, collisionMask);
			
			//If the ray hits...
			if(hit.collider != null){
				//Calculate the distance from ray to what it hit.
				float hitDistance = (Vector2.Distance (hit.point, currentRay) - boxSize.x/2);
				moveToContactX = direction * Vector2.right * hitDistance;
				transform.Translate(moveToContactX);							//Move object to contact.
				velocity.x = 0;													//Stop object.
				//Debug.DrawRay(currentRay, new Vector2(direction * rayLength, 0), Color.red);
				break;
			}
			//else Debug.DrawRay(currentRay, new Vector2(direction * rayLength, 0), Color.green);
		}
	}
	//----------------------------------------------------------------------------------------------------
	//VerticalRaycasting does vertical collision checking, and moves the player accordingly.
	//----------------------------------------------------------------------------------------------------
	void verticalRaycasting(){
		float direction = Mathf.Sign(velocity.y);				//Direction of the vertical movement.
		float margin = boxSize.x/20;							//Padding in the collider for raycasting.
		float rayLength = boxSize.y/2 + Mathf.Abs(velocity.y);	//Length of the rays.
		int rayNumber = 5;
		moveToContactY = Vector3.zero;

		Vector2 rayStart = new Vector2(boxCenter.x - boxSize.x/2 + margin, boxCenter.y);	//Beginning of raycast lerp.
		Vector2 rayFinish = new Vector2(boxCenter.x + boxSize.x/2 - margin, boxCenter.y);	//Ending of raycast lerp.
		
		//Assume there is no ground beneath our feet.
		grounded = false;
		//Start raycasting.
		for (int i = 0; i < rayNumber; i++) {
			//Calculate t amount to interpolate with.
			float t = (float)i/(float)(rayNumber-1);
			//Set current ray's origin.
			Vector2 currentRay = Vector2.Lerp(rayStart, rayFinish, t);
			//Cast the ray.
			RaycastHit2D hit = Physics2D.Raycast (currentRay, Vector2.up * direction, rayLength, collisionMask);
			
			//If the ray hits...
			if(hit.collider != null){
				//Calculate the distance from ray to what it hit.
				float hitDistance = (Vector2.Distance (hit.point, currentRay) - boxSize.y/2);
				moveToContactY = direction * Vector2.up * hitDistance;
				transform.Translate(moveToContactY);							//Move object to contact.
				velocity.y = 0;													//Stop object.
				if(direction == -1) {
					if(animator.GetCurrentAnimatorStateInfo(0).IsName("Fall")) animator.SetTrigger("landing");
					grounded = true;							//Remember we are grounded if we hit the floor.
					if (!continuousGrounded) AudioSource.PlayClipAtPoint(landSound, transform.position);
					continuousGrounded = true;
					}
				//Debug.DrawRay(currentRay, new Vector2(0, direction * rayLength), Color.red);
				break;
			}

			//else Debug.DrawRay(currentRay, new Vector2(0,direction * rayLength), Color.green);
		}
		if (!grounded) continuousGrounded = false;
	}
	//----------------------------------------------------------------------------------------------------
	//GetSpeedXFromInput is a helper function that calculates the desired horizontal speed based on input.
	//----------------------------------------------------------------------------------------------------
	float getSpeedXFromInput (float currentVelocity, float maxSpeed, float acceleration, string axis){
		//Get the axis.
		float horizontalAxis = Input.GetAxisRaw(axis);
		
		//If we are receiving an input...
		if(horizontalAxis != 0){
			//accelerate until we reach our max speed.
			currentVelocity = Mathf.Clamp(currentVelocity + acceleration * horizontalAxis, -maxSpeed, maxSpeed);
		}
		//Otherwise, if we are moving...
		else if(currentVelocity != 0) {
			//decelerate till we stop.
			if(currentVelocity > 0)
				currentVelocity = Mathf.Max(0,currentVelocity - acceleration);
			else
				currentVelocity = Mathf.Min(0,currentVelocity + acceleration);
		}
		
		return currentVelocity;
	}
}
