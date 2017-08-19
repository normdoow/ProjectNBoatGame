using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//The abstract keyword enables you to create classes and class members that are incomplete and must be implemented in a derived class.
public abstract class MovingParent : MonoBehaviour {

	public float shootTime = 1.2f;           //Time it will take object to move, in seconds.
	public float moveTime = 2f;
	//public float value = 0.2;
	public Transform playerTran;
	public Transform enemyTran;
	private CannonParent cannonScript;
	private CannonParent cannonScriptEnemy;
	private GameObject enemyObject;
//	public LayerMask blockingLayer;         //Layer on which collision will be checked.
	
	//moveing transfomr variables
	public float speed = 2F;
	public Button goButton;					//the button in the gui

	//variables for the players
	private SphereCollider sphereCollider;      //The BoxCollider component attached to this object.
	private Rigidbody rb;               //The Rigidbody component attached to this object.
	private float inverseMoveTime;          //Used to make movement more efficient.
	private string direction = "North";
	protected static string[] playerMoves;
	protected static string[] enemyMoves;

	protected bool bigBuns = false;
	Vector2 oldPos;
	float radius;

	//Protected, virtual functions can be overridden by inheriting classes.
	protected virtual void Start () {
		//Get a component reference to this object's BoxCollider2D
		sphereCollider = GetComponent <SphereCollider> ();
		//cannonScript = GetComponent<CannonParent> ();
		//cannonScript = gameObject.GetComponent <CannonParent>();
		//enemyObject = GameObject.FindWithTag ("Player");
		cannonScript = (CannonParent)GameObject.Find("Player").GetComponent(typeof(CannonParent));
		cannonScriptEnemy = (CannonParent)GameObject.Find("Enemy").GetComponent(typeof(CannonParent));
//		if (enemyObject != null)
//		{
//			//cannonScript = gameObject.GetComponent <CannonParent>();
//		}
//		if (cannonScript == null)
//		{
//			Debug.Log ("Cannot find 'GameController' script");
//		}
		//Get a component reference to this object's Rigidbody2D
		rb = GetComponent <Rigidbody> ();
		//playerTran = GetComponent<Transform> ();
		//enemyTran = GetComponent<Transform> ();


		//init both of the different player moves
		enemyMoves = new string[6];
		playerMoves = new string[6];
		for(int k = 0; k < 6; k++) {			//initialize the players default moves
			playerMoves[k] = "Stay";
		}
		//StartCoroutine(rotateTranLeft(0f, 0f, playerTran, "West"));
	}

	//use update to run the animations
	public virtual void Update() {


	}

	public void setGoButtonInteractable(bool isInter) {
		goButton.interactable = isInter;
	}
	
	//Move returns true if it is able to move and false if not. 
	//Move takes parameters for x direction, y direction and a RaycastHit2D to check collision.
//	protected bool Move (int xDir, int yDir, out RaycastHit hit) {
//		//Store start position to move from, based on objects current transform position.
//		Vector3 start = transform.position;
//		
//		// Calculate end position based on the direction parameters passed in when calling Move.
//		Vector3 end = start + new Vector3 (xDir, yDir, 0f);
//		
//		//Disable the boxCollider so that linecast doesn't hit this object's own collider.
//		sphereCollider.enabled = false;
//
//		Ray ray = new Ray(start, end);		//set the ray based on the start and end position
//
//		//Cast a line from start point to end point checking collision on blockingLayer.
////		hit = Physics.Linecast (start, end, blockingLayer);
//		if(Physics.Raycast(ray, out hit)) {
//
//		}
//		//Re-enable boxCollider after linecast
//		sphereCollider.enabled = true;
//		
//		//Check if anything was hit
////		if(hit.transform == null) {
////			//If nothing was hit, start SmoothMovement co-routine passing in the Vector end as destination
//			StartCoroutine (SmoothMovement (end));
////			
////			//Return true to say that Move was successful
//			return true;
////		}
//		
//		//If something was hit, return false, Move was unsuccesful.
//		return false;
//	}

