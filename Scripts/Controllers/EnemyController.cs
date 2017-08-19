using UnityEngine;
using System.Collections;

public class EnemyController : MovingParent {

	public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.
	//public int pointsPerFood = 10;              //Number of points to add to player food points when picking up a food object.
	//public int pointsPerSoda = 20;              //Number of points to add to player food points when picking up a soda object.
	//public int wallDamage = 1;                  //How much damage a player does to a wall when chopping it.
	
	//private Animator animator;                  //Used to store a reference to the Player's animator component.
	//private int food;                           //Used to store player food points total during level.

	private int index;							//used for knowing how many inputs we have
	
	//Start overrides the Start function of MovingObject
	protected override void Start () {
		base.Start ();
		index = 0;
		//BoardManager.initGrid ();		//init the data board
	}
	
	void Update () {
		//If it's not the enemy's turn, exit the function.
		if (GameManager.playersTurn) return;

		if (Input.GetKeyUp (KeyCode.S)) {
			enemyMoves [index] = "Stay";
			index++;
		} else if (Input.GetKeyUp (KeyCode.W)) {
			enemyMoves[index] = "Up";
			index++;
		} else if (Input.GetKeyUp (KeyCode.D)) {
			enemyMoves[index] = "Right";
			index++;
		} else if (Input.GetKeyUp (KeyCode.A)) {
			enemyMoves[index] = "Left";
			index++;
		}
		if (index > 5) {		//if we have gotten all our input
			print ("play moves");
			StartCoroutine(makeMoves());
			//makeMoves ();
			index = 0;
			GameManager.playersTurn = true;
		}
		
	}

	//Restart reloads the scene when called.
	private void Restart () {
		//Load the last scene loaded, in this case Main, the only scene in the game.
		Application.LoadLevel (Application.loadedLevel);
	}
	

}
