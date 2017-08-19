using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;		//imports needed to use Unity
using System.Collections.Generic;					//needed for lists

public class MultiplayerController : RealTimeMultiplayerListener {

	//instance of this class
	private static MultiplayerController _instance = null;

	//Our Listeners
	public MPOpponentListener opponentListener;				//used for listening to opponent movements
	public MPUpdateListener updateListener;

	//variables to set how many people can play
	private uint minimumOpponents = 1;
	private uint maximumOpponents = 1;
	private uint gameVariation = 0;					//the type of game we are playing

	//used for updating the multiplayer game
	private byte _protocolVersion = 0;				//this is the version of multiplayer game that we are using
	// 2 bytes for info and 6 bytes for the move
	private int _updateMessageLength = 8;
	private List<byte> _updateMessage;
	private int _myMessageNum;

	//used for finishing the game
	// 2 bytes for info and 8 buytes for two floats
	private int _startMessageLength = 18;

	private MultiplayerController() {
		_updateMessage = new List<byte>(_updateMessageLength);			//declare the size of our list
		PlayGamesPlatform.DebugLogEnabled = true;
		PlayGamesPlatform.Activate ();					//have to activate the PlayGamesPlatform
	}

	public static MultiplayerController Instance {
		get {
			if (_instance == null) {
				_instance = new MultiplayerController();
			}
			return _instance;
		}
	}

	/**
	 * 
	 * 	Signing In Stuff
	 * 
	 **/

	//signs in and starts the multiplayer game
	public void SignInAndStartMPGame() {
		if (!PlayGamesPlatform.Instance.localUser.authenticated) {
			PlayGamesPlatform.Instance.localUser.Authenticate((bool success) => {
				if (success) {
					Debug.Log ("We're signed in! Welcome " + PlayGamesPlatform.Instance.localUser.userName);
					// We could start our game now
					StartMatchMaking();
				} else {
					Debug.Log ("Oh... we're not signed in.");
				}
			});
		} else {
			Debug.Log ("You're already signed in.");
			// We could also start our game now
			StartMatchMaking();
		}
	}

	//Method trys to Silently sign you in so you don't have to
	public void TrySilentSignIn() {
		if (! PlayGamesPlatform.Instance.localUser.authenticated) {
			PlayGamesPlatform.Instance.Authenticate ((bool success) => {
				if (success) {
					Debug.Log ("Silently signed in! Welcome " + PlayGamesPlatform.Instance.localUser.userName);
				} else {
					Debug.Log ("Oh... we're not signed in.");
				}
			}, true);
		} else {
			Debug.Log("We're already signed in");
		}
	}

	//signs your account out of Google Play
	public void SignOut() {
		Debug.Log("You Signed out of your google account");
		PlayGamesPlatform.Instance.SignOut();
	}

	//returns if the player is authenticated or not
	public bool IsAuthenticated() {
		return PlayGamesPlatform.Instance.localUser.authenticated;
	}

	/**
	 * 
	 * 	setting up a room and connecting
	 * 
	 **/

	//starts looking for a match
	private void StartMatchMaking() {
		Debug.Log ("Creating Match");
		PlayGamesPlatform.Instance.RealTime.CreateQuickGame (minimumOpponents, maximumOpponents, gameVariation, this);
		//set that we are trying to do a multiplayer game
		RetainedUserPicks.Instance.setIsMultiplayerGame(true);
	}

	//get the participant IDs of all the players
	public List<Participant> GetAllPlayers() {
		return PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants ();
	}
	
	//gets this players participantID
	public string GetMyParticipantId() {
		return PlayGamesPlatform.Instance.RealTime.GetSelf().ParticipantId;
	}

	/*
	 * 
	 * Sending Updates to the Opponent
	 * 
	 */

