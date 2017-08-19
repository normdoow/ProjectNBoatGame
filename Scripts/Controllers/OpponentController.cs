using UnityEngine;
using System.Collections;

public class OpponentController : MovingParent, MPOpponentListener {

	// Use this for initialization
	void Start () {
		base.Start();				//have to call the MovingParent Start
		MultiplayerController.Instance.opponentListener = this;		//tell multiplayerController that I am the listener
	}

	public void UpdateReceived(string participantId, char[] opponentMoves) {

		//set the enemy Moves from the data recieved
		for (int k = 0; k < opponentMoves.Length; k++) {
			if(opponentMoves[k] == 'S'){
				enemyMoves[k] = "Stay";
			} else if(opponentMoves[k] == 'U'){
				enemyMoves[k] = "Up";
			} else if(opponentMoves[k] == 'R'){
				enemyMoves[k] = "Right";
			} else if(opponentMoves[k] == 'L'){
				enemyMoves[k] = "Left";
			}
		}

		GameManager.playersTurn = true;
		GameManager.singleton.setEnemySentInMoves(true);

	}

}
