using UnityEngine;
using System.Collections;

public class replayPowerUp : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			//calls game manager with the player that collected it
			//game manager waits until sufficient json has been collected from that player 
			//(makes a delegate and counts somehow)
			//after sufficient passed, send messages to all handhelds with replay string
			//on HH: if string is not player name, accept replays
			//game manager: when all json is replayed, enable HH again
			

			Destroy (gameObject);	//clean this up
		}
	}
}
