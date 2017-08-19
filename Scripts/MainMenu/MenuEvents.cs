using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuEvents : MonoBehaviour {

	public Button signOutButt;

	// Use this for initialization
	void Start () {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;					//never sleep
		MultiplayerController.Instance.TrySilentSignIn();				//try to sign you in silently so you don't have to later

	}
	
	// Update is called once per frame
	void Update () {
		//if the player is authenticated
		if (MultiplayerController.Instance.IsAuthenticated ()) {
			signOutButt.gameObject.SetActive (true);
		} else {				//if they are not than don't show the button
			signOutButt.gameObject.SetActive(false);
		}
	}

	//event for the single player button being pressed
	public void singlePlayerButtonPressed() {
		Debug.Log("Entered the Battle Scene");
		Application.LoadLevel ("BattleScene");
	}

	//event for the multipleplayer button being pressed
	public void multiplePlayerButtonPressed() {
		MultiplayerController.Instance.SignInAndStartMPGame();			//signs in and starts the multiplayer game
	}

	//event for when the sign out button is pressed
	public void signOutButtonPressed() {
		MultiplayerController.Instance.SignOut();						//signs you out of the Google account
	}
}
