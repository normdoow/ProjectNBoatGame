using UnityEngine;
using System.Collections;

public class GoEvent : MovingParent {

	// Use this for initialization
//	void Start() {
//		base.Start ();
//		for(int k = 0; k < 6; k++) {
//			playerMoves[k] = "Stay";
//		}
//	}

	private string[] nextPlayerMoves;			//use nextPlayerMoves so that the player can set the next moves without changing their
												//playerMoves that they sent into go
	void Start() {
		base.Start();
		nextPlayerMoves = new string[6];
	}

	public void setDropDownMoves(int row, string move) {
		int indexForMove = 0;
		if (row == 0) {
			indexForMove = 0;
		} else if (row == 1) {
			indexForMove = 2;
		} else if (row == 2) {
			indexForMove = 4;
		}
		nextPlayerMoves[indexForMove] = move;
	}

	public void setToggleMoves(int row, string move) {
		int indexForMove = 0;
		if (row == 0) {
			indexForMove = 1;
		} else if (row == 1) {
			indexForMove = 3;
		} else if (row == 2) {
			indexForMove = 5;
		}
		nextPlayerMoves[indexForMove] = move;
	}

	public void goButtonClicked() {
		GameManager.playersTurn = false;
		setGoButtonInteractable(false);
		playerMoves = nextPlayerMoves;				//sets the player moves from what was set from the buttons before hand
		if(RetainedUserPicks.Instance.getIsMultiplayerGame()) {
			GameManager.singleton.setPlayerSentInMoves(true);
			DoMultiplayerUpdate();
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
}
