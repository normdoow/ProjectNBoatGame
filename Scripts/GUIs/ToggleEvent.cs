using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ToggleEvent : MonoBehaviour {

	public int whichRow = 0;				//var for the row of toggle this comes from
	public string whichDirection = "Stay";			//var for left if 0 and right if 1
	public GameObject player;
	private GoEvent goEvent;

	void Start() {
		goEvent = player.GetComponent<GoEvent>();
	}

	public void toggleChanged(bool isOn) {
		if (isOn) {
			goEvent.setToggleMoves(whichRow, whichDirection);
		} else if (!isOn){
			goEvent.setToggleMoves(whichRow, "Stay");
		}
	}
}
