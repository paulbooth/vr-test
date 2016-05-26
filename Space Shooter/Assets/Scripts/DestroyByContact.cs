using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour {

	public GameObject explosion;
	public GameObject playerExplosion;
	public int scoreValue;

	private GameController gameController;

	void Start ()
	{
		GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent<GameController> ();
		}
		if (gameController == null) {
			Debug.Log ("Cannot find GameController script.");
		}
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.CompareTag("Boundary")) {
			return;
		}
		Destroy (col.gameObject);
		Destroy (gameObject); 
		Instantiate (explosion, transform.position, transform.rotation);
		if (col.CompareTag ("Player")) {
			Instantiate (playerExplosion, col.transform.position, col.transform.rotation);
			gameController.GameOver ();
		} else {
			gameController.AddScore (scoreValue);
		}
	}
}
