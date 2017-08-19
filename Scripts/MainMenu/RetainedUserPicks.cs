using UnityEngine;
using System.Collections;

/**
 * Used just to pass information from one scene to the next
 */

public class RetainedUserPicks {

	private bool isMultiplayerGame;
	private static RetainedUserPicks _instance = null;
	
	private RetainedUserPicks() {
		// Anything to init would go here
	}
	
	public static RetainedUserPicks Instance {
		get {
			if (_instance == null) {
				_instance = new RetainedUserPicks();
			}
			return _instance;
		}
	}

	public void setIsMultiplayerGame(bool isMult) {
		isMultiplayerGame = isMult;
	}

	public bool getIsMultiplayerGame() {
		return isMultiplayerGame;
	}
	
}
