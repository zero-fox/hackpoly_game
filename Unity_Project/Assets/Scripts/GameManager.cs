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



	BladeCast.BCSender sender;

	[System.NonSerialized]
	public int victoryPointsCondition = 4;	//is actually 5 points

	public List<JSONObject> recentJsons = new List<JSONObject> ();
	static public GameManager Instance;
	public List<Player> playerList = new List<Player>();

	private GameObject gameOverCanvas;
	public Camera mainCamera;

	void Awake() {
		Instance = this;
		gameOverCanvas = GameObject.Find ("GameOverCanvas");
		mainCamera = GameObject.Find ("Main Camera").GetComponent<Camera> ();
	}


	void Start () {
		Application.runInBackground = true;
		gameOverCanvas.SetActive (false);
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

	public void sendthing() {
		sender.SendToListeners("health", "health", health, 1);
	}

	public void CheckPlayersWinCondition() {
		foreach (Player ply in playerList) {
			if (ply.myPoints >= victoryPointsCondition) {
				GameOver (ply);
			}
		}
	}

	void SpawnAPlayer0() {
		string tempPlayerId = Random.value.ToString ();
		playersRegistered.Add (tempPlayerId);

		GameObject newPlayer = Instantiate (playerPrefab) as GameObject;
		newPlayer.GetComponent<Player> ().playerId = tempPlayerId;
		playerList.Add (newPlayer.GetComponent<Player> ());

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
