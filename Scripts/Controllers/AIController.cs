using UnityEngine;
using System.Collections;

public class AIController : MovingParent {
	EasyAI AI;
	public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.
	//public int pointsPerFood = 10;              //Number of points to add to player food points when picking up a food object.
	//public int pointsPerSoda = 20;              //Number of points to add to player food points when picking up a soda object.
	//public int wallDamage = 1;                  //How much damage a player does to a wall when chopping it.
	
	//private Animator animator;                  //Used to store a reference to the Player's animator component.
	//private int food;                           //Used to store player food points total during level.
	
	private int index;							//used for knowing how many inputs we have
	
	//Start overrides the Start function of MovingObject
	protected override void Start() {
		base.Start();
		index = 0;
		//BoardManager.initGrid ();		//init the data board
		AI = new EasyAI();
		AI.setLevel (1);				//sets the level easy or medium
		//AI.setDifficulty(100);		//sets the difficulty
	}
	
	private void Update() {
		//If it's not the enemy's turn, exit the function.
		if (GameManager.playersTurn) return;
		
		enemyMoves = AI.smartMove();
		StartCoroutine(makeMoves());
		GameManager.playersTurn = true;
		
	}
	
}