	public IEnumerator makeMoves() {			//reutrns IEnumerator for the waiting

		for (int k = 0; k < playerMoves.Length; k++) {
			if(playerMoves[k]!= null && enemyMoves[k]!= null) {
				if (k % 2 != 0) {		//if even then it is a cannon shot
					if(playerMoves[k] == "Right") {
						cannonScript.shootCannon("Right", BoardManager.getDirection());
					} else if(playerMoves[k] == "Left") {
						cannonScript.shootCannon("Left", BoardManager.getDirection());
					}
					//do the enemies move
					if(enemyMoves[k] == "Right") {
						cannonScriptEnemy.shootCannon("Right", BoardManager.getDirectionEnemy());
					} else if(enemyMoves[k] == "Left") {
						cannonScriptEnemy.shootCannon("Left", BoardManager.getDirectionEnemy());
					}
					yield return new WaitForSeconds(shootTime);			//wait for the shoot time before continuing
				} else {				//otherwise it is a direction movement
					//go through and update the enemy board
					float oldPosXEnemy = BoardManager.getCurrentPosEnemy().x;
					float oldPosYEnemy = BoardManager.getCurrentPosEnemy().y;
					string enemyDirection = BoardManager.getDirectionEnemy();		//get old values for use
					BoardManager.updateBoardWithEnemyMove(enemyMoves[k]);
					//then update the player board
					oldPos = BoardManager.getCurrentPos();					//get old values for use
					float oldPosX = BoardManager.getCurrentPos().x;
					float oldPosY = BoardManager.getCurrentPos().y;
					string playerDirection = BoardManager.getDirection();
					BoardManager.updateBoardWithPlayerMove(playerMoves[k]);

					//do enemy move
					doTransformationOfMove(oldPosXEnemy, oldPosYEnemy, BoardManager.getCurrentPosEnemy().x, BoardManager.getCurrentPosEnemy().y, 
					                       enemyDirection, enemyTran, enemyMoves[k]);
					//do player move
					doTransformationOfMove(oldPosX, oldPosY, BoardManager.getCurrentPos().x, BoardManager.getCurrentPos().y, 
					                       					playerDirection, playerTran, playerMoves[k]);

					//makes the boat just jump to the right location
//					enemyTran.position = new Vector3(BoardManager.getCurrentPosEnemy().x, BoardManager.getCurrentPosEnemy().y, 0f);
//					rotateTowardsDirection(enemyDirection, false);		//rotates the boat
//
//					playerTran.position = new Vector3(BoardManager.getCurrentPos().x, BoardManager.getCurrentPos().y, 0f);
//					rotateTowardsDirection(playerDirection, true);

					yield return new WaitForSeconds(moveTime);			//wait for the move time
				}
			}

		}
		goButton.interactable = true;
		//setGoButtonInteractable(true);				//sets the go button so that it can be pushed again
	}

	private void doTransformationOfMove(float oldPosX, float oldPosY, float newX, float newY, 
	                                    string oldDirection, Transform tran, string move) {
		if(move == "Right") {
			StartCoroutine(rotateTranRight(oldPosX, oldPosY, newX, newY, tran, oldDirection));
			//rotateTowardsDirection(playerDirection, true);		//rotates the boat
		} else if(move == "Left") {
			StartCoroutine(rotateTranLeft(oldPosX, oldPosY, newX, newY, tran, oldDirection));
			//rotateTowardsDirection(playerDirection, true);		//rotates the boat
		} else {			//esle were going straight
			if(tran == playerTran) {		//if this is the player
				StartCoroutine(movePlayerToTran(BoardManager.getCurrentPos().x, BoardManager.getCurrentPos().y, tran));
			} else {				//otherwise this is the enemy
				StartCoroutine(movePlayerToTran(BoardManager.getCurrentPosEnemy().x, BoardManager.getCurrentPosEnemy().y, tran));
			}
		}
	}

	//rotates the boat towards the new direction
	//this only used in the old way of doing things
	private void rotateTowardsDirection(string oldDirection, bool isPlayer) {
		string newDirection;
		Transform tran;
		if (isPlayer) {
			newDirection = BoardManager.getDirection ();		//get the new direction
			tran = playerTran;
		} else {
			newDirection = BoardManager.getDirectionEnemy ();
			tran = enemyTran;
		}
		if (newDirection != oldDirection) {				
			switch (oldDirection) {
			case "North":		
				if (newDirection == "East") {		//go right
					tran.Rotate (Vector3.up, 90);
				} else {			//if it is West
					tran.Rotate (Vector3.up, -90);
				}
				break;
			case "South":
				if (newDirection == "East") {
					tran.Rotate (Vector3.up, -90);
				} else {			//if it is West		//go right
					tran.Rotate (Vector3.up, 90);
				}
				break;
			case "East":
				if (newDirection == "North") {
					tran.Rotate (Vector3.up, -90);
				} else {			//if it is West		//go right
					tran.Rotate (Vector3.up, 90);
				}
				break;
			case "West":
				if (newDirection == "North") {			//go right
					tran.Rotate (Vector3.up, 90);
				} else {			//if it is West
					tran.Rotate (Vector3.up, -90);
				}
				break;
			}
		}
	}

