using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BladeCast;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	[System.NonSerialized]
	public List<string> playersRegistered = new List<string>();
	public GameObject playerPrefab;
	public GameObject disablePowerupPrefab;
	public GameObject replayAttackPrefab;

	public bool isSavingJson = false;

	BCListener.Listener movementListener;
	BCListener.Listener jumpingListener;
	public string theNonPlayerId;

	BladeCast.BCSender sender;

	[System.NonSerialized]
	public int victoryPointsCondition = 4;	//is actually 5 points

	public List<JSONObject> recentJsons = new List<JSONObject> ();
	static public GameManager Instance;
	public List<Player> playerList = new List<Player>();

	private GameObject gameOverCanvas;
	public Camera mainCamera;
	public GameObject floor1Obj;
	public GameObject floor2Obj;

	void Awake() {
		Instance = this;
		gameOverCanvas = GameObject.Find ("GameOverCanvas");
		mainCamera = GameObject.Find ("Main Camera").GetComponent<Camera> ();

		floor1Obj = GameObject.Find ("Floor1");
		floor2Obj = GameObject.Find ("Floor2");
	}

	public bool powerUpInPlay = false;


	void Start () {
		Application.runInBackground = true;
		gameOverCanvas.SetActive (false);
		//how to do stuffff


		BladeCast.BCListener listener = GetComponent<BladeCast.BCListener>();
		sender = GetComponent<BladeCast.BCSender>();

		//sender.SendToListeners("disabled", "playerId", "ten", 1);	//immune player


		listener.Add ("connect", 0, "SpawnAPlayer");

		movementListener = new BCListener.Listener ("movement", 0, "SaveJson");
		listener.Add (movementListener);
		//listener.Add("movement", 0, "Move");

		jumpingListener = new BCListener.Listener ("jump", 0, "SaveJson");
		//listener.Add("jump", 0, "Jump");
		listener.Add(jumpingListener);

		StartCoroutine (SpawnPowerUpsForever ());

	}

	public IEnumerator SpawnPowerUpsForever() {
		while (true) {
			Debug.Log ("spawning power up");
			yield return new WaitForSeconds (15.0f);
			if (!powerUpInPlay)
				SpawnRandomPowerUp ();
		}
	}


	void Update() {
		if (Input.GetKeyDown (KeyCode.Space)) {
			SpawnAPlayer0 ();
		}

	}


	public void ExecuteReplayPowerUp(Player ply) {
		ply.myPoints++;
		ply.UpdateHHOnPoints ();
		isSavingJson = true;
		theNonPlayerId = ply.playerId;
		recentJsons.Clear ();
	}

	public IEnumerator ExecuteDisablePlayersPowerUp(Player play) {
		play.myPoints++;
		play.UpdateHHOnPoints ();
		theNonPlayerId = play.playerId;

		Debug.Log ("the players id: " + theNonPlayerId);
		//disable players for a little
		sender.SendToListeners("disabled", "playerId", theNonPlayerId, 1);	//immune player


		foreach (Player ply in playerList) {
			if (ply.playerId != theNonPlayerId) {
				ply.listener.Remove (ply.movementListener);
				ply.listener.Remove (ply.jumpingListener);
			}
		}

		//disables for 5 seconds
		Debug.LogError("disabled");
		yield return new WaitForSeconds (5.0f);
		Debug.LogError ("should be enabled");

		//send re-enable message to players
		sender.SendToListeners ("enabled", "playerId", theNonPlayerId, 1);
		foreach (Player ply in playerList) {
			Debug.Log ("trying to add listeners");
			if (ply.playerId != theNonPlayerId) {
				ply.listener.Add (ply.movementListener);
				ply.listener.Add (ply.jumpingListener);
			}
		}
		yield return new WaitForSeconds (0.1f);
	}



	public void CheckPlayersWinCondition() {
		foreach (Player ply in playerList) {
			if (ply.myPoints >= victoryPointsCondition) {
				GameOver (ply);
			}
		}
	}

	public void SpawnRandomPowerUp() {
		powerUpInPlay = true;
		Debug.Log ("actually doing power up");
		//change this
		if (Random.value > 0.90f) {
			GameObject power = Instantiate (disablePowerupPrefab) as GameObject;
			power.transform.position = floor1Obj.transform.position + Vector3.up * 2.0f;
		} else {
			GameObject replayThing = Instantiate (replayAttackPrefab) as GameObject;
			replayThing.transform.position = floor2Obj.transform.position + Vector3.up * 2.0f;
		}

	}

	void SpawnAPlayer0() {
		string tempPlayerId = Random.value.ToString ();
		playersRegistered.Add (tempPlayerId);

		GameObject newPlayer = Instantiate (playerPrefab) as GameObject;
		newPlayer.GetComponent<Player> ().playerId = tempPlayerId;
		playerList.Add (newPlayer.GetComponent<Player> ());

	}

	void SaveJson(ControllerMessage msg) {
		if (isSavingJson)
			recentJsons.Add (msg.Payload);
		if (recentJsons.Count > 500) {
			isSavingJson = false;
			StartCoroutine (ReplayToPlayer ());
		}
	}

	public IEnumerator ReplayToPlayer() {
		//disable players for a little
		sender.SendToListeners("disabled", "playerId", theNonPlayerId, 1);	//immune player


		foreach (Player ply in playerList) {
			if (ply.playerId != theNonPlayerId) {
				ply.listener.Remove (ply.movementListener);
				ply.listener.Remove (ply.jumpingListener);
			}
		}


		foreach(JSONObject jo in recentJsons) {
			foreach (Player ply in playerList) {
				if (ply.playerId != theNonPlayerId) {
					ControllerMessage cm = new ControllerMessage (1, 0, jo.GetField ("type").str, jo);
					if (jo.GetField ("type").str == "move") {
						ply.Move (cm);
					} else if (jo.GetField ("type").str == "jump") {
						ply.Jump (cm);
					}
				}
			}
			yield return new WaitForSeconds(0.01f);
		}

		//send re-enable message to players
		sender.SendToListeners ("enabled", "playerId", theNonPlayerId, 1);
		foreach (Player ply in playerList) {
			if (ply.playerId != theNonPlayerId) {
				ply.listener.Add (ply.movementListener);
				ply.listener.Add (ply.jumpingListener);
			}
		}


	}
	
	void SpawnAPlayer(ControllerMessage msg) {
		Debug.Log ("connect detected by game manager, its controller source is: " + msg.ControllerSource.ToString());
		if (msg.ControllerSource == Player.playerIndex) {
			Debug.Log ("thign");
			if (msg.Payload.HasField ("playerId") && !playersRegistered.Contains(msg.Payload.GetField ("playerId").str)) {
				playersRegistered.Add (msg.Payload.GetField ("playerId").str);

				GameObject newPlayer = Instantiate (playerPrefab) as GameObject;
				newPlayer.GetComponent<Player> ().playerId = msg.Payload.GetField ("playerId").str;
				newPlayer.name = msg.Payload.GetField ("playerName").str;
				newPlayer.transform.GetChild (0).GetChild (0).GetComponent<Text> ().text = newPlayer.name;
				playerList.Add (newPlayer.GetComponent<Player> ());
			}
		}
	}

	void GameOver(Player winnerPlayer) {
		gameOverCanvas.SetActive (true);

		string buildGameOverText = "GameOver! \n";
		buildGameOverText = buildGameOverText + winnerPlayer.name + " has won the game!";
		gameOverCanvas.transform.GetChild(0).gameObject.GetComponent<Text>().text = buildGameOverText;
	}
}
