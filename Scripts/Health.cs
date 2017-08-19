using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GooglePlayGames.BasicApi.Multiplayer;		//included for multiplayer api
using System.Collections.Generic;       //Allows us to use Lists.

//Both The Player and enemy will have this class
//so we have to set static variables in GameManager
//and then check if they are both done
public class Health : MonoBehaviour {
	public int startingHealth = 3;            // The amount of health the enemy starts the game with.
	public int currentHealth;                   // The current health the enemy has.
	public Slider healthSlider1;					//slider to show off how much health the player has
	public Slider healthSlider2;
	private Slider realSlider;
	//public float sinkSpeed = 2.5f;              // The speed at which the enemy sinks through the floor when dead.
	//public int scoreValue = 10;                 // The amount added to the player's score when the enemy dies.
	//public AudioClip deathClip;                 // The sound to play when the enemy dies.
	
	
	//	Animator anim;                              // Reference to the animator.
	//	AudioSource enemyAudio;                     // Reference to the audio source.
	//	ParticleSystem hitParticles;                // Reference to the particle system that plays when the enemy is damaged.
	//	CapsuleCollider capsuleCollider;            // Reference to the capsule collider.
	bool isDead;                                // Whether the enemy is dead.
	//bool isSinking;                             // Whether the enemy has started sinking through the floor.
	
	
	void Awake () {
		// Setting up the references.
		//		anim = GetComponent <Animator> ();
		//		enemyAudio = GetComponent <AudioSource> ();
		//		hitParticles = GetComponentInChildren <ParticleSystem> ();
		//		capsuleCollider = GetComponent <CapsuleCollider> ();
		
		// Setting the current health when the enemy first spawns.
		currentHealth = startingHealth;
		setPlayerSlider();
	}

	void setPlayerSlider() {
		//if it is a multiplayer game
		if (RetainedUserPicks.Instance.getIsMultiplayerGame ()) {
			string _myParticipantId = MultiplayerController.Instance.GetMyParticipantId ();
			List<Participant> allPlayers = MultiplayerController.Instance.GetAllPlayers ();
			if (string.Compare (_myParticipantId, allPlayers [0].ParticipantId) == 0 
			    							&& gameObject.tag == "Player") {		//you are the first player
				realSlider = healthSlider1;
			} else if (string.Compare (_myParticipantId, allPlayers [0].ParticipantId) != 0 
			           && gameObject.tag == "Enemy") {		//you are the second player and the enemy controller
				realSlider = healthSlider1;
			} else {
				realSlider = healthSlider2;
			}
		} else {				//if it is not a multiplayer game
			if(gameObject.tag == "Player") {			//this is the player controller
				realSlider = healthSlider1;
			} else {
				realSlider = healthSlider2;
			}
		}
	}

	void Update () {
		// If the enemy should be sinking...
//		if(isSinking) {
//			// ... move the enemy down by the sinkSpeed per second.
//			//transform.Translate (-Vector3.up * sinkSpeed * Time.deltaTime);
//		}
	}
	
	
	public void TakeDamage (int amount, Vector3 hitPoint) {
		// If the enemy is dead...
		if (isDead) {
			// ... no need to take damage so exit the function.
			return;
		}
		// Play the hurt sound effect.
		//		enemyAudio.Play ();
		
		// Reduce the current health by the amount of damage sustained.
		currentHealth -= amount;
		realSlider.value = currentHealth;

		//print (gameObject.tag + " health is " + currentHealth);
		// Set the position of the particle system to where the hit was sustained.
		//		hitParticles.transform.position = hitPoint;
		//		
		//		// And play the particles.
		//		hitParticles.Play();
		
		// If the current health is less than or equal to zero...
		if(currentHealth <= 0) {
			// ... the enemy is dead.
			Death ();
		}

		//Check if there was a victory
		GameManager.singleton.checkForWin();
	}
	
	void Death () {
		// The enemy is dead.
		isDead = true;
		if (gameObject.tag == "Player") {
			GameManager.singleton.setEnemyWon(true);			//set the static variable from GameManager
		} else {
			GameManager.singleton.setPlayerWon(true);
		}

		//Application.LoadLevel(Application.loadedLevel);
		// Turn the collider into a trigger so shots can pass through it.
		//		capsuleCollider.isTrigger = true;
		//		
		//		// Tell the animator that the enemy is dead.
		//		anim.SetTrigger ("Dead");
		//		
		//		// Change the audio clip of the audio source to the death clip and play it (this will stop the hurt clip playing).
		//		enemyAudio.clip = deathClip;
		//		enemyAudio.Play ();
	}
	
	
//	public void StartSinking ()
//	{
//		// Find and disable the Nav Mesh Agent.
//		GetComponent <NavMeshAgent> ().enabled = false;
//		
//		// Find the rigidbody component and make it kinematic (since we use Translate to sink the enemy).
//		GetComponent <Rigidbody> ().isKinematic = true;
//		
//		// The enemy should no sink.
//		isSinking = true;
//		
//		// Increase the score by the enemy's score value.
//		//ScoreManager.score += scoreValue;
//		
//		// After 2 seconds destory the enemy.
//		Destroy (gameObject, 2f);
//	}
}