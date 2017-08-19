using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using System.Threading.Tasks;

public class EasyAI : MonoBehaviour 
{
	int playerShipX = BoardManager.getCurrentPosX();//x position of the opposing ship
	int playerShipY = BoardManager.getCurrentPosY(); //y position of the opposing ship
	string playerShipDir = BoardManager.getDirection(); //North
	int myX = BoardManager.getCurrentPosXEnemy();
	int myY = BoardManager.getCurrentPosYEnemy();
	string myDir = BoardManager.getDirectionEnemy();
	int numRows = BoardManager.getRows();
	int numColumns = BoardManager.getColumns();
	System.Random rnd= new System.Random();
	private int level = 0; //0 for easy, 1 for medium
	int difficulty = 50; //sets the percentage of the time that the best move will be used
	int howEasy=20;
	// Use this for initialization
	void Start () 
	{
		//playerShipX = BoardManager.getCurrentPosX(); //x position of the opposing ship
		//playerShipY = BoardManager.getCurrentPosY(); //y position of the opposing ship
		//playerShipDir = BoardManager.getDirection(); //North
		//myX = BoardManager.getCurrentPosXEnemy();
		//myY = BoardManager.getCurrentPosYEnemy();
		//myDir = BoardManager.getDirectionEnemy();
		//numRows = BoardManager.getRows();
		//numColumns = BoardManager.getColumns();
	}
	public string[] smartMove() //this function returns a string array with length of 6 containing a set of 3 moves and 3 shots
	{
		
		int[, ,] grid = guessMoveGrid();
		string[] moveCombo = new string[6]; //format {firstMove, firstShot, secondMove, secondShot, thirdMove, thirdShot}
		
		
		string firstMove = pickMove(BoardManager.getCurrentPosXEnemy(), BoardManager.getCurrentPosYEnemy(), BoardManager.getDirectionEnemy(),1,grid,difficulty); //creates the first move in form " move, shot"
		string[] splitArray = firstMove.Split(',');
		moveCombo[0] = splitArray[0];
		moveCombo[1] = splitArray[1];
		
		Vector2 pos;
		pos.x = BoardManager.getCurrentPosXEnemy();
		pos.y = BoardManager.getCurrentPosYEnemy();
		String newDir = BoardManager.calculateDirectionFrom(BoardManager.getDirectionEnemy(), moveCombo[0]);
		Vector2 newPos = BoardManager.calculatePosFrom(BoardManager.getDirectionEnemy(), moveCombo[0], pos);
		int newX = (int)newPos.x;
		int newY = (int)newPos.y;
		
		
		string secondMove = pickMove(newX, newY, newDir, 2, grid,difficulty);
		string[] splitArray1 = secondMove.Split(',');
		moveCombo[2]=splitArray1[0];
		moveCombo[3] = splitArray1[1];
		
		Vector2 pos2;
		pos2.x = newX;
		pos2.y = newY;
		String newDir2 = BoardManager.calculateDirectionFrom(newDir, moveCombo[2]);
		Vector2 newPos2 = BoardManager.calculatePosFrom(newDir, moveCombo[2], pos2);
		int newX2 = (int)newPos2.x;
		int newY2 = (int)newPos2.y;
		
		
		string thirdMove = pickMove(newX2, newY2, newDir2, 3, grid,difficulty);
		string[] splitArray2 = thirdMove.Split(',');
		moveCombo[4]=splitArray2[0];
		moveCombo[5] = splitArray2[1];
		
		return moveCombo;
		//return new string[]{"Right","Right","Left","Right", "Left", "Right"}; //this is a test
		
	}
	
	public void setDifficulty(int diff) {
		if (diff > -1 && diff < 101) {
			difficulty = diff;
		} else {
			difficulty = 0;
			print ("Illegal difficulty passed");
		}
	}

	public void setLevel(int lev) {
		if (lev == 0 || lev == 1) {
			level = lev;
		} else {
			lev = 0;
			print ("Illegal difficulty passed");
		}
	}
	
