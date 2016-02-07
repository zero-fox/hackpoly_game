using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BladeCast;

public class GameManager : MonoBehaviour {

	[System.NonSerialized]
	public List<string> playersRegistered = new List<string>();
	public GameObject playerPrefab;
	BladeCast.BCSender sender;

	public List<JSONObject> recentJsons = new List<JSONObject> ();
	GameManager Instance;

	void Awake() {
		Instance = this;
	}


	void Start () {
		Application.runInBackground = true;
		//how to do stuffff


		BladeCast.BCListener listener = GetComponent<BladeCast.BCListener>();
		sender = GetComponent<BladeCast.BCSender>();
		listener.Add ("connect", 0, "SpawnAPlayer");

	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.Space)) {
			SpawnAPlayer0 ();
		}

	}

	void SpawnAPlayer0() {
		string tempPlayerId = Random.value.ToString ();
		playersRegistered.Add (tempPlayerId);

		GameObject newPlayer = Instantiate (playerPrefab) as GameObject;
		newPlayer.GetComponent<Player> ().playerId = tempPlayerId;

	}
	
	void SpawnAPlayer(ControllerMessage msg) {
		Debug.Log ("connect detected by game manager, its controller source is: " + msg.ControllerSource.ToString());
		if (msg.ControllerSource == Player.playerIndex) {
			Debug.Log ("thign");
			if (msg.Payload.HasField ("playerId") && !playersRegistered.Contains(msg.Payload.GetField ("playerId").str)) {
				playersRegistered.Add (msg.Payload.GetField ("playerId").str);

				GameObject newPlayer = Instantiate (playerPrefab) as GameObject;
				newPlayer.GetComponent<Player> ().playerId = msg.Payload.GetField ("playerId").str;
			}
		}
	}
}
