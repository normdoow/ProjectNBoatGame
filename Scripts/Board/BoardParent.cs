using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;       //Allows us to use Lists.
using Random = UnityEngine.Random;      //Tells Random to use the Unity Engine random number generator.

/// <summary>
/// Board parent has all the tools methods for the board. It also has all the 
/// variables and getters and setters
/// The Board is used for all the logic for the game. 
/// So it is really important!
/// </summary>

public abstract class BoardParent : MonoBehaviour {

	public static int columns = 9;                                         //Number of columns in our game board.
	public static int rows = 5;                                            //Number of rows in our game board.

	protected Transform boardHolder;                                  //A variable to store a reference to the transform of our Board object.
	protected List <Vector3> gridPositions = new List <Vector3> ();   //A list of possible locations to place tiles.
	public static string testStatic = "";
	protected static string[,] grid;
	//player variables
	protected static Vector2 currentPos;
	protected static string direction = "North";
	protected static string priorDirection = null;
	protected static float momentum = 0;
	
	//enemy variables
	protected static Vector2 currentPosEnemy;
	protected static string directionEnemy = "South";
	protected static string priorDirectionEnemy = null;
	protected static Vector2 priorPosEnemy;
	protected static float momentumEnemy = 0;

	//getters
	public static string[,] getGrid() {
		return grid;
	}
	
	public static int getRows() {
		return rows;
	}
	
	public static int getColumns() {
		return columns;
	}
	//player getters
	public static Vector2 getCurrentPos() {
		return currentPos;
	}
	
	public static int getCurrentPosX() {
		return (int)currentPos.x;
	}
	public static int getCurrentPosY() {
		return (int)currentPos.y;
	}
	
	public static String getDirection() {
		return direction;
	}
	//enemy getters
	public static Vector2 getCurrentPosEnemy() {
		return currentPosEnemy;
	}
	
	public static int getCurrentPosXEnemy() {
		return (int)currentPosEnemy.x;
	}
	public static int getCurrentPosYEnemy() {
		return (int)currentPosEnemy.y;
	}
	
	public static String getDirectionEnemy() {
		return directionEnemy;
	}
	
	//setters
	protected static void setCurrentPos(int x, int y) {
		currentPos.x = x;
		currentPos.y = y;
	}
	protected static void setDirection(string dir) {
		direction = dir;
	}
	protected static void setPriorDirection(string dir) {
		priorDirection = dir;
	}

	protected static void setMomentum(float mom) {
		momentum = mom;
	}
	
	protected static void setCurrentPosEnemy(int x, int y) {
		currentPosEnemy.x = x;
		currentPosEnemy.y = y;
	}

	protected static void setDirectionEnemy(string dir) {
		directionEnemy = dir;
	}

	protected static void setPriorDirectionEnemy(string dir) {
		priorDirectionEnemy = dir;
	}

	protected static void setMomentumEnemy(float mom) {
		momentumEnemy = mom;
	}

