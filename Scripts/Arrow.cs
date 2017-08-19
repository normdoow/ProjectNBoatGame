using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {

	public Transform[] arrowLocations;
	public Transform[] touchLocations;

	// Use this for initialization
	void Start () {
		
	}
	
	public Transform[] getTouchLocations() {
		return touchLocations;
	}

	public Transform[] getArrowLocations() {
		return arrowLocations;
	}
}
