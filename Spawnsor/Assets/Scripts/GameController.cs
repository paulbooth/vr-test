using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour {

	public GameObject zombie;
	public GameObject cube;
	public GameObject sphere;
	public GameObject playerObjectExplosion;
	public float playerObjectShrinkTime = 5f;

	public Transform zombieTarget;
	public float wallLength = 10;
	public float spawnHeight = 1;

	public int zombieCount;
	public float spawnWait;
	public float startWait;
	public float waveWait;

	private int score;
	private bool gameOver = true;

	public void AddScore (int newScoreValue)
	{
		score += newScoreValue;
//		UpdateScore ();
	}

	public void GameOver ()
	{
		gameOver = true;
//		gameOverText.text = "Game Over!";
	}

	// Use this for initialization
	void Start () {
//		SpawnZombieWaves (zombieCount, startWait, spawnWait, waveWait, 0);
	}

	// Update is called once per frame
	void Update () {
		if (gameOver) {
			if (Input.GetKeyDown (KeyCode.R)) {
				SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
			}
		}
	}

	public void SpawnZombieWaves (int zombieCount, float startWait, float spawnWait, float waveWait, int numWaves)
	{
		StartCoroutine (SpawnZombieWavesRoutine (zombieCount, startWait, spawnWait, waveWait, numWaves));
	}

	IEnumerator SpawnZombieWavesRoutine (int zombieCount, float startWait, float spawnWait, float waveWait, int numWaves)
	{
		yield return new WaitForSeconds (startWait);
		int waveCount = 0;
		while (numWaves == 0 || waveCount < numWaves) {
			waveCount++;
			for (int i = 0; i < zombieCount; i++) {
				SpawnZombie ();
				yield return new WaitForSeconds (spawnWait);
			}
			yield return new WaitForSeconds (waveWait);
		}
	}

	public void SpawnZombie() {
		bool xDir = Random.value > 0.5f;
		float wallDistance = Random.value > 0.5f ? wallLength: -wallLength;
		float wallPosition = Random.Range (-wallLength, wallLength);
		Vector3 spawnPosition = new Vector3 (
			xDir ? wallPosition : wallDistance,
			spawnHeight,
			xDir ? wallDistance : wallPosition
		);
		GameObject zombieClone = Instantiate (zombie, spawnPosition, Quaternion.identity) as GameObject;
		zombieClone.GetComponent<NavigateObject> ().tracked = zombieTarget;
	}

	private Vector3 getPlayerSpawnObjectPosition()
	{
		return zombieTarget.position + Vector3.up * 2;
	}

	public void SpawnCube (Color color)
	{
		GameObject cubeClone = SpawnGameObject (cube, color);
	}

	public void SpawnSphere (Color color)
	{
		GameObject sphereClone = SpawnGameObject (sphere, color);
	}

	public GameObject SpawnGameObject(GameObject go, Color color)
	{
		GameObject clone = Instantiate (go, getPlayerSpawnObjectPosition (), Quaternion.identity) as GameObject;
		ShrinkAndDestroy sad = clone.AddComponent<ShrinkAndDestroy> ();
		sad.delayTime = playerObjectShrinkTime;
		sad.explosion = playerObjectExplosion;

		MeshRenderer renderer = clone.GetComponent<MeshRenderer>();
		renderer.material.color = color;

		return clone;
	}

	public void SpawnDrawnObject(string name, Color color)
	{
		switch (name.ToLower ()) {
		case "square":
			SpawnCube (color);
			break;
		case "circle":
			SpawnSphere (color);
			break;
		default:
			Debug.Log ("Unknown drawn object: " + name);
			break;
		}
	}
}
