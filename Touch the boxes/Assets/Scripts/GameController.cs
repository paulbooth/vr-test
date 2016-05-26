using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public GameObject box;
	public Vector3 spawnValue;
	public int boxCount;
	public float spawnWait;
	public float startWait;
	public float waveWait;
	private int score;

	public void MakeBoxes () {

	}

	// Use this for initialization
	void Start () {
//		StartCoroutine (SpawnBoxWaves ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator SpawnBoxWaves ()
	{
		yield return new WaitForSeconds (startWait);
		while (true) {
			for (int i = 0; i < boxCount; i++) {
				SpawnBox ();
				yield return new WaitForSeconds (spawnWait);
			}
			yield return new WaitForSeconds (waveWait);
		}
	}

	private void SpawnBox() {
		Vector3 spawnPosition = new Vector3 (
			Random.Range(-spawnValue.x, spawnValue.x),
			spawnValue.y,
			Random.Range(-spawnValue.z, spawnValue.z)
		);
		Instantiate (box, spawnPosition, Quaternion.identity);
	}
}
