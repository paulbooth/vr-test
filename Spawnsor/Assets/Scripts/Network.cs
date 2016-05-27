using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SocketIO;

public class Network : MonoBehaviour {

	public GameController gc;
	private SocketIOComponent socket;

	// Use this for initialization
	void Start ()
	{
		socket = GetComponent<SocketIOComponent> ();
		if (socket) {
			socket.On ("open", OnConnected);
			socket.On ("zombie", OnZombie);
			socket.On ("spawn", OnSpawn);
			socket.Connect ();
		}
	}

	void OnConnected(SocketIOEvent e)
	{
		Debug.Log ("Connected");
//		socket.Emit ("move");
	}

	void OnZombie(SocketIOEvent e)
	{
		gc.SpawnZombieWaves ( (int) e.data ["number"].n, 0, 0.5f, 0, 1);
	}

	void OnSpawn(SocketIOEvent e)
	{
		List<JSONObject> color = e.data ["color"].list;
		Color spawnColor = new Color (color[0].n, color[1].n, color[2].n);
		Debug.Log (e.data ["color"]);
		Debug.Log (color[0]);
		Debug.Log (color[1]);
		Debug.Log (color[2]);
		gc.SpawnDrawnObject (e.data ["name"].str, spawnColor);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
