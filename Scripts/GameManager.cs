using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GooglePlayGames.BasicApi.Multiplayer;		//included for multiplayer api
using System.Collections.Generic;       //Allows us to use Lists. 

public class GameManager : MonoBehaviour, MPUpdateListener {
	
	public static GameManager singleton;              //Static instance of GameManager which allows it to be accessed by any other script.
	public GameObject enemy;							//used to acces the enemy
	public GameObject player;
	private BoardManager boardScript;                       //Store a reference to our BoardManager which will set up the level.
	private PickStartLocations pickLocationsScript;
	//private int level = 3;                                  //Current level number, expressed in game as "Day 1".
	[HideInInspector] public static bool playersTurn = true;

	//public variables
	public Text gameEndingText;
	public Text whichPlayerAreYou;
	private bool playerWon;								//used to keep track of who won
	private bool enemyWon;
	private Vector2 touchOrigin = -Vector2.one; //Used to store location of screen touch origin for mobile controls.

	//multiplayer variables
	private bool isMultiplayerGame = false;				//will be set to true if it is a multiplayer game
	private bool enemySentInMoves = false;
	private bool playerSentInMoves = false;
	private string _myParticipantId;

	//used for updating the multiplayer game
	private byte _protocolVersion = 0;				//this is the version of multiplayer game that we are using
	// Byte + Byte + 2 floats for position + 2 floats for velcocity + 1 float for rotZ
	private int _updateMessageLength = 7;
	private List<byte> _updateMessage;

	//Awake is always called before any Start functions
	void Awake() {
		if (!singleton) {			//create the singleton
			singleton = this;
			//DontDestroyOnLoad (gameObject);
			Debug.Log("Created GameManager Object");

			Application.targetFrameRate = 30;		//set the frame rate that we want
			QualitySettings.vSyncCount = 0;			//something special that needs to be set for targetframerate to work
		} else {
			Destroy (gameObject);
			Debug.Log("Destroyed the GameManager Object");
		}

		//Sets this to not be destroyed when reloading scene
		//DontDestroyOnLoad(gameObject);

	}

	void Start() {
		//Get a component reference to the attached BoardManager script
		boardScript = GetComponent<BoardManager>();
		//get the pickLocations script
		pickLocationsScript = GetComponent<PickStartLocations> ();
		
		//Call the InitGame function to initialize the first level 
		InitGame();
	}
	
	//Initializes the game for each level.
	void InitGame() {
		//Call the SetupScene function of the BoardManager script, pass it current level number.
		boardScript.SetupScene();
		//pickLocationsScript.pickLocations();			//pick the locations and add the boats to the board

		//set up the grid to hold the data
		//BoardManager.initGrid(pickLocationsScript.getStartPos(), pickLocationsScript.getStartPosEnemy());

		//init the player winnings
		playerWon = false;
		enemyWon = false;

		//retain if this was a multiplayer game or not
		RetainedUserPicks userPicksScript = RetainedUserPicks.Instance;
		isMultiplayerGame = userPicksScript.getIsMultiplayerGame();
		//isMultiplayerGame = true;
		if (isMultiplayerGame) {		//if it is a multiplayer game set the opponent component
			Debug.Log("This is a multiplayer game!");
			enemy.GetComponent<OpponentController>().enabled = true;
			SetupMultiplayerGame();
		} else {						//set the AI component
			Debug.Log("This is a singleplayer game!");
			enemy.GetComponent<AIController>().enabled = true;
			SetupSingleplayerGame();
		}
	}

	//setters
	public void setPlayerWon(bool didWin) {
		playerWon = didWin;
	}

	public void setEnemyWon(bool didWin) {
		enemyWon = didWin;
	}
	
	public void setPlayerSentInMoves(bool isSent) {
		playerSentInMoves = isSent;
	}

	public void setEnemySentInMoves(bool isSent) {
		enemySentInMoves = isSent;
	}

	void SetupSingleplayerGame() {
		//get random positions and then add the random positions to the two different boats
		Vector2[] positions = pickLocationsScript.setBothPositions();
		SetPlayerPositions(positions[0], positions[1]);
	}

	void SetupMultiplayerGame() {
		MultiplayerController.Instance.updateListener = this;							//tell multicontroller that I am the listener
		_myParticipantId = MultiplayerController.Instance.GetMyParticipantId();
		List<Participant> allPlayers = MultiplayerController.Instance.GetAllPlayers();
		if (string.Compare (_myParticipantId, allPlayers [0].ParticipantId) == 0) {		//you are the first player
			Vector2[] positions = pickLocationsScript.setBothPositions ();
			SetPlayerPositions(positions [0], positions [1]);
			//send the positions to the other player
			MultiplayerController.Instance.SendBeginningData (positions [0], positions [1]);

			whichPlayerAreYou.text = "Player 1";				//changes the text to show what player you are
			//set the color of the boats
			player.GetComponent<Renderer>().material.color = Color.green;
			enemy.GetComponent<Renderer>().material.color = Color.red;
		} else {												//you are the second player
			whichPlayerAreYou.text = "Player 2";
			//set the color of the boats
			player.GetComponent<Renderer>().material.color = Color.red;
			enemy.GetComponent<Renderer>().material.color = Color.green;
		}

	}

	public void SetPlayerPositions(Vector2 posOne, Vector2 posTwo) {
		pickLocationsScript.addPlayersToScene(posOne, posTwo);
		//set up the grid to hold the data
		BoardManager.initGrid(posOne, posTwo);
	}

	//Update is called every frame.
	void Update() {


		if (isMultiplayerGame && playerSentInMoves && enemySentInMoves) {		//they both sent in the moves so go ahead and run the game
			Debug.Log ("Start making moves");
			StartCoroutine(enemy.GetComponent<OpponentController>().makeMoves());				//make the moves with the enemy controller
			playerSentInMoves = false;				//set both the bools to false so we don't run it again
			enemySentInMoves = false;
		}

	}

	/*
	 * 
	 * For ending the game
	 * 
	 **/
	
	public void checkForWin() {
		//enabled = false;
		if(playerWon != null && enemyWon != null) {
			if (playerWon && enemyWon) {
				gameEndingText.text = "You Tied!";
			} else if (playerWon) {
				gameEndingText.text = "You Won!";
			} else if (enemyWon) {
				gameEndingText.text = "You Lost!";
			}
		}
	}
	
	//Goes Back to Menu
	void goToMainMenu() {
		Application.LoadLevel ("MainMenu");
	}
	
	//call this when we want to leave the game
	void LeaveMPGame() {
		MultiplayerController.Instance.LeaveGame();
	}

	//this is called to confirm that we left the game
	public void LeftRoomConfirmed() {
		Debug.Log("We Left the game and went to MainMenu");
		MultiplayerController.Instance.updateListener = null;
		goToMainMenu ();
			//changes the player to move back to the MainMenu scene
	}

	//called when a player left the room
	public void PlayerLeftRoom(string participantId) {
		Debug.Log("The Other Player Left");
		MultiplayerController.Instance.LeaveGame();			//we want to leave the game because the other player did
	}

	//checks for time outs and if there is one kicks them from the room
	void CheckForTimeOuts() {
//		foreach (string participantId in _opponentScripts.Keys) {
//
//			Debug.Log("Haven't heard from " + participantId + " in " + timeOutThreshold + 
//			          " seconds! They're outta here!");
//			PlayerLeftRoom(participantId);
//		}
	}
}
