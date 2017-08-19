using UnityEngine;
using System.Collections;

public class CannonBallMove : MonoBehaviour {

	//private Rigidbody rig;
	// Use this for initialization
	void Start () {
		//rig = GetComponent<Rigidbody> ();
		StartCoroutine (shootCannon (2f, 0f));
	}
	
	IEnumerator shootCannon(float x, float y) {
		float currentX = 0;
		float step = 5f * Time.deltaTime;
		//Transform tran = new Vector3(2f, 0f, 0f);
		
		
		float startTime = Time.time;
		float journeyLength = Vector3.Distance(transform.position, new Vector3 (2f, 0f, 0f));
		float distCovered = 0f;
		//Instantiate(cannonBall, rightShootPos.position, Quaternion.identity);
		while(distCovered < journeyLength) {
			//transform.position = Vector3.Lerp(rightShootPos.position, new Vector3 (2f, 0f, 0f), (Time.time - startTime) * step);
			//cannonBall.transform.position = Vector3.MoveTowards(rightShootPos.position, new Vector3 (5f, 0f, 0f), step);
			distCovered = (Time.time - startTime) * 3f;
			float fracJourney = distCovered / journeyLength;
			transform.position = Vector3.Lerp(transform.position, new Vector3 (2f, 0f, 0f), fracJourney);
			yield return new WaitForSeconds(.01f);
		}
		
	}
}