	//	//this function returns a string array with length of 6 containing a set of 3 moves and 3 shots
	//	public string[] smartMove() {
	//		//rewriting what you had above
	//		int[, ,] grid = guessMoveGrid();
	//		string[] moveCombo = new string[6]; //format {firstMove, firstShot, secondMove, secondShot, thirdMove, thirdShot}
	//		int xMove = myX;
	//		int yMove = myY;
	//		string currentDir = myDir;
	//		//go through and make the 6 different moves 2 at a time. That is why k increments by 2
	//		for(int k = 0, moveCount = 1; k < moveCombo.Length; k += 2, moveCount++) {
	//			string moveString = pickMove(xMove, yMove, currentDir, moveCount, grid, difficulty); //creates the first move in form " move, shot"
	//			string[] splitArray = moveString.Split(',');
	//			moveCombo[k] = splitArray[0];						//set moveCombo
	//			moveCombo[k + 1] = splitArray[1];
	//			String newDir = BoardManager.calculateDirectionFrom(currentDir, moveCombo[k]);
	//			Vector2 newPos = BoardManager.calculatePosFrom(currentDir, moveCombo[k], new Vector2((float)xMove, (float)yMove));
	//			xMove = (int)newPos.x;				//make the new moves for next round
	//			yMove = (int)newPos.y;
	//			currentDir = newDir;			//set the new direction for next round
	//		}
	//		return moveCombo;
	//	
	//	}
	
