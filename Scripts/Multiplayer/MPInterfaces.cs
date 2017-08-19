using UnityEngine;
//interface for the MainMenu to follow
//public interface MPLobbyListener {
//	void SetLobbyStatusMessage(string message);
//	void HideLobby();
//}

//interface for the GameController to follow
public interface MPUpdateListener {
//	void UpdateReceived(string participantId, int messageNum, float posX, float posY, float velX, float velY, float rotZ);
//	void PlayerFinished(string senderId, float finalTime);
	void LeftRoomConfirmed();
	void PlayerLeftRoom(string participantId);
	void SetPlayerPositions(Vector2 posOne, Vector2 posTwo);
}

//interface used for the opponent
public interface MPOpponentListener {
	void UpdateReceived(string participantId, char[] opponentMoves);
//	void LeftRoomConfirmed();
//	void PlayerLeftRoom(string participantId);
}