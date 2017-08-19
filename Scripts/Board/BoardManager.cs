using UnityEngine;
using System;
using System.Collections.Generic;       //Allows us to use Lists.
//using Random = UnityEngine.Random;      //Tells Random to use the Unity Engine random number generator.

/// <summary>
/// Board Manager is used for updating the board and is a child of Board Parent
/// </summary>
public class BoardManager : BoardParent {
	// Using Serializable allows us to embed a class with sub properties in the inspector.
//	[Serializable]
//	public class Count {
//		public int minimum;             //Minimum value for our Count class.
//		public int maximum;             //Maximum value for our Count class.
//
//		//Assignment constructor.
//		public Count (int min, int max) {
//			minimum = min;
//			maximum = max;
//		}
//	}

	public GameObject gridTile;										//tile from prefab
	public GameObject wallTile;

	public static void initGrid(Vector2 playerPos, Vector2 enemyPos) {
		grid = new string[columns, rows];
		for (int x = 0; x < columns; x++) {
			for (int y = 0; y < rows; y++) {
				grid[x,y] = "empty";
			}
		}
		grid[0,0] = "Player";
		grid [8, 4] = "Enemy";
		currentPos = playerPos;
		currentPosEnemy = enemyPos;
		if(playerPos.y == 0) {	//if the player is at the bottom
			direction = "North";
			directionEnemy = "South";
		} else {
			direction = "South";
			directionEnemy = "North";
		}
	}
	
	//Clears our list gridPositions and prepares it to generate a new board.
//	void InitialiseList () {
//		//Clear our list gridPositions.
//		gridPositions.Clear ();
//		
//		//Loop through x axis (columns).
//		for(int x = 0; x < columns; x++) {
//			//Within each column, loop through y axis (rows).
//			for(int y = 0; y < rows; y++) {
//				//At each index add a new Vector3 to our list with the x and y coordinates of that position.
//				gridPositions.Add (new Vector3(x, y, 0f));
//			}
//		}
//	}

	//Sets up the outer walls and floor (background) of the game board.
	void BoardSetup () {
		//Instantiate Board and set boardHolder to its transform.
		boardHolder = new GameObject ("Board").transform;
		
		//Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
		for(int x = 0; x < columns; x++) {
			//Loop along y axis, starting from -1 to place floor or outerwall tiles.
			for(int y = 0; y < rows; y++) {
//					//Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
//					GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)];
//					
//					//Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
//					if(x == -1 || x == columns || y == -1 || y == rows)
//						//toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];

				GameObject toInstantiate = gridTile;

//				if(x == -1 || x == columns || y == -1 || y == rows) {
//					wallTile.tag = "Wall";
//					toInstantiate = wallTile;
//				}
				//toInstantiate.transform.Rotate(90f,0,0);
				//Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
				GameObject instance =
					Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
				
				//Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
				instance.transform.SetParent (boardHolder);
			}
		}
	}

	public static void updateBoardWithPlayerMove(string move) {
		grid[(int)currentPos.x, (int)currentPos.y] = "empty";
		Vector2 newPosition = calculatePosFrom(getDirection(), move, currentPos);		//get the position we want
		setPriorDirection(getDirection());
		setDirection(calculateDirectionFrom(getDirection(), move));			//set the direction
		setMomentum(calculateMomentum(move));
		if(checkIsNoBoatCollision((int)newPosition.x, (int)newPosition.y)) {
			setCurrentPos((int)newPosition.x , (int)newPosition.y);
		}
		grid[(int)currentPos.x, (int)currentPos.y] = "Player";
	}

	public static void updateBoardWithEnemyMove(string move) {
		grid[(int)currentPosEnemy.x, (int)currentPosEnemy.y] = "empty";
		priorPosEnemy = currentPosEnemy;			//set prior before we change it
		Vector2 newPosition = calculatePosFrom(getDirectionEnemy(), move, currentPosEnemy);		//get the position we want
		setPriorDirectionEnemy(getDirectionEnemy()); 
		setDirectionEnemy(calculateDirectionFrom(getDirectionEnemy(), move));			//set the direction
		setMomentumEnemy(calculateMomentum(move));
		setCurrentPosEnemy((int)newPosition.x, (int)newPosition.y);
		grid[(int)newPosition.x, (int)newPosition.y] = "Enemy";
	}

	private static float calculateMomentum(string moveString) {
		float mom = 1f;
		if (moveString == "Stay") {
			mom = 0f;
		} else if (moveString == "Left") {
			mom = 0.5f;
		} else if(moveString == "Right") {
			mom = 0.5f;
		}
		return mom;
	}

