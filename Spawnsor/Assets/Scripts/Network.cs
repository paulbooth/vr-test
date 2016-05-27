using UnityEngine;
using System.Collections;
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
		gc.SpawnDrawnObject (e.data ["name"].str);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
