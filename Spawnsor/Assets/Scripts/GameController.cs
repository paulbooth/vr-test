﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour {

	public GameObject zombie;
	public Transform target;
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
		StartCoroutine (SpawnZombieWaves ());

	}

	// Update is called once per frame
	void Update () {
		if (gameOver) {
			if (Input.GetKeyDown (KeyCode.R)) {
				SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
			}
		}
	}

	IEnumerator SpawnZombieWaves ()
	{
		yield return new WaitForSeconds (startWait);
		while (true) {
			for (int i = 0; i < zombieCount; i++) {
				SpawnZombie ();
				yield return new WaitForSeconds (spawnWait);
			}
			yield return new WaitForSeconds (waveWait);
		}
	}

	private void SpawnZombie() {
		Debug.Log ("Spawning a zombie");
		bool xDir = Random.value > 0.5f;
		float wallDistance = Random.value > 0.5f ? wallLength: -wallLength;
		float wallPosition = Random.Range (-wallLength, wallLength);
		Vector3 spawnPosition = new Vector3 (
			xDir ? wallPosition : wallDistance,
			spawnHeight,
			xDir ? wallDistance : wallPosition
		);
		GameObject zombieClone = Instantiate (zombie, spawnPosition, Quaternion.identity) as GameObject;
		zombieClone.GetComponent<NavigateObject> ().tracked = target;
	}
}
