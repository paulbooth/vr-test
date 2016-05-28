using UnityEngine;
using System.Collections;
using SocketIO;

public class Network : MonoBehaviour {

	public SpawnController spawnController;
	private SocketIOComponent socket;

	// Use this for initialization
	void Start ()
	{
		socket = GetComponent<SocketIOComponent> ();
		if (socket) {
			socket.On ("open", OnConnected);
			socket.On ("zombie", OnZombie);
			socket.On ("spawn", OnSpawn);
			socket.On ("spawn custom", OnSpawnCustom);
			socket.Connect ();
		}
	}

	void OnConnected(SocketIOEvent e)
	{
		Debug.Log ("Connected");
	}

	void OnZombie(SocketIOEvent e)
	{
		spawnController.SpawnZombieWaves ( (int) e.data ["number"].n, 0, 0.5f, 0, 1);
	}

	void OnSpawn(SocketIOEvent e)
	{
		spawnController.SpawnDrawnObject (e.data);
	}

	void OnSpawnCustom(SocketIOEvent e)
	{
		spawnController.SpawnCustomObject (e.data);
	}
}
