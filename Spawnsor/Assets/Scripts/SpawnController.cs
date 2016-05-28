using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnController : MonoBehaviour {

	public GameObject zombie;
	public GameObject cube;
	public GameObject sphere;
	public GameObject gun;
	public GameObject playerObjectExplosion;
	public float playerObjectShrinkTime = 5f;

	public Transform zombieTarget;
	public float wallLength = 10;
	public float spawnHeight = 1;

	public int zombieCount = 5;
	public float spawnWait = 0.5f;
	public float startWait = 1f;
	public float waveWait = 4f;

	public void Start ()
	{
		if (zombieCount > 0) {
			SpawnZombieWaves (zombieCount, startWait, spawnWait, waveWait, 0);
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

	public GameObject SpawnKnownGameObject(GameObject go, Color color, float sizeX, float sizeY, float explodeTime)
	{
		GameObject clone = Instantiate (go, getPlayerSpawnObjectPosition (), Quaternion.identity) as GameObject;
		clone.transform.localScale = Vector3.Scale(clone.transform.localScale, new Vector3(sizeX, sizeY, sizeX));
		if (explodeTime > 0) {
			ShrinkAndDestroy sad = clone.AddComponent<ShrinkAndDestroy> ();
			sad.delayTime = explodeTime;
			sad.explosion = playerObjectExplosion;
		}

		MeshRenderer renderer = clone.GetComponent<MeshRenderer>();
        if (!renderer)
        {
            renderer = clone.GetComponentInChildren<MeshRenderer>();
        }
        if (renderer)
        {
            renderer.material.color = color;
        }

        return clone;
	}

	public void SpawnCustomObject(JSONObject data, Color color) {
		Debug.Log ("spawning custom object");
	}

	public void SpawnDrawnObject(JSONObject data)
	{
		List<JSONObject> colorData = data ["color"].list;
		Color color = new Color (colorData[0].n, colorData[1].n, colorData[2].n);

		float sizeX = data ["sizeX"].n;
		float sizeY = data ["sizeY"].n;

		string name = data ["name"].str;

		switch (name.ToLower ()) {
		case "square":
			SpawnKnownGameObject (cube, color, sizeX, sizeY, playerObjectShrinkTime);
			break;
		case "circle":
			SpawnKnownGameObject (sphere, color, sizeX, sizeY, playerObjectShrinkTime);
			break;
		case "gun":
			SpawnKnownGameObject (gun, color, sizeX, sizeY, playerObjectShrinkTime * 3);
			break;
		case "custom":
			SpawnCustomObject (data, color);
			break;
		default:
			Debug.LogError ("Unknown drawn object: " + name);
			break;
		}
	}
}
