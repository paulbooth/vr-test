using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PortalController : MonoBehaviour {
	public GameObject player;

	public HingeJoint door;
	public int numLevels = 2;

	private bool loading = false;

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
		if (loading) {
			return;
		}
		loading = true;
		int nextLevel = SceneManager.GetActiveScene ().buildIndex + 1;
		if (nextLevel < numLevels) {
			SceneManager.LoadScene (nextLevel);
		}
	}

	IEnumerator AsynchronousLoad (int sceneIndex)
	{
		yield return null;

		AsyncOperation ao = SceneManager.LoadSceneAsync(sceneIndex);
		ao.allowSceneActivation = false;

		while (! ao.isDone)
		{
			// [0, 0.9] > [0, 1]
			float progress = Mathf.Clamp01(ao.progress / 0.9f);
			Debug.Log("Loading progress: " + (progress * 100) + "%");

			// Loading completed
			if (ao.progress == 0.9f)
			{
				Debug.Log("Press a key to start");
				if (Input.anyKey)
					ao.allowSceneActivation = true;
			}

			yield return null;
		}
	}
}
