using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SwipeController : MovingParent {

	//the public link to buttons and stuff
	public GameObject swipePanel;
	public Image[] buttImage;					//the three different images for the buttons
	public Sprite[] moveSprites;					//sprites that are the different possible moves in order: left, forward, right, stay
	public GameObject[] shotImages;
	public Text swipeText;

	private string[] nextPlayerMoves;			//use nextPlayerMoves so that the player can set the next moves without changing their
	//playerMoves that they sent into go

	//bools to check which buttons have been tapped
	bool firstButtIsTouched = false;
	bool secButtIsTouched = false;
	bool thirdButtIsTouched = false;
	bool isBoatMove = true;

	//swipe controls
	private Vector2 touchOrigin = -Vector2.one; //Used to store location of screen touch origin for mobile controls.

	// Use this for initialization
	void Start () {
		base.Start();
		nextPlayerMoves = new string[6];
		for (int k = 0; k < nextPlayerMoves.Length; k++) {
			nextPlayerMoves[k] = "Stay";
		}
	}

	//called when the first button is touched
	public void touchFirstMove() {
		if (!firstButtIsTouched) {					//only want to be able to touch this if 
			Debug.Log ("The first Butt touched");
			StartCoroutine(waitForMove(0));
		}
	}

	//called when the second button is touched
	public void touchSecondMove() {
		Debug.Log ("The second Butt touched");
		StartCoroutine(waitForMove(1));
	}

	//called when the third button is touched
	public void touchThirdMove() {
		Debug.Log("The third Butt touched");
		StartCoroutine(waitForMove(2));
	}

	//waits for a little bit before running the code to start the touch controls
	IEnumerator waitForMove(int moveNum) {
		yield return new WaitForSeconds(0.3f);
		swipePanel.SetActive(true);
		if (moveNum == 0) {
			firstButtIsTouched = true;
		} else if (moveNum == 1) {
			secButtIsTouched = true;
		} else {
			thirdButtIsTouched = true;
		}
	}

	//makes the go button interactible
//	IEnumerator waitForAnimation() {
//		yield return new WaitForSeconds(10f);
//		goButton.GetComponent<Button>().interactable = true;
//	}

	//called when the go button is touched
	public void touchGoButton() {
		Debug.Log ("The go Butt touched");
		GameManager.playersTurn = false;
		//setGoButtonInteractable(false);
		playerMoves = nextPlayerMoves;				//sets the player moves from what was set from the buttons before hand
		if(RetainedUserPicks.Instance.getIsMultiplayerGame()) {
			GameManager.singleton.setPlayerSentInMoves(true);
			DoMultiplayerUpdate();
		}
		//waitForAnimation();					//makes the go button interactible
	}

	//used for updating the opponent if in multiplayer
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

	//called every frame to check if there is a move or not
	void Update() {

		//go through the different possibilities of what move you are on
		if (firstButtIsTouched) {
			if(isBoatMove) {
				nextPlayerMoves[0] = checkTouchControl(nextPlayerMoves[0], 0, 0);		//set the next moves to what they swiped
			} else {
				nextPlayerMoves[1] = checkTouchControl(nextPlayerMoves[1], 0, 1);		//set the shot
			}
		} else if (secButtIsTouched) {
			if (isBoatMove) {
				nextPlayerMoves[2] = checkTouchControl(nextPlayerMoves[2], 1, 0);		//set the next moves to what they swiped
			} else {
				nextPlayerMoves[3] = checkTouchControl(nextPlayerMoves[3], 1, 1);		//set the shot
			}
		} else if (thirdButtIsTouched) {		//thirdButtIsTouched
			if (isBoatMove) {
				nextPlayerMoves[4] = checkTouchControl(nextPlayerMoves[4], 2, 0);		//set the next moves to what they swiped
			} else {
				nextPlayerMoves[5] = checkTouchControl(nextPlayerMoves[5], 2, 1);		//set the shot
			}
		}

	}

	string checkTouchControl(string prevMove, int buttNum, int whichMove) {
		//Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
		#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8
		
		int horizontal = 0;     //Used to store the horizontal move direction.
		int vertical = 0;       //Used to store the vertical move direction.

		//Check if Input has registered more than zero touches
		if (Input.touchCount > 0) {
			//Store the first touch detected.
			Touch myTouch = Input.touches[0];
			
			//Check if the phase of that touch equals Began
			if(myTouch.phase == TouchPhase.Began) {
				//If so, set touchOrigin to the position of that touch
				touchOrigin = myTouch.position;
			}
			//If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
			else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0) {
				//Set touchEnd to equal the position of this touch
				Vector2 touchEnd = myTouch.position;
				
				//Calculate the difference between the beginning and end of the touch on the x axis.
				float x = touchEnd.x - touchOrigin.x;
				//Calculate the difference between the beginning and end of the touch on the y axis.
				float y = touchEnd.y - touchOrigin.y;
				
				//Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
				touchOrigin.x = -1; 
				//Check if the difference along the x axis is greater than the difference along the y axis.
				if (Mathf.Abs(x) > Mathf.Abs(y)) {
					//If x is greater than zero, set horizontal to 1, otherwise set it to -1
					horizontal = x > 0 ? 1 : -1;
				} else {
					//If y is greater than zero, set horizontal to 1, otherwise set it to -1
					vertical = y > 0 ? 1 : -1;
				}
				if(whichMove == 0) {
					return setTurn(prevMove, buttNum, horizontal, vertical);
				} else {
					return setShot(prevMove, buttNum, horizontal, vertical);
				}
			}
			
		}
		#endif //End of mobile platform dependendent compilation section started above with #elif
		return prevMove;
	}

	//funciton for setting the turn
	string setTurn(string prevMove, int buttNum, int horizontal, int vertical) {

		Sprite spriteToReturn = moveSprites[3]; 
		string strToReturn = prevMove;
		if (horizontal > 0) {
			Debug.Log ("Swiped Right");
			strToReturn = "Right";
			spriteToReturn = moveSprites [2];			//set it to the right arrow sprite
		} else if (horizontal < 0) {		//horizontal less than 0
			Debug.Log ("Swiped Left");
			strToReturn = "Left";
			spriteToReturn = moveSprites [0];
		} else if (vertical > 0) {
			Debug.Log ("Swiped Up");
			strToReturn = "Up";
			spriteToReturn = moveSprites [1];
		} else {
			strToReturn = "Stay";
		}
		//swipePanel.SetActive(false);
		if(buttNum == 0) {
			//firstButtIsTouched = false;
			buttImage[0].sprite = spriteToReturn;
		} else if(buttNum == 1) {
			//secButtIsTouched = false;
			buttImage[1].sprite = spriteToReturn;
		} else {
			//thirdButtIsTouched = false;
			buttImage[2].sprite = spriteToReturn;
		}
		//swipePanel.SetActive(false);
		//set it up so it is time to make the cannon shots
		swipeText.text = "Cannon Shot";
		isBoatMove = false;			//makes it so now we try to figure out the shot
		return strToReturn;


		//return prevMove;
	}

	//sets the shot for the spcified button
	string setShot(string prevMove, int buttNum, int horizontal, int vertical) {

		string strToReturn = "Up";
		if(horizontal > 0) {
			Debug.Log("Swiped Right");
			strToReturn = "Right";
		} else if(horizontal < 0) {		//horizontal less than 0
			Debug.Log("Swiped Left");
			strToReturn = "Left";
		} else if(vertical > 0) {
			Debug.Log("Swiped Up");
			strToReturn = "Up";
		}
		if(buttNum == 0) {
			firstButtIsTouched = false;
			shotImages[0].SetActive(false);				//set them back to default
			shotImages[1].SetActive(false);
			if(strToReturn == "Left") {
				shotImages[0].SetActive(true);
				shotImages[1].SetActive(false);
			} else if (strToReturn == "Right") {
				shotImages[0].SetActive(false);
				shotImages[1].SetActive(true);
			}
		} else if(buttNum == 1) {
			secButtIsTouched = false;
			shotImages[2].SetActive(false);
			shotImages[3].SetActive(false);
			if(strToReturn == "Left") {
				shotImages[2].SetActive(true);
				shotImages[3].SetActive(false);
			} else if (strToReturn == "Right") {
				shotImages[2].SetActive(false);
				shotImages[3].SetActive(true);
			}
		} else {
			thirdButtIsTouched = false;
			shotImages[4].SetActive(false);					//set them back to default
			shotImages[5].SetActive(false);
			if(strToReturn == "Left") {
				shotImages[4].SetActive(true);
				shotImages[5].SetActive(false);
			} else if (strToReturn == "Right") {
				shotImages[4].SetActive(false);
				shotImages[5].SetActive(true);
			}
		}
		//set the needed things to finish up the move
		swipeText.text = "Boat Move";
		swipePanel.SetActive(false);
		isBoatMove = true;

		return strToReturn;
	}

}