	public static string calculateDirectionFrom(string startDirection, string moveString) {
		string returnedDirection = startDirection;
		if (moveString != "Up" && moveString != "Stay") {
			switch (startDirection) {
			case "North":
				if (moveString == "Right") {
					returnedDirection = "East";
				} else {						//otherwise the move is Left
					returnedDirection = "West";
				}
				break;
			case "South":
				if (moveString == "Right") {
					returnedDirection = "West";
				} else {
					returnedDirection = "East";
				}
				break;
			case "West":
				if (moveString == "Right") {
					returnedDirection = "North";
				} else {
					returnedDirection = "South";
				}
				break;
			case "East":
				if (moveString == "Right") {
					returnedDirection = "South";
				} else {
					returnedDirection = "North";
				}
				break;
			}
		}
		return returnedDirection;
	}

	//updating the board for the enemy
	public static Vector2 calculatePosFrom(string startDirection, string moveString, Vector2 position) {

		Vector2 returnedMove = position;
		//set the value of the move based on the 
		int move = 1;			//if the move is right or up it will already be 1
		if (moveString == "Stay") {
			move = 0;
		} else if (moveString == "Left") {
			move = -1;
		}

		switch(startDirection) {
		case "North":
			if(moveString == "Left" || moveString == "Right") {
				returnedMove.x = calculateXMove((int)position.x + move, (int)position.y + 1, startDirection, (int)position.x);
				returnedMove.y = calculateYMove((int)position.x + move, (int)position.y + 1, startDirection, (int)position.y);
			} else {						//were dealing with a vertical
				if(!isWallCollisionOnY((int)position.y + move)) {				//if no wall collision for going forward
					returnedMove.x = (int)position.x;
					returnedMove.y = (int)position.y + move;
				}
			}
			break;
		case "South":
			if(moveString == "Left" || moveString == "Right") {
				returnedMove.x = calculateXMove((int)position.x - move, (int)position.y - 1, startDirection, (int)position.x);
				returnedMove.y = calculateYMove((int)position.x - move, (int)position.y - 1, startDirection, (int)position.y);
			} else {						//were dealing with a vertical
				if(!isWallCollisionOnY((int)position.y - move)) {				//if no wall collision for going forward
					returnedMove.x = (int)position.x;
					returnedMove.y = (int)position.y - move;
				}
			}
			break;
		case "West":
			if(moveString == "Left" || moveString == "Right") {
				returnedMove.x = calculateXMove((int)position.x - 1, (int)position.y + move, startDirection, (int)position.x);
				returnedMove.y = calculateYMove((int)position.x - 1, (int)position.y + move, startDirection, (int)position.y);
			} else {						//were dealing with a vertical
				if(!isWallCollisionOnX((int)position.x - move)) {
					returnedMove.x = (int)position.x - move;
					returnedMove.y = (int)position.y;
				}
			}
			break;
		case "East":
			if(moveString == "Left" || moveString == "Right") {
				returnedMove.x = calculateXMove((int)position.x + 1, (int)position.y - move, startDirection, (int)position.x);
				returnedMove.y = calculateYMove((int)position.x + 1, (int)position.y - move, startDirection, (int)position.y);
			} else {						//were dealing with a vertical
				if(!isWallCollisionOnX((int)position.x + move)) {					//if no wall collision
					returnedMove.x = (int)position.x + move;
					returnedMove.y = (int)position.y;
				}
			}
			break;
		}
		return returnedMove;			//this is the move that we want to return
	}

	//calculates what x should be based on if it is off the wall
	protected static int calculateXMove(int x, int y, string startDirection, int posX) {
		if(isWallCollisionOnX(x)) {						//check if there is a collision for x
			//print ("Had Wall Collision!");
			return posX;									//if there is make it 0
		}
		if(startDirection == "North" || startDirection == "South") {				//needed for case where you are already facing
			if(isWallCollisionOnY(y)) {									//the wall and you want to turn
				return posX;								//then you only change diretion
			}
		}
		return x;
	}
	
	//calculates what y should be based on if it is off the wall
	protected static int calculateYMove(int x, int y, string startDirection, int posY) {
		if(isWallCollisionOnY(y)) {						//check if there is a collision for y
			//print ("Had Wall Collision!");
			return posY;								//if there is make it 0
		}
		if(startDirection == "East" || startDirection == "West") {				//needed for case where you are already facing
			if(isWallCollisionOnX(x)) {									//the wall and you want to turn
				return posY;								//then you only change diretion
			}
		}
		return y;
	}

	//SetupScene initializes our level and calls the previous functions to lay out the game board
	public void SetupScene (/*int level*/) {
		//Creates the outer walls and floor.
		BoardSetup ();
		//Reset our list of gridpositions.
		//InitialiseList ();
		
//			//Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
//			LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);
//			
//			//Instantiate a random number of food tiles based on minimum and maximum, at randomized positions.
//			LayoutObjectAtRandom (foodTiles, foodCount.minimum, foodCount.maximum);
//			
//			//Determine number of enemies based on current level number, based on a logarithmic progression
//			int enemyCount = (int)Mathf.Log(level, 2f);
//			
//			//Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
//			LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount);
//			
//			//Instantiate the exit tile in the upper right hand corner of our game board
//			Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0f), Quaternion.identity);
	}

}