	protected static bool checkIsNoBoatCollision(int newX, int newY) {
		//more possible collissions other than this
		//direct collision into same grid
		if (grid [newX, newY] == "Enemy") {
			print ("There is a direct boat collision");
			if (momentum == 1 && momentumEnemy == 1) {
				//move enemy back to his original space
				moveEnemyBackToPos ();
			} else if (momentum == 0.5 && momentumEnemy == 0.5) {
				if (!isSpecialTurnCase ()) {			//if it is not the special case do whats inside
					movePlayerWithTurn (newX, newY);
					moveEnemyWithTurn ();
				}
			} else if (momentum == 0 || momentumEnemy == 0) {
				if (momentumEnemy == 0) {
					movePlayerWithTurn (newX, newY);
				} else {		//momentum must Be 0
					moveEnemyWithTurn ();
				}
			} else if (momentum == 0.5 && momentumEnemy == 1) {
				if (!isSpecialStraightTurnCase (true)) {//true since player is turning //check for the special cases
					movePlayerWithTurn (newX, newY);				//player does his turn
					//move enemy back to original space
				}
				moveEnemyBackToPos ();
			} else if (momentum == 1 && momentumEnemy == 0.5) {
				if (!isSpecialStraightTurnCase (false)) {//false since enemy is turning //check for the special cases
					//player stays in the same place
					moveEnemyWithTurn ();						//enemy does his turn
				}
			}
			return false;
		} else if (isNorthSouthFacingCase () || isWestEastFacingCase ()) {
			//if the ships are facing eachother
			//and one away do nothing and just let them change direction
			moveEnemyBackToPos ();
			return false;
		} else if (isSideBySideGoingTowardsEachOtherCase ()) {
			movePlayerWithTurn (newX, newY);
			moveEnemyWithTurn ();
			return false;
		} else if (isFacingShipThatIsNotMovingCase ()) {
			moveEnemyBackToPos ();
			return false;
		} else if (currentPos == currentPosEnemy && priorPosEnemy.x == newX && priorPosEnemy.y == newY
		           && !areOppositeDirections(priorDirection, priorDirectionEnemy)) {
			//if the ships are moving towards the same place, they will collide
			moveEnemyBackToPos ();
			return false;
		} else if (isDiagonalAndGoingThroughSameSpotCase (newX, newY)) {
			moveEnemyBackToPos ();
			return false;
		}
		return true;
	}

	protected static void moveEnemyBackToPos() {
		grid[(int)currentPosEnemy.x, (int)currentPosEnemy.y] = "empty";
		currentPosEnemy = priorPosEnemy;
		grid[(int)currentPosEnemy.x, (int)currentPosEnemy.y] = "Enemy";
	}
	
	protected static void movePlayerWithTurn(int newX, int newY) {
		switch(direction) {
		case "North":
		case "South":
			setCurrentPos(newX, (int)currentPos.y);
			break;
		case "West":
		case "East":
			setCurrentPos((int)currentPos.x, newY);
			break;
		}
	}
	
	protected static void moveEnemyWithTurn() {
		grid[(int)currentPosEnemy.x, (int)currentPosEnemy.y] = "empty";
		switch(directionEnemy) {
		case "North":
			grid[(int)currentPosEnemy.x, (int)currentPosEnemy.y - 1] = "Enemy";
			setCurrentPosEnemy((int)currentPosEnemy.x, (int)currentPosEnemy.y - 1);
			break;
		case "South":
			grid[(int)currentPosEnemy.x, (int)currentPosEnemy.y + 1] = "Enemy";
			setCurrentPosEnemy((int)currentPosEnemy.x, (int)currentPosEnemy.y + 1);
			break;
		case "West":
			grid[(int)currentPosEnemy.x + 1, (int)currentPosEnemy.y] = "Enemy";
			setCurrentPosEnemy((int)currentPosEnemy.x + 1, (int)currentPosEnemy.y);
			break;
		case "East":
			grid[(int)currentPosEnemy.x - 1, (int)currentPosEnemy.y] = "Enemy";
			setCurrentPosEnemy((int)currentPosEnemy.x - 1, (int)currentPosEnemy.y);
			break;
		}
	}
	
	//used for special case when both boats are turning
	protected static bool isSpecialTurnCase() {
		bool isSpecialCase = false;
		switch(direction) {
		case "North":
		case "South":
			if(((int)priorPosEnemy.x == (int)currentPos.x + 2 || (int)priorPosEnemy.x == (int)currentPos.x - 2)
			   			&& (int)priorPosEnemy.y == (int)currentPos.y) {		//if this then there is the special case
				isSpecialCase = true;
			}
			break;
		case "West":
		case "East":
			if(((int)priorPosEnemy.y == (int)currentPos.y - 2 || (int)priorPosEnemy.y == (int)currentPos.y + 2) 
			   			&& (int)priorPosEnemy.x == (int)currentPos.x) {		//if this then there is the special case
				isSpecialCase = true;
			}
			break;
		}
		if(isSpecialCase) {			//if it is the special case move the enemy back so both players don't move
			moveEnemyBackToPos();
		}
		return isSpecialCase;
	}
	
