using UnityEngine;
using System.Collections;

public class PickStartLocations : MonoBehaviour {

	public int rows = 5;
	public int columns = 9;
	public Transform playerTran;
	public Transform enemyTran;

//	private Vector2 startPos;
//	private Vector2 startPosEnemy;

	// Use this to call from GameManager
//	public void pickLocations() {
//		setBothVectors();
//		addPlayersToScene();
//	}

//	//getters
//	public Vector2 getStartPos() {
//		return startPos;
//	}
//
//	public Vector2 getStartPosEnemy() {
//		return startPosEnemy;
//	}

	//use this to set the vectors
	public Vector2[] setBothPositions() {
		Vector2 posOne;
		Vector2 posTwo;
		int topOrBottom = Random.Range (0, 2);
		print ("topOrBottom is " + topOrBottom);
		if (topOrBottom == 0) {
			//one on the top
			int xPos = Random.Range(0, columns);
			posOne = new Vector3 (xPos, (rows - 1), 0f);
			//one on the bottom
			xPos = Random.Range(0, columns);
			posTwo = new Vector3 (xPos, 0f, 0f);
		} else {
			//one on the top
			int xPos = Random.Range(0, columns);
			posTwo = new Vector3 (xPos, (rows - 1), 0f);
			//one on the bottom
			xPos = Random.Range(0, columns);
			posOne = new Vector3 (xPos, 0f, 0f);
		}
		return new Vector2[] {posOne, posTwo};

	}

	public void addPlayersToScene(Vector2 posOne, Vector2 posTwo) {

		Quaternion target = Quaternion.identity;
		Quaternion target2 = Quaternion.identity;
		//figure out how to rotate the two boats
		if(posOne.y == 0) {	//if the player is at the bottom
			target = Quaternion.Euler(270f, 0f, 0f);			//to the north Quaternion.Euler(180f, 0f, 0f);//
			target2 = Quaternion.Euler(-270f, -90f, 90f);		//to the south
		} else {
			target2 = Quaternion.Euler(270f, 0f, 0f);			//to the north
			target = Quaternion.Euler(-270f, -90f, 90f);		//to the south Quaternion.Euler(0f, 180f, 0f);//
		}

		//set the players transform and rotation
		playerTran.position = posOne;
		playerTran.rotation = target;
		enemyTran.position = posTwo;
		enemyTran.rotation = target2;
	}

	
}