	IEnumerator movePlayerToTran(float x, float y, Transform tran) {
		
		Vector3 newPos = new Vector3(x, y, 0f);
		float startTime = Time.time;
		float journeyLength = Vector3.Distance(tran.position, newPos);
		float distCovered = 0.0f;
		while(distCovered < journeyLength) {
			distCovered += 0.018f;//+= 0.01f; 		//can't go much lower
			float fracJourney = distCovered / journeyLength;
			tran.position = Vector3.Lerp(tran.position, newPos, fracJourney);
			yield return new WaitForSeconds(0.03f);
		}
		tran.position = new Vector3(x, y, 0f);
	}

	IEnumerator rotateTranRight(float startX, float startY, float newX, float newY, Transform tran, string direction) {
		float x = 0f;
		float y = 0f;
		float radius = 1f;
		float value = 0.03f;	//0.02f;
		while (x < 1f) {
			//speed value and rotate value
			float step = speed * Time.time;
			tran.Rotate (Vector3.up, 90 * value);//Time.deltaTime);
			x += value;

			//based on the direction decides what way the boat should turn
			if (direction == "North") {
				y = Mathf.Sqrt (radius * radius - (x - 1) * (x - 1));
				tran.position = Vector3.MoveTowards (tran.position, new Vector3 (x + startX, y + startY, 0f), step);
			} else if (direction == "South") {
				y = -Mathf.Sqrt (radius * radius - (x - 1) * (x - 1));
				tran.position = Vector3.MoveTowards (tran.position, new Vector3 (-x + startX, y + startY, 0f), step);
			} else if (direction == "East") {
				y = Mathf.Sqrt (radius * radius - (x - 1) * (x - 1));
				tran.position = Vector3.MoveTowards (tran.position, new Vector3 (y + startX, -x + startY, 0f), step);
			} else if (direction == "West") {
				y = -Mathf.Sqrt (radius * radius - (x - 1) * (x - 1));
				tran.position = Vector3.MoveTowards (tran.position, new Vector3 (y + startX, x + startY, 0f), step);
			}

			yield return new WaitForSeconds (0.03f);
		}

		tran.position = new Vector3(newX, newY, 0f);		//set the boat to the right place at the end
		rotateToDirection(tran);							//make sure boat is at the right rotation
	}

	IEnumerator rotateTranLeft(float startX, float startY, float newX, float newY, Transform tran, string direction) {
		float x = 0f;
		float y = 0f;
		float radius = 1f;
		float xPos = x;
		float value = 0.03f;
		while (x < 1f) {
			//set step value
			float step = speed * Time.time;
			tran.Rotate (Vector3.up, -90 * value);//Time.deltaTime);
			x += value;

			//based on the direction decides what way the boat should turn
			if (direction == "North") {
				y = Mathf.Sqrt (radius * radius - (x - 1) * (x - 1));
				tran.position = Vector3.MoveTowards (tran.position, new Vector3 (-x + startX, y + startY, 0f), step);
			} else if (direction == "South") {
				y = -Mathf.Sqrt (radius * radius - (x - 1) * (x - 1));
				tran.position = Vector3.MoveTowards (tran.position, new Vector3 (x + startX, y + startY, 0f), step);
			} else if (direction == "East") {
				y = Mathf.Sqrt (radius * radius - (x - 1) * (x - 1));
				tran.position = Vector3.MoveTowards (tran.position, new Vector3 (y + startX, x + startY, 0f), step);
			} else if (direction == "West") {
				y = -Mathf.Sqrt (radius * radius - (x - 1) * (x - 1));
				tran.position = Vector3.MoveTowards (tran.position, new Vector3 (y + startX, -x + startY, 0f), step);
			}

			yield return new WaitForSeconds (0.03f);
		}

		tran.position = new Vector3(newX, newY, 0f);
		rotateToDirection(tran);									//make sure boat is at the right rotation
	}