	//used when one boat is going straight and the other is turning
	protected static bool isSpecialStraightTurnCase(bool isPlayer) {
		bool isSpecialCase = false;
		if (isPlayer) {
			switch (direction) {
			case "North":
			case "South":
				if ((int)priorPosEnemy.x == (int)currentPos.x + 1 || (int)priorPosEnemy.x == (int)currentPos.x - 1) {		//if this then there is the special case
					isSpecialCase = true;
				}
				break;
			case "West":
			case "East":
				if ((int)priorPosEnemy.y == (int)currentPos.y - 1 || (int)priorPosEnemy.y == (int)currentPos.y + 1) {		//if this then there is the special case
					isSpecialCase = true;
				}
				break;
			}
		} else {
			switch (directionEnemy) {
			case "North":
			case "South":
				if ((int)priorPosEnemy.x == (int)currentPos.x + 1 || (int)priorPosEnemy.x == (int)currentPos.x - 1) {		//if this then there is the special case
					isSpecialCase = true;
				}
				break;
			case "West":
			case "East":
				if ((int)priorPosEnemy.y == (int)currentPos.y - 1 || (int)priorPosEnemy.y == (int)currentPos.y + 1) {		//if this then there is the special case
					isSpecialCase = true;
				}
				break;
			}
		}
		if(isSpecialCase) {			//if it is the special case move the enemy back so both players don't move
			moveEnemyBackToPos();
			grid[(int)currentPos.x, (int)currentPos.y] = "Player";
		}
		return isSpecialCase;
	}

	//case for when the ships are diagonal from each other, one turns and the other goes straight so it looks like they collide
	protected static bool isDiagonalAndGoingThroughSameSpotCase(int newX, int newY) {
		if (momentum == 1 && momentumEnemy == 0.5) {
			if(currentPosEnemy == currentPos) {
				if(direction == "North" && priorPosEnemy.y == currentPos.y + 1) {
					return true;
				} else if(direction == "South" && priorPosEnemy.y == currentPos.y - 1) {
					return true;
				} else if(direction == "West" && priorPosEnemy.x == currentPos.x - 1) {
					return true;
				} else if(direction == "East" && priorPosEnemy.x == currentPos.x + 1) {
					return true;
				}
			}
		} else if (momentum == 0.5 && momentumEnemy == 1) {
			if((int)currentPosEnemy.x == newX && (int)currentPosEnemy.y == newY) {
				if(directionEnemy == "North" && priorPosEnemy.y == currentPos.y - 1) {
					return true;
				} else if(directionEnemy == "South" && priorPosEnemy.y == currentPos.y + 1) {
					return true;
				} else if(directionEnemy == "West" && priorPosEnemy.x == currentPos.x + 1) {
					return true;
				} else if(directionEnemy == "East" && priorPosEnemy.x == currentPos.x - 1) {
					return true;
				}
			}
		}
		return false;
	}

	//see if the ships are side by side and turning towards each other
	protected static bool isSideBySideGoingTowardsEachOtherCase() {
		if (priorDirection == priorDirectionEnemy) {
			if (direction == "North" && directionEnemy == "South" && (int)currentPos.y + 1 == (int)priorPosEnemy.y 
				&& (int)currentPos.x == (int)priorPosEnemy.x) {
				return true;
			} else if (direction == "South" && directionEnemy == "North" && (int)currentPos.y - 1 == (int)priorPosEnemy.y
				&& (int)currentPos.x == (int)priorPosEnemy.x) {
				return true;
			} else if (direction == "West" && directionEnemy == "East" && (int)currentPos.x - 1 == (int)priorPosEnemy.x
				&& (int)currentPos.y == (int)priorPosEnemy.y) {
				return true;
			} else if (direction == "East" && directionEnemy == "West" && (int)currentPos.x + 1 == (int)priorPosEnemy.x
				&& (int)currentPos.y == (int)priorPosEnemy.y) {
				return true;
			}
		}
		return false;
	}