	string pickMove(int x, int y, string dir, int moveNum, int[,,] grid, int bestMovePercentage) //try All 4 moves with shots in each direction to return a string of the form "Move,Shot"
	{
		int bestSum = 0;
		int secondSum = 0;
		string[] bestMoves = new string[10];
		string[] secondMoves = new string[10];
		int bestIndex = 0; //how many spots have been filled in the best array
		int secondIndex = 0; //how many spots have been filled in the second array
		string move="Right";
		string shoot="Right";
		
		for (int w = 0; w < 4; w++) //assume 0 is Stay, 1 is Left, 2 is Up, and 3 is Right
		{
			string currentMove;
			switch (w)
			{
			case 0:
				currentMove = "Stay";
				break;
			case 1:
				currentMove = "Left";
				break;
			case 2:
				currentMove = "Up";
				break;
			case 3:
				currentMove = "Right";
				break;
			default:
				currentMove = "Stay";
				break;
				
			}
			string currentState=(x.ToString()+","+y.ToString()+","+dir);
			string newState=moveResultString(currentState, currentMove);
			string[] splitArray = newState.Split(',');
			int newX = Int32.Parse(splitArray[0]);
			int newY = Int32.Parse(splitArray[1]);
			string newDir = splitArray[2];
			int leftAdder=0;
			int rightAdder=0;
			switch (newDir)
			{
			case("North"):
				//left
				if(newX-1>0) //check if there's one square to the left 
				{
					leftAdder=leftAdder+grid[newX-1,newY,(moveNum-1)];
				}
				if(newX-2>0) //check if there's two squares to the left 
				{
					leftAdder = leftAdder + grid[newX - 2, newY, (moveNum - 1)];
				}
				if (leftAdder > bestSum)
				{
					secondSum=bestSum;
					bestSum=leftAdder;
					secondIndex=bestIndex;
					bestIndex=0;
					Array.Copy(bestMoves, secondMoves, 10);
					Array.Clear(bestMoves,0,bestMoves.Length);
					bestMoves[bestIndex]=currentMove+",Left";
					bestIndex++;
				}
				else if (leftAdder==bestSum && bestSum != 0)
				{		
					bestMoves[bestIndex]=currentMove+",Left";
					bestIndex++;
				}
				else if (leftAdder<bestSum && leftAdder>secondSum)
				{		
					secondSum=leftAdder;
					Array.Clear(secondMoves,0,secondMoves.Length);
					secondIndex=0;
					secondMoves[secondIndex]=currentMove+",Left";
					secondIndex++;
				}
				else if (leftAdder==secondSum && secondSum!=0)
				{
					secondMoves[secondIndex]=currentMove+",Left";
					secondIndex++;
				}
				//right
				if(newX+1>0&& newX+1<=numColumns-1) //check if there's one square to the right
				{
					rightAdder=rightAdder+grid[newX+1,newY,(moveNum-1)];
				}
				if(newX+2>0 && newX+2<=numColumns-1) //check if there's two squares to the right 
				{
					rightAdder = rightAdder + grid[newX + 2, newY, (moveNum - 1)];
				}
				if (rightAdder > bestSum)
				{
					secondSum=bestSum;
					bestSum=rightAdder;
					secondIndex=bestIndex;
					bestIndex=0;
					Array.Copy(bestMoves, secondMoves, 10);
					Array.Clear(bestMoves,0,bestMoves.Length);
					bestMoves[bestIndex]=currentMove+",Right";
					bestIndex++;
				}
				else if (rightAdder==bestSum && bestSum != 0)
				{		
					bestMoves[bestIndex]=currentMove+",Right";
					bestIndex++;
				}
				else if (rightAdder<bestSum && rightAdder>secondSum)
				{		
					secondSum=rightAdder;
					Array.Clear(secondMoves,0,secondMoves.Length);
					secondIndex=0;
					secondMoves[secondIndex]=currentMove+",Right";
					secondIndex++;
				}
				else if (rightAdder==secondSum && secondSum!=0)
				{
					secondMoves[secondIndex]=currentMove+",Right";
					secondIndex++;
				}
				break;
			case("South"):
				//left
				if (newX + 1 >= 0 && newX+1<=numColumns-1) //check if there's one square to the left 
				{
					leftAdder = leftAdder + grid[newX + 1, newY, (moveNum - 1)];
				}
				if (newX + 2 >= 0 && newX+2<=numColumns-1) //check if there's two squares to the left 
				{
					leftAdder = leftAdder + grid[newX + 2, newY, (moveNum - 1)];
				}
				if (leftAdder > bestSum)
				{
					secondSum=bestSum;
					bestSum=leftAdder;
					secondIndex=bestIndex;
					bestIndex=0;
					Array.Copy(bestMoves, secondMoves, 10);
					Array.Clear(bestMoves,0,bestMoves.Length);
					bestMoves[bestIndex]=currentMove+",Left";
					bestIndex++;
				}
				else if (leftAdder==bestSum && bestSum != 0)
				{		
					bestMoves[bestIndex]=currentMove+",Left";
					bestIndex++;
				}
				else if (leftAdder<bestSum && leftAdder>secondSum)
				{		
					secondSum=leftAdder;
					Array.Clear(secondMoves,0,secondMoves.Length);
					secondIndex=0;
					secondMoves[secondIndex]=currentMove+",Left";
					secondIndex++;
				}
				else if (leftAdder==secondSum && secondSum!=0)
				{
					secondMoves[secondIndex]=currentMove+",Left";
					secondIndex++;
				}
				//right
				if ((newX - 1) >= 0 ) //check if there's one square to the right
				{
					rightAdder = rightAdder + grid[newX - 1, newY, (moveNum - 1)];
				}
				if (newX - 2 >= 0) //check if there's two squares to the right 
				{
					rightAdder = rightAdder + grid[newX - 2, newY, (moveNum - 1)];
				}
				if (rightAdder > bestSum)
				{
					secondSum=bestSum;
					bestSum=rightAdder;
					secondIndex=bestIndex;
					bestIndex=0;
					Array.Copy(bestMoves, secondMoves, 10);
					Array.Clear(bestMoves,0,bestMoves.Length);
					bestMoves[bestIndex]=currentMove+",Right";
					bestIndex++;
				}
				else if (rightAdder==bestSum && bestSum != 0)
				{		
					bestMoves[bestIndex]=currentMove+",Right";
					bestIndex++;
				}
				else if (rightAdder<bestSum && rightAdder>secondSum)
				{		
					secondSum=rightAdder;
					Array.Clear(secondMoves,0,secondMoves.Length);
					secondIndex=0;
					secondMoves[secondIndex]=currentMove+",Right";
					secondIndex++;
				}
				else if (rightAdder==secondSum && secondSum!=0)
				{
					secondMoves[secondIndex]=currentMove+",Right";
					secondIndex++;
				}
				break;
			case ("West"):
				//left
				if (newY - 1 >= 0) //check if there's one square to the left 
				{
					leftAdder = leftAdder + grid[newX, newY-1, (moveNum - 1)];
				}
				if (newY - 2 >= 0) //check if there's two squares to the left 
				{
					leftAdder = leftAdder + grid[newX, newY-2, (moveNum - 1)];
				}
				if (leftAdder > bestSum)
				{
					secondSum=bestSum;
					bestSum=leftAdder;
					secondIndex=bestIndex;
					bestIndex=0;
					Array.Copy(bestMoves, secondMoves, 10);
					Array.Clear(bestMoves,0,bestMoves.Length);
					bestMoves[bestIndex]=currentMove+",Left";
					bestIndex++;
				}
				else if (leftAdder==bestSum && bestSum != 0)
				{		
					bestMoves[bestIndex]=currentMove+",Left";
					bestIndex++;
				}
				else if (leftAdder<bestSum && leftAdder>secondSum)
				{		
					secondSum=leftAdder;
					Array.Clear(secondMoves,0,secondMoves.Length);
					secondIndex=0;
					secondMoves[secondIndex]=currentMove+",Left";
					secondIndex++;
				}
				else if (leftAdder==secondSum && secondSum!=0)
				{
					secondMoves[secondIndex]=currentMove+",Left";
					secondIndex++;
				}
				//right
				if (newY + 1 >= 0 && newY+1<=numRows-1) //check if there's one square to the right
				{
					rightAdder = rightAdder + grid[newX, newY+1, (moveNum - 1)];
				}
				if (newY + 2 >= 0 && newY+2<=numRows-1) //check if there's two squares to the right 
				{
					rightAdder = rightAdder + grid[newX, newY+2, (moveNum - 1)];
				}
				if (rightAdder > bestSum)
				{
					secondSum=bestSum;
					bestSum=rightAdder;
					secondIndex=bestIndex;
					bestIndex=0;
					Array.Copy(bestMoves, secondMoves, 10);
					Array.Clear(bestMoves,0,bestMoves.Length);
					bestMoves[bestIndex]=currentMove+",Right";
					bestIndex++;
				}
				else if (rightAdder==bestSum && bestSum != 0)
				{		
					bestMoves[bestIndex]=currentMove+",Right";
					bestIndex++;
				}
				else if (rightAdder<bestSum && rightAdder>secondSum)
				{		
					secondSum=rightAdder;
					Array.Clear(secondMoves,0,secondMoves.Length);
					secondIndex=0;
					secondMoves[secondIndex]=currentMove+",Right";
					secondIndex++;
				}
				else if (rightAdder==secondSum && secondSum!=0)
				{
					secondMoves[secondIndex]=currentMove+",Right";
					secondIndex++;
				}
				break;
			case ("East"):
				//left
				if (newY + 1 >= 0 && newY+1<=numRows-1) //check if there's one square to the left 
				{
					leftAdder = leftAdder + grid[newX, newY + 1, (moveNum - 1)];
				}
				if (newY + 2 >= 0 && newY+2<=numRows-1) //check if there's two squares to the left 
				{
					leftAdder = leftAdder + grid[newX, newY + 2, (moveNum - 1)];
				}
				if (leftAdder > bestSum)
				{
					secondSum=bestSum;
					bestSum=leftAdder;
					secondIndex=bestIndex;
					bestIndex=0;
					Array.Copy(bestMoves, secondMoves, 10);
					Array.Clear(bestMoves,0,bestMoves.Length);
					bestMoves[bestIndex]=currentMove+",Left";
					bestIndex++;
				}
				else if (leftAdder==bestSum && bestSum != 0)
				{		
					bestMoves[bestIndex]=currentMove+",Left";
					bestIndex++;
				}
				else if (leftAdder<bestSum && leftAdder>secondSum)
				{		
					secondSum=leftAdder;
					Array.Clear(secondMoves,0,secondMoves.Length);
					secondIndex=0;
					secondMoves[secondIndex]=currentMove+",Left";
					secondIndex++;
				}
				else if (leftAdder==secondSum && secondSum!=0)
				{
					secondMoves[secondIndex]=currentMove+",Left";
					secondIndex++;
				}
				//right
				if (newY - 1 >= 0) //check if there's one square to the right
				{
					rightAdder = rightAdder + grid[newX, newY - 1, (moveNum - 1)];
				}
				if (newY - 2 >= 0) //check if there's two squares to the right 
				{
					rightAdder = rightAdder + grid[newX, newY - 2, (moveNum - 1)];
				}
				if (rightAdder > bestSum)
				{
					secondSum=bestSum;
					bestSum=rightAdder;
					secondIndex=bestIndex;
					bestIndex=0;
					Array.Copy(bestMoves, secondMoves, 10);
					Array.Clear(bestMoves,0,bestMoves.Length);
					bestMoves[bestIndex]=currentMove+",Right";
					bestIndex++;
				}
				else if (rightAdder==bestSum && bestSum != 0)
				{		
					bestMoves[bestIndex]=currentMove+",Right";
					bestIndex++;
				}
				else if (rightAdder<bestSum && rightAdder>secondSum)
				{		
					secondSum=rightAdder;
					Array.Clear(secondMoves,0,secondMoves.Length);
					secondIndex=0;
					secondMoves[secondIndex]=currentMove+",Right";
					secondIndex++;
				}
				else if (rightAdder==secondSum && secondSum!=0)
				{
					secondMoves[secondIndex]=currentMove+",Right";
					secondIndex++;
				}
				break;
			}
		}
		if (bestSum == 0) { //not in range	
			shoot = "Up";
			if (dir == "North") {
				if (BoardManager.getCurrentPosX () < x && BoardManager.getCurrentPosY () > y) {
					move = "Left";
				} else if (BoardManager.getCurrentPosX () > x && BoardManager.getCurrentPosY () > y) {
					move = "Right";
				} else if (BoardManager.getCurrentPosY () > y) {
					move = "Up";
				} else {
					switch (rnd.Next (2)) {
					case 0:
						move = "Left";
						break;
					case 1:
						move = "Right";
						break;
						
					}
					
				}
				
				
			} else if (dir == "South") {
				if (BoardManager.getCurrentPosX () < x && BoardManager.getCurrentPosY () < y) {
					move = "Right";
				} else if (BoardManager.getCurrentPosX () > x && BoardManager.getCurrentPosY () < y) {
					move = "Left";
				} else if (BoardManager.getCurrentPosY () < y) {
					move = "Up";
				} else {
					switch (rnd.Next (2)) {
					case 0:
						move = "Left";
						break;
					case 1:
						move = "Right";
						break;
						
					}
					
				}
			} else if (dir == "West") {
				if (BoardManager.getCurrentPosY () < y && BoardManager.getCurrentPosX () < x) {
					move = "Left";
				} else if (BoardManager.getCurrentPosY () > y && BoardManager.getCurrentPosX () < x) {
					move = "Right";
				} else if (BoardManager.getCurrentPosX () < x) {
					move = "Up";
				} else {
					switch (rnd.Next (2)) {
					case 0:
						move = "Left";
						break;
					case 1:
						move = "Right";
						break;
						
					}
					
				}
				
			} else {  //East
				if (BoardManager.getCurrentPosY () < y && BoardManager.getCurrentPosX () > x) {
					move = "Right";
				} else if (BoardManager.getCurrentPosY () > y && BoardManager.getCurrentPosX () > x) {
					move = "Left";
				} else if (BoardManager.getCurrentPosX () > x) {
					move = "Up";
				} else {
					switch (rnd.Next (2)) {
					case 0:
						move = "Left";
						break;
					case 1:
						move = "Right";
						break;
						
					}
					
				}
				
			}
			//try to move closer, if that isn't possible, move either left or right
			
			
			
		} else if (secondSum == 0) {
			int pickIndex = rnd.Next (bestIndex);
			string[] moveResult = bestMoves [pickIndex].Split (',');
			move = moveResult [0];
			shoot = moveResult [1];
			
		} 
		else 
		{
			int whichMove=rnd.Next(100);
			if (whichMove>=bestMovePercentage)
			{
				int pickIndex = rnd.Next (secondIndex);
				string[] moveResult = secondMoves [pickIndex].Split (',');
				move = moveResult [0];
				shoot = moveResult [1];
			}
			else
			{
				int pickIndex = rnd.Next (bestIndex);
				string[] moveResult = bestMoves [pickIndex].Split (',');
				move = moveResult [0];
				shoot = moveResult [1];
			}
			
		}
		if (level == 0) {
			int shotDeleter = rnd.Next (100);
			if (shotDeleter < howEasy) {
				shoot = "Up";					//Why is shoot set to Up here. That could cause problems in the future
			}
		}
		return (move + "," + shoot);
		
		
		
	}
	int[, ,] guessMoveGrid()
	{
		int[, ,] grid = new int[numColumns, numRows, 3]; //this is what will be returned, 9 rows and 5 columns, 3 total moves, we want to check percentages for each
		
		for (int k = 0; k < numColumns; k ++) {
			for (int i = 0; i < numRows; i ++) {
				for (int j = 0; j < 3; j ++) {
					grid[k,i,j] = 0;
				}
			}
		}
		//each string within the following arrays will have the form "x,y,dir", they will contain the potential starting points for each move
		string[] possibleBeforeFirst = new string[1];
		string[] possibleBeforeSecond = new string[4]; 
		string[] possibleBeforeThird = new string[16]; 
		string[] possibleAfterThird = new string[64];
		possibleBeforeFirst [0] = (BoardManager.getCurrentPosX() + "," + BoardManager.getCurrentPosY() + "," + BoardManager.getDirection());
		
		for (int i = 0; i < 3; i++) //generates pBS, pBT, and pAT
		{
			int length;
			string[] use;
			if (i == 0)
			{
				use = new string[1];
				Array.Copy(possibleBeforeFirst, use, 1);
				length = 1;
			}
			else if (i == 1)
			{
				use = new string[4];
				Array.Copy(possibleBeforeSecond, use, 4);
				length = 4;
			}
			else
			{
				use = new string[16];
				Array.Copy(possibleBeforeThird, use, 16);
				length = 16;
			}
			int index=0;
			for (int j = 0; j < length; j++)// goes through each of the possible starting points
			{
				string[] moveListBuffer=tryFour(use[j]); //This tests each of the 4 moves from the position and direction given
				
				if (i == 0)
				{
					for (int q = 0; q < 4; q++)
					{
						possibleBeforeSecond[q + index] = moveListBuffer[q];
					}
					
				}
				if (i == 1)
				{
					for (int q = 0; q < 4; q++)
					{
						possibleBeforeThird[q + index] = moveListBuffer[q];
					}
					
				}
				else
				{
					for (int q = 0; q < 4; q++)
					{
						possibleAfterThird[q + index] = moveListBuffer[q];
					}
					
				}
				index = index + 4;
			}
			
			
		}
		// go through each of the 4 arrays and assign their respective percentages
		
		string info1 = possibleBeforeFirst[0];
		string[] splitArray1 = info1.Split(',');
		int x1 = Int32.Parse(splitArray1[0]);
		int y1 = Int32.Parse(splitArray1[1]);
		//grid[x1,y1,0] = (grid[x1,y1,0] + 100);
		
		
		for (int s = 0; s < 4; s++)
		{
			string info = possibleBeforeSecond[s];
			string[] splitArray = info.Split(',');
			int x = Int32.Parse(splitArray[0]);
			int y = Int32.Parse(splitArray[1]);
			grid[x, y, 0] = (grid[x, y, 0] + 25);
			
		}
		for (int t = 0; t < 16; t++)
		{
			string info = possibleBeforeThird[t];
			string[] splitArray = info.Split(',');
			int x = Int32.Parse(splitArray[0]);
			int y = Int32.Parse(splitArray[1]);
			grid[x, y, 1] = (grid[x, y, 1] + 6);
			
		}
		for (int u = 0; u < 64; u++)
		{
			string info = possibleAfterThird[u];
			string[] splitArray = info.Split(',');
			int x = Int32.Parse(splitArray[0]);
			int y = Int32.Parse(splitArray[1]);
			grid[x, y, 2] = (grid[x, y, 2] + 2);
			
		}
		return grid;
	}
	string[] tryFour(string state)
	{
		
		string[] moveListBuffer = new string[4];
		moveListBuffer[0] = moveResultString(state, "Stay");
		moveListBuffer[1] = moveResultString(state, "Left");
		moveListBuffer[2] = moveResultString(state, "Up");
		moveListBuffer[3] = moveResultString(state, "Right");
		return moveListBuffer;
	}
	string moveResultString(string state, string move) //gives position and dir in string form along with a move and returns a string of move and position
	{
		
		string[] splitArray = state.Split(',');
		int x = Int32.Parse(splitArray[0]);
		int y = Int32.Parse(splitArray[1]);
		string dir = splitArray[2];
		
		Vector2 pos;
		pos.x = x;
		pos.y = y;
		Vector2 resPos=BoardManager.calculatePosFrom(dir, move, pos);
		int resX = (int)resPos.x;
		int resY = (int)resPos.y;
		string resDir = BoardManager.calculateDirectionFrom(dir, move);
		return (resX + "," + resY + "," + resDir);
		
		
		
	}
	
}
