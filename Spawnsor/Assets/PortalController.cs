using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PortalController : MonoBehaviour {
	public GameObject player;

	public HingeJoint door;

	void OnTriggerEnter (Collider col) {
		if (!isActiveAndEnabled) {
			return;
		}

		if (door.angle < 5) {
			return;
		}

		if (col.gameObject.Equals(player)) {
			LoadNextLevel ();
		}
	}

	public void LoadNextLevel ()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);
	}
}
