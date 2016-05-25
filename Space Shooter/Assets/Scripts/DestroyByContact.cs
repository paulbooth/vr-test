using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour {

	public GameObject explosion;
	public GameObject playerExplosion;

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
		}
	}
}
