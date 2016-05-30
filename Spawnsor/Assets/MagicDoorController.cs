using UnityEngine;
using System.Collections;

public class MagicDoorController : MonoBehaviour {

	public GameObject portal;

	void Start() {
		Disable ();
	}

	public void Enable () {
		portal.SetActive (true);
		Debug.Log ("Enabled a door");
	}

	public void Disable() {
		portal.SetActive (false);
	}
}
