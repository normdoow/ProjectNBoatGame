using UnityEngine;
using System.Collections;

public class TouchController : MovingParent {
	
	public GameObject testCube;
	public GameObject camera;
	public Transform[] arrowLocations;
	public Transform[] touchLocations;
	public GameObject leftArrow;
	public GameObject upArrow;
	public GameObject rightArrow;

	//private variables
	private string[] nextPlayerMoves;			//use nextPlayerMoves so that the player can set the next moves without changing their
												//playerMoves that they sent into go
	private int moveCount = 0;

	void Start() {
		base.Start();
		nextPlayerMoves = new string[6];

		StartCoroutine(placeArrows());
	}

	IEnumerator placeArrows() {
		yield return new WaitForSeconds(3f);
		if(GameManager.playersTurn) {
			leftArrow = Instantiate(leftArrow, arrowLocations[0].position, arrowLocations[0].rotation) as GameObject;
			upArrow = Instantiate(upArrow, arrowLocations[1].position, arrowLocations[1].rotation) as GameObject;
			rightArrow = Instantiate(rightArrow, arrowLocations[2].position, arrowLocations[2].rotation) as GameObject;
		}
	}

	public void touchRightArrow() {
		Debug.Log("Touched the Right Arrow!");
		if(moveCount < 6) {
			nextPlayerMoves[moveCount] = "Right";
			if(moveCount < 4) {
				setNextArrows();
			}
			moveCount += 2;
		}
	}

	public void touchLeftArrow() {
		Debug.Log("Touched the Left Arrow!");
		if (moveCount < 6) {
			nextPlayerMoves[moveCount] = "Left";
			//Instantiate(leftArrow, arrowLocations[0].position, arrowLocations[0].rotation);
			if(moveCount < 4) {
				setNextArrows();
			} else {
				deleteArrows();
			}
			moveCount += 2;
		}
	}

	public void touchUpArrow() {
		Debug.Log("Touched the Up Arrow!");
		if (moveCount < 6) {
			nextPlayerMoves[moveCount] = "Up";
			if(moveCount < 4) {
				setNextArrows();
			}
			moveCount += 2;
		}
	}

	public void touchX() {
		Debug.Log("Touched the X!");
		if (moveCount < 6) {
			nextPlayerMoves[moveCount] = "Stay";
			if(moveCount < 4) {
				setNextArrows();
			}
			moveCount += 2;
		}
	}

	private void setNextArrows() {
		//get the info from the arrow clicked and then delete the old arrows
		Arrow arrowScript = leftArrow.GetComponent<Arrow>();
		arrowLocations = arrowScript.getArrowLocations();
		touchLocations = arrowScript.getTouchLocations();
		deleteArrows();				//delete the arrows we had before

		leftArrow = Instantiate(leftArrow, arrowLocations[0].position, arrowLocations[0].rotation) as GameObject;
		upArrow = Instantiate(upArrow, arrowLocations[1].position, arrowLocations[1].rotation) as GameObject;
		rightArrow = Instantiate(rightArrow, arrowLocations[2].position, arrowLocations[2].rotation) as GameObject;
	}

	private void deleteArrows() {
		//Destroy(leftArrow);
		Destroy(upArrow);
		Destroy(rightArrow);
	}

	public void goButtonClicked() {
		if(moveCount > 5) {
			moveCount = 0;
			Debug.Log("The go button is hit!");
			GameManager.playersTurn = false;
			setGoButtonInteractable (false);
			playerMoves = nextPlayerMoves;				//sets the player moves from what was set from the buttons before hand
			if (RetainedUserPicks.Instance.getIsMultiplayerGame()) {			//if it is a multiplayer game
				GameManager.singleton.setPlayerSentInMoves(true);
				DoMultiplayerUpdate();
			}
			GameObject[] arrows = GameObject.FindGameObjectsWithTag ("LeftArrow");
			for(int k = 0; k < arrows.Length; k++) {
				Destroy(arrows[k]);
			}
			GameObject[] arrows2 = GameObject.FindGameObjectsWithTag("UpArrow");
			for(int k = 0; k < arrows2.Length; k++) {
				Destroy(arrows2[k]);
			}
			GameObject[] arrows3 = GameObject.FindGameObjectsWithTag("RightArrow");
			for(int k = 0; k < arrows3.Length; k++) {
				Destroy(arrows3[k]);
			}

		}
	}