	//sends the update data as bytes
	public void SendMyUpdate(char[] movesAsChar) {
		Debug.Log ("Sending move update");
		_updateMessage.Clear ();
		_updateMessage.Add (_protocolVersion);
		_updateMessage.Add ((byte)'U');
		//_updateMessage.AddRange(System.BitConverter.GetBytes(++_myMessageNum));
		for (int k = 0; k < 6; k++) {
			_updateMessage.Add ((byte)movesAsChar[k]);
		}
		byte[] messageToSend = _updateMessage.ToArray(); 
		//Debug.Log ("Sending my update message  " + messageToSend + " to all players in the room");
		PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, messageToSend);		//sends message to everyone using Google Play
		//the false means we are using unreliable netoworking which is less exspensive
		//we will probably use true for ProjectN
	}

	//send data for the beginning of the game
	public void SendBeginningData(Vector2 posOne, Vector2 posTwo) {
		Debug.Log ("Sending beginngin update");
		List<byte> bytes = new List<byte>(_startMessageLength); 
		bytes.Add(_protocolVersion);
		bytes.Add((byte)'B');			//B for Beginning
		bytes.AddRange(System.BitConverter.GetBytes (posOne.x));	//passing the bytes for the two positions
		bytes.AddRange(System.BitConverter.GetBytes (posOne.y));
		bytes.AddRange(System.BitConverter.GetBytes (posTwo.x));
		bytes.AddRange(System.BitConverter.GetBytes (posTwo.y));
		//pass the message
		byte[] messageToSend = bytes.ToArray ();
		PlayGamesPlatform.Instance.RealTime.SendMessageToAll (true, messageToSend);
	}

	/*
	 * 
	 * The interface methods that we have to implement for a multiplayer game
	 * 
	 */
	
	public void OnRoomSetupProgress (float percent) {
		Debug.Log("We are " + percent + "% done with setup");
	}
	
	public void OnRoomConnected (bool success) {
		if (success) {
			Debug.Log("We are connected to the room! I would probably start our game now.");
			Application.LoadLevel("BattleScene");			//actually load the game
		} else {
			Debug.Log("Uh-oh. Encountered some error connecting to the room.");
		}
		//hide the stuff and start the game because we are not connected
//		lobbyListener.HideLobby();
//		lobbyListener = null;
//		_myMessageNum = 0;			//sets how many messages passed to 0

	}
	
	public void OnLeftRoom () {
		Debug.Log("We have left the room.");
		if (updateListener != null) {
			updateListener.LeftRoomConfirmed();
		}
	}
	
	public void OnParticipantLeft (Participant participant) {
		Debug.Log("The other participant left the room");
		updateListener.PlayerLeftRoom(participant.ParticipantId);
	}
	
	public void OnPeersConnected (string[] participantIds) {
		foreach (string participantID in participantIds) {
			Debug.Log("Player " + participantID + " has joined.");
		}
	}
	
	public void OnPeersDisconnected (string[] participantIds) {
		foreach (string participantID in participantIds) {
			Debug.Log("Player " + participantID + " has left.");
			if (updateListener != null) {
				updateListener.PlayerLeftRoom(participantID);
			}
		}
	}
	
	//leaves the game using Google plays method
	public void LeaveGame() {
		PlayGamesPlatform.Instance.RealTime.LeaveRoom();
	}
	
	//fuction that gets called whenever you recieve a message from the network
	public void OnRealTimeMessageReceived (bool isReliable, string senderId, byte[] data) {
		Debug.Log("Recieved a real time message");
		byte messageVersion = (byte)data[0];

		// Let's figure out what type of message this is.
		char messageType = (char)data[1];
		if (messageType == 'U' && data.Length == _updateMessageLength) { 			//if it is a upate message
//			int messageNum = System.BitConverter.ToInt32(data, 2);
//			float posX = System.BitConverter.ToSingle(data, 2);
//			float posY = System.BitConverter.ToSingle(data, 6);
//			float velX = System.BitConverter.ToSingle(data, 10);
//			float velY = System.BitConverter.ToSingle(data, 14);
//			float rotZ = System.BitConverter.ToSingle(data, 18);
			char[] messageToReceive = new char[6];
			for(int k = 0; k < 6; k++) {
				messageToReceive[k] = (char)data[k + 2];
				Debug.Log(messageToReceive[k]);
			}

			// We'd better tell our OpponentController about this.
			if (opponentListener != null) {
				opponentListener.UpdateReceived(senderId, messageToReceive);
			}
		} else if (messageType == 'B' && data.Length == _startMessageLength) {		//if it is a gameover message
			// We received a final time!
			float posX = System.BitConverter.ToSingle(data, 2);
			float posY = System.BitConverter.ToSingle(data, 6);
			float posTwoX = System.BitConverter.ToSingle(data, 10);
			float posTwoY = System.BitConverter.ToSingle(data, 14);
			//pass the data to the listener. We pass it in a different order so the devices end up the same
			updateListener.SetPlayerPositions(new Vector2(posTwoX, posTwoY), new Vector2(posX, posY));
		}

	}

}
