using UnityEngine;
using System.Collections;
using SocketIO;

public class Network : MonoBehaviour {

	public GameObject cube;
	private SocketIOComponent socket;

	// Use this for initialization
	void Start ()
	{
		socket = GetComponent<SocketIOComponent> ();
		socket.On ("open", OnConnected);
		socket.On ("cube", OnCubed);
	}

	void OnConnected(SocketIOEvent e)
	{
		Debug.Log ("Connected");
//		socket.Emit ("move");
	}

	void OnCubed(SocketIOEvent e)
	{
		Debug.Log ("cubed.");
		Debug.Log (e.data["size"]);
		Debug.Log (e.data["size"].type);
		GameObject newCube = Instantiate (cube);
		float size = 1;
		float.TryParse (e.data ["size"].str, out size);
		newCube.transform.localScale = new Vector3 (size, size, size);
	}
	// Update is called once per frame
	void Update () {
	
	}
}
