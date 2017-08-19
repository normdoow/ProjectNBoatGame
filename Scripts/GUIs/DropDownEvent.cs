using UnityEngine;
using System.Collections;

public class DropDownEvent : MonoBehaviour {

	public int whichRow = 0;
	public GameObject player;
	private GoEvent goEvent;

	void Start() {
		goEvent = player.GetComponent<GoEvent>();
	}

	public void dropDownChanged(int buttonNum) {
		if (buttonNum == 0) {
			goEvent.setDropDownMoves(whichRow, "Left");
		} else if (buttonNum == 1) {
			goEvent.setDropDownMoves(whichRow, "Up");
		} else if (buttonNum == 2) {
			goEvent.setDropDownMoves(whichRow, "Right");
		} else if (buttonNum == 3) {
			goEvent.setDropDownMoves(whichRow, "Stay");
		}
	}
}