	void DoMultiplayerUpdate() {
		char[] movesAsChar = new char[6];
		for (int k = 0; k < movesAsChar.Length; k++) {
			if(playerMoves[k] == "Stay") {
				movesAsChar[k] = 'S';
			} else if(playerMoves[k] == "Up") {
				movesAsChar[k] = 'U';
			} else if(playerMoves[k] == "Right") {
				movesAsChar[k] = 'R';
			} else if(playerMoves[k] == "Left") {
				movesAsChar[k] = 'L';
			}
		}
		MultiplayerController.Instance.SendMyUpdate(movesAsChar);
	}

	void Update() {

		//Check if we are running either in the Unity editor or in a standalone build.
//		#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
//
//			if (Input.GetButtonDown("Fire1")) {
//				 Vector3 realWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//				Debug.Log(realWorldPos);
//			}
//		#endif
		//Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
		#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8
//		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
//
//
//		} else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) {
//			Touch touch = Input.GetTouch(0);
//			checkTouch(touch.position);
//		} 
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) {
			//Debug.DrawRay(touch.position, contact.normal, Color.green, 2, false);
			Touch touch = Input.touches[0];
			Vector3 pos = touch.position;
			pos.z = -camera.transform.position.z;
			Vector3 realWorldPos = Camera.main.ScreenToWorldPoint(pos);
			//Debug.Log(realWorldPos);
//			int x = (int)Mathf.Round(realWorldPos.x);
//			int y = (int)Mathf.Round(realWorldPos.y);
			realWorldPos.x = Mathf.Round(realWorldPos.x);
			realWorldPos.y = Mathf.Round(realWorldPos.y);
			realWorldPos.z = 0f;
			//Instantiate(testCube, realWorldPos, Quaternion.identity);

			checkWhichArrow(realWorldPos);

//		} else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) {
//			Touch touch = Input.touches[0];
//			Vector3 pos = touch.position;
//			pos.z = -camera.transform.position.z;
//			Vector3 realWorldPos = Camera.main.ScreenToWorldPoint(pos);
//			Debug.Log(realWorldPos);
//			realWorldPos.z = 0f;
//			Instantiate(testCube, realWorldPos, Quaternion.identity);
		}
		#endif //End of mobile platform dependendent compilation section started above with #if

	}

	private void checkWhichArrow(Vector3 realWorldPos) {
		for(int k = 0; k < touchLocations.Length; k++) {
			if(Vector3.Distance(realWorldPos, touchLocations[k].position) < 0.5f) {		//check how far the distance is, if it is less than 0.5 then thats the arrow they touched
				switch(k) {
				case 0:
					touchLeftArrow();
					break;
				case 1:
					touchUpArrow();
					break;
				case 2:
					touchRightArrow();
					break;
				}
			}
			if(Vector3.Distance(realWorldPos, transform.position) < 0.5f) {				//if they hit the boat, so stay
				touchX();
			}
		}
	}

	private void checkTouch(Vector3 pos) {
		// Construct a ray from the current touch coordinates
		Ray ray = Camera.main.ScreenPointToRay(pos);
		RaycastHit hitInfo;
		//Create a particle if hit
		if (Physics.Raycast (ray, out hitInfo, -camera.transform.position.z)){		//if the ray collides
			Debug.Log (hitInfo.collider.gameObject);
			if (hitInfo.collider.gameObject.tag == "RightArrow") {
				touchRightArrow ();
			} else if (hitInfo.collider.gameObject.tag == "LeftArrow") {
				touchLeftArrow();
			} else if (hitInfo.collider.gameObject.tag == "UpArrow") {
				touchUpArrow();
			}
		}
	}

}
