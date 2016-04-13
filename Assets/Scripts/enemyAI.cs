using UnityEngine;
using System.Collections;

public class enemyAI : MonoBehaviour {

	//Kinematics
	public float speed = 0.1f;			//The desired speed of the enemy.
	private Vector2 velocity;		//The velocity of the enemy.
	private int direction = 1;		//The direction of the enemy (1 = right, -1 = left)

	//Existence Stuff
	[HideInInspector]
	public bool exists = false;		//Keep track of the enemy's existence.
	private Vector3 viewPosBL;		//The Bottom Left corner of the enemy relative to the screen.
	private Vector3 viewPosTR;		//The Top Right corner of the enemy relative to the screen.

	//Collision stuff
	public LayerMask collisionMask;
	private BoxCollider2D box;
	private Vector2 boxSize;
	private Vector2 boxCenter;
	
	void Start () {
		//Get the box collider.
		box = GetComponent<BoxCollider2D> ();
		boxSize = box.bounds.size;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//Check if currently on screen.
		checkExistance();

		//Get the desired move of the enemy.
		if(exists) velocity = move (velocity, speed, direction);

		//Figure out where the center of the collider is.
		boxCenter = box.bounds.center;

		//Do raycasting.
		horizontalRaycasting();

		//Flip image when necessary.
		if(direction == -1) transform.localScale = new Vector3(1,transform.localScale.y,transform.localScale.z);
		else transform.localScale = new Vector3(-1,transform.localScale.y,transform.localScale.z);

		//Finally, move the object.
		transform.Translate(velocity);
		velocity = Vector2.zero;
	
	}

	Vector2 move(Vector2 velocity, float speed, int direction){
		velocity.x += speed * direction; 
		return velocity;
	}

	//----------------------------------------------------------------------------------------------------
	//HorizontalRaycasting does horizontal collision checking, and moves the player accordingly.
	//----------------------------------------------------------------------------------------------------
	void horizontalRaycasting(){
		//float direction = Mathf.Sign(velocity.x);				//Direction of the vertical movement.
		float margin = boxSize.y/20;							//Padding in the collider for raycasting.
		float rayLength = boxSize.x/2 + Mathf.Abs(velocity.x);	//Length of the rays.
		int rayNumber = 5;
		
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
				transform.Translate(direction * Vector2.right * hitDistance);	//Move object to contact.
				Debug.DrawRay(currentRay, new Vector2(direction * rayLength, 0), Color.red);
				direction *= -1;												//Switch direction.
				break;
			}
			else Debug.DrawRay(currentRay, new Vector2(direction * rayLength, 0), Color.green);
		}
	}

	//checkExistance updates the exists variable.
	void checkExistance(){
		//Get the position of the object, relative to the screen.
		viewPosBL = Camera.main.WorldToViewportPoint(transform.position - new Vector3(boxSize.x/2, boxSize.y/2,0));
		viewPosTR = Camera.main.WorldToViewportPoint(transform.position + new Vector3(boxSize.x/2, boxSize.y/2,0));
		
		//If the object is not visible in the screen...
		if(((viewPosTR.x <= -0.05) || (viewPosBL.x >= 1.05)) || 
		   ((viewPosTR.y <= 0) || (viewPosBL.y >= 1)))
			exists = false; 
		else 
			exists = true;
		
	}

}