	void rotateToDirection(Transform tran) {
		string direction;
		Quaternion target = Quaternion.identity;
		if (tran == playerTran) {
			direction = BoardManager.getDirection();
		} else {
			direction = BoardManager.getDirectionEnemy();
		}
		if (direction == "North") {
			target = Quaternion.Euler(270f, 0f, 0f);			//to the north
		} else if (direction == "South") {
			target = Quaternion.Euler(-270f, -90f, 90f);		//to the south
		} else if (direction == "West") {
			target = Quaternion.Euler(360f, -90f, 90f);		//to the west
		} else if (direction == "East") {
			target = Quaternion.Euler(360f, 90f, -90f);		//to the east
		}
		tran.rotation = target;
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Boundary") {
			return;
		}
//		Instantiate (explosion, transform.position, transform.rotation);
//		if (other.tag == "Player") {		//if hits the player the player explodes as well
//			Instantiate (playerExplosion, other.transform.position, other.transform.rotation);
//			gameController.GameOver();
//		}
//		
//		gameController.AddScore (scoreValue);
//		Destroy(other.gameObject);
//		Destroy(gameObject);
	}
	
	//Co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
//	protected IEnumerator SmoothMovement (Vector3 end) {
//		print ("Doing smooth move");
//		//Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
//		//Square magnitude is used instead of magnitude because it's computationally cheaper.
//		float sqrRemainingDistance = (rb.position - end).sqrMagnitude;
//		
//		//While that distance is greater than a very small amount (Epsilon, almost zero):
//		while(sqrRemainingDistance > float.Epsilon) {
//			//Find a new position proportionally closer to the end, based on the moveTime
//			Vector3 newPostion = Vector3.MoveTowards(transform.position, end, inverseMoveTime * Time.deltaTime);
//			
//			//Call MovePosition on attached Rigidbody2D and move it to the calculated position.
//			rb.MovePosition (newPostion);
//			
//			//Recalculate the remaining distance after moving.
//			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
//			
//			//Return and loop until sqrRemainingDistance is close enough to zero to end the function
//			yield return null;
//		}
//	}
	
	//The virtual keyword means AttemptMove can be overridden by inheriting classes using the override keyword.
	//AttemptMove takes a generic parameter T to specify the type of component we expect our unit to interact with if blocked (Player for Enemies, Wall for Player).
	//protected virtual void AttemptMove <T> (int xDir, int yDir) where T : Component {
//	protected virtual void AttemptMove (int xDir, int yDir) {
//
//		//Hit will store whatever our linecast hits when Move is called.
//		RaycastHit hit;
//		
//		//Set canMove to true if Move was successful, false if failed.
//		bool canMove = Move (xDir, yDir, out hit);
//
//		if (!canMove) {
//			print ("We hit something!");
//		}
//		//Check if nothing was hit by linecast
////		if(hit.transform == null) {
////			//If nothing was hit, return and don't execute further code.
////			return;
////		}
////		
////		//Get a component reference to the component of type T attached to the object that was hit
////		T hitComponent = hit.transform.GetComponent <T> ();
////		
////		//If canMove is false and hitComponent is not equal to null, meaning MovingObject is blocked and has hit something it can interact with.
////		if (!canMove && hitComponent != null) {
////			//Call the OnCantMove function and pass it hitComponent as a parameter.
////			OnCantMove (hitComponent);
////		}
//	}

	private bool wontHitWall() {
		RaycastHit hit;
		Vector3 dir = Vector3.up;
		switch (direction) {									//choose what dir should be
		case "North":
			dir = Vector3.up;
			break;
		case "South":
			dir = Vector3.down;
			break;
		case "East":
			dir = Vector3.right;
			break;
		case "West":
			dir = Vector3.left;
			break;
		}
		Ray forwardRay = new Ray (transform.position, dir);
		Debug.DrawRay (transform.position, dir, Color.green, 3);			//test out the ray
		if (Physics.Raycast (forwardRay, out hit, 1)) {
			if(hit.collider.tag == "Wall") {
				print ("You ran into a wall");
				return false;
			}
		}
		return true;
	}
	
	
	//The abstract modifier indicates that the thing being modified has a missing or incomplete implementation.
	//OnCantMove will be overriden by functions in the inheriting classes.
	//protected abstract void OnCantMove <T> (T component) where T : Component;

}
