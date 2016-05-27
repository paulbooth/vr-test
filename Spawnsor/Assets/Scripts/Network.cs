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
		socket.On ("open", OnConnected);
		socket.On ("zombie", OnZombie);
		socket.Connect ();
	}

	void OnConnected(SocketIOEvent e)
	{
		Debug.Log ("Connected");
//		socket.Emit ("move");
	}

	void OnZombie(SocketIOEvent e)
	{
		float number = 1;
		float.TryParse (e.data ["number"].str, out number);
		gc.SpawnZombieWaves ( (int) number, 0, 0.5f, 0, 1);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
