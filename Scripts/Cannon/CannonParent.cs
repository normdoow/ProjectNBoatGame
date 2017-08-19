using UnityEngine;
using System.Collections;

public class CannonParent : MonoBehaviour {

	public int damagePerShot = 35;                  // The damage inflicted by each bullet.
	public float range = 2f;                      // The distance the gun can fire.
	//info of the cannonballl
	public GameObject cannonBall;
	public float speed = 4f;
	public Transform rightShootPos;
	public Transform leftShootPos;
	
	//float timer;                                    // A timer to determine when to fire.
	Ray shootRay;                                   // A ray from the gun end forwards.
	RaycastHit shootHit;                            // A raycast hit to get information about what was hit.
	int shootableMask;                              // A layer mask so the raycast only hits things on the shootable layer.
//	ParticleSystem gunParticles;                    // Reference to the particle system.
//	LineRenderer gunLine;                           // Reference to the line renderer.
//	AudioSource gunAudio;                           // Reference to the audio source.
//	Light gunLight;                                 // Reference to the light component.
//	float effectsDisplayTime = 0.2f;                // The proportion of the timeBetweenBullets that the effects will display for.
	
	void Awake () {
		// Create a layer mask for the Shootable layer.
		shootableMask = LayerMask.GetMask ("Default");
		// Set up the references.
//		gunParticles = GetComponent<ParticleSystem> ();
//		gunLine = GetComponent <LineRenderer> ();
//		gunAudio = GetComponent<AudioSource> ();
//		gunLight = GetComponent<Light> ();
	}

	float startTime;
	float journeyLength;
	float distCovered;
	void Start() {
		//Instantiate(cannonBall, rightShootPos.position, Quaternion.identity);
		//ballRigidBody = cannonBall.GetComponent<Rigidbody>();
		startTime = Time.time;
		journeyLength = Vector3.Distance(rightShootPos.position, new Vector3 (2f, 0f, 0f));
		distCovered = 0f;
	}
	float x = 0f;
	float y = 0f;
	void Update () {
		
		// If the Fire1 button is being press and it's time to fire...
//		if(Input.GetButton ("Fire1") && timer >= timeBetweenBullets)
//		{
//			// ... shoot the gun.
//			shootCannon("Right");
//		}
		
		// If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
//		if(timer >= timeBetweenBullets * effectsDisplayTime)
//		{
//			// ... disable the effects.
//			DisableEffects ();
//		}


//		x += 0.01f;
//		float step = 5f * Time.deltaTime;
//		if(x < 100f) {
//			transform.position = Vector3.MoveTowards (cannonBall.transform.position, new Vector3 (2f, y, 0f), step);
//		}

	}

//	void FixedUpdate() {
//		Vector3 currentPosition = Vector3.Lerp (rightShootPos.position, new Vector3 (2f, 0f, 0f), Time.time / 1f);
//		rigidbody.MovePosition(new Vector3 (2f, 0f, 0f));
//
//	}
	
	public void DisableEffects ()
	{
		// Disable the line renderer and the light.
//		gunLine.enabled = false;
//		gunLight.enabled = false;
	}
	
	public void shootCannon (string direction, string shipDirection) {

		// Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
		shootRay.origin = transform.position;
		if (direction == "Right") {
			shootRay.direction = transform.right;
		} else if (direction == "Left") {
			shootRay.direction = -transform.right;
		}

		if (gameObject.tag == "Player") {
			Debug.DrawRay (transform.position, shootRay.direction * range, Color.green, 4);
		} else {
			Debug.DrawRay (transform.position, shootRay.direction * range, Color.cyan, 4);
		}
		// Perform the raycast against gameobjects on the shootable layer and if it hits something...
		if(Physics.Raycast (shootRay, out shootHit, range)) {
			// Try and find an EnemyHealth script on the gameobject hit.
			Health enemyHealth = shootHit.collider.GetComponent <Health>();
			// If the EnemyHealth component exist...
			if(enemyHealth != null) {
				// ... the enemy should take damage.
				enemyHealth.TakeDamage (damagePerShot, shootHit.point);
				//print ("The enemy took damage!");
			}
			
			// Set the second position of the line renderer to the point the raycast hit.
//			gunLine.SetPosition (1, shootHit.point);
		}
		// If the raycast didn't hit anything on the shootable layer...
		else {
			// ... set the second position of the line renderer to the fullest extent of the gun's range.
//			gunLine.SetPosition (1, shootRay.origin + shootRay.direction * range);
		}

		if (direction == "Right") {
			//shoot a projectile
			StartCoroutine(shootCannon(2f, 0f, rightShootPos, shipDirection));
		} else if (direction == "Left") {
			//shoot a projectile
			StartCoroutine(shootCannon(-2f, 0f, leftShootPos, shipDirection));
//			Rigidbody shot = Instantiate(ballRigidBody, leftShootPos.position, leftShootPos.rotation) as Rigidbody;
//			shot.AddForce(-leftShootPos.right * shotForce);
		}

	}

	IEnumerator shootCannon(float x, float y, Transform tran, string shipDirection) {

		GameObject ball = Instantiate(cannonBall, tran.position, Quaternion.identity) as GameObject;

		Vector3 newPos = getShotDestination(x, y, shipDirection);
		float startTime = Time.time;
		float journeyLength = Vector3.Distance(tran.position, newPos);
		float distCovered = 0f;
		while(distCovered < journeyLength) {
			distCovered = (Time.time - startTime) * speed;
			float fracJourney = distCovered / journeyLength;
			ball.transform.position = Vector3.Lerp(tran.position, newPos, fracJourney);
			yield return new WaitForSeconds(.01f);
		}
		Destroy(ball);
	}

	Vector3 getShotDestination(float x, float y, string shipDirection) {
		Vector3 returnedVector = new Vector3(0f, 0f, 0f);
		if (shipDirection == "North") {		//if pointing up
			returnedVector = new Vector3(transform.position.x + x, transform.position.y + y, 0f);
		} else if (shipDirection == "South") {	//pointing down
			returnedVector = new Vector3(transform.position.x - x, transform.position.y - y, 0f);
		} else if (shipDirection == "West") {			//pointing right
			returnedVector = new Vector3(transform.position.x + y, transform.position.y + x, 0f);
		} else if (shipDirection == "East") {			//pointing left
			returnedVector = new Vector3(transform.position.x - y, transform.position.y - x, 0f);
		}
		return returnedVector;
	}
}