	//see if one of the ships is facing the other and one is staying still
	protected static bool isFacingShipThatIsNotMovingCase() {
		if (momentum > 0 && momentumEnemy == 0.0) {
			if(priorDirection == "North" && (int)currentPos.y + 1 == (int)priorPosEnemy.y 
			   && (int)currentPos.x == (int)priorPosEnemy.x) {
				return true;
			} else if(priorDirection == "South" && (int)currentPos.y - 1 == (int)priorPosEnemy.y 
			        && (int)currentPos.x == (int)priorPosEnemy.x) {
				return true;
			}else if(priorDirection == "West" && (int)currentPos.x - 1 == (int)priorPosEnemy.x
					&& (int)currentPos.y == (int)priorPosEnemy.y) {
				return true;
			} else if(priorDirection == "East" && (int)currentPos.x + 1 == (int)priorPosEnemy.x 
			       	&& (int)currentPos.y == (int)priorPosEnemy.y) {
				return true;
			}
		} else if(momentum == 0.0 && momentumEnemy > 0) {
			if(priorDirectionEnemy == "North" && (int)currentPos.y - 1 == (int)priorPosEnemy.y 
			   && (int)currentPos.x == (int)priorPosEnemy.x) {
				return true;
			} else if(priorDirectionEnemy == "South" && (int)currentPos.y + 1 == (int)priorPosEnemy.y 
			          && (int)currentPos.x == (int)priorPosEnemy.x) {
				return true;
			}else if(priorDirectionEnemy == "West" && (int)currentPos.x + 1 == (int)priorPosEnemy.x
			         && (int)currentPos.y == (int)priorPosEnemy.y) {
				return true;
			} else if(priorDirectionEnemy == "East" && (int)currentPos.x - 1 == (int)priorPosEnemy.x 
			          && (int)currentPos.y == (int)priorPosEnemy.y) {
				return true;
			}
		}
		return false;
	}

	//used for the case when two boats are facing eachother for a collision
	protected static bool isNorthSouthFacingCase() {
		return (priorDirection == "North" && priorDirectionEnemy == "South" 
		        && (int)priorPosEnemy.y == (int)currentPos.y + 1
		        && (int)priorPosEnemy.x == (int)currentPos.x) || 
				(priorDirection == "South" && priorDirectionEnemy == "North" 
			 	&& (int)priorPosEnemy.y == (int)currentPos.y - 1
			 	&& (int)priorPosEnemy.x == (int)currentPos.x);
	}

	//used for the case when two boats are facing eachother for a collision
	protected static bool isWestEastFacingCase() {
		return (priorDirection == "West" && priorDirectionEnemy == "East" 
		        && (int)priorPosEnemy.x == (int)currentPos.x - 1 
		        && (int)priorPosEnemy.y == (int)currentPos.y) || 
				(priorDirection == "East" && priorDirectionEnemy == "West" 
				&& (int)priorPosEnemy.x == (int)currentPos.x + 1
			 	&& (int)priorPosEnemy.y == (int)currentPos.y);
	}

	protected static bool areOppositeDirections(string firstDir, string secDir) {
		if ((firstDir == "North" && secDir == "South") || (firstDir == "South" && secDir == "North")) {
			return true;
		} else if (firstDir == "West" && secDir == "East" || firstDir == "East" && secDir == "West") {
			return true;
		}
		return false;
	}

	protected static bool isWallCollisionOnX(int x) {
		return (x < 0 || x >= columns); 
	}
	
	protected static bool isWallCollisionOnY(int y) {
		return (y < 0 || y >= rows);
	}

}
