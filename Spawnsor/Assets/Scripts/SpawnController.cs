using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NewtonVR;

public class SpawnController : MonoBehaviour {

	public GameObject zombie;
	public GameObject cube;
	public GameObject sphere;
	public GameObject gun;
	public GameObject playerObjectExplosion;
	public Material customObjectMaterial;
	public float playerObjectLiveTime = 5f;
	public float extrusionDepth = 2;

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

	public GameObject SpawnKnownGameObject(GameObject go, Color color, float sizeX, float sizeY, float liveTime)
	{
		GameObject clone = Instantiate (go, getPlayerSpawnObjectPosition (), Quaternion.identity) as GameObject;
		clone.transform.localScale = Vector3.Scale(clone.transform.localScale, new Vector3(sizeX, sizeY, sizeX));

		TurnIntoPlayerMadeObject (clone, color, liveTime);
        return clone;
	}

	public void TurnIntoPlayerMadeObject(GameObject go, Color color, float liveTime)
	{
		if (liveTime > 0) {
			FadeAndDestroy fad = go.AddComponent<FadeAndDestroy> ();
			fad.delayTime = liveTime;
			fad.explosion = playerObjectExplosion;
		}

		NVRInteractableItem interactableItem = go.GetComponent<NVRInteractableItem> ();
		if (!interactableItem) {
			go.AddComponent<NVRInteractableItem> ();
		}

		MeshRenderer renderer = go.GetComponent<MeshRenderer>();
		if (!renderer)
		{
			renderer = go.GetComponentInChildren<MeshRenderer>();
		}
		if (renderer)
		{
			renderer.material.color = color;
		}

		go.transform.position = getPlayerSpawnObjectPosition ();
		go.tag = "PlayerCreated";
	}

	private Vector2[] GetVectorsFromJSON(List<JSONObject> data)
	{
		Vector2[] points = new Vector2[data.Count];
		for (int i = 0; i < data.Count; i++) {
			points [i] = new Vector2 (data [i] ["X"].n, data [i] ["Y"].n);
		}
		return points;
	}

	public void SpawnCustomObject(JSONObject data)
	{
		Vector2[] points = GetVectorsFromJSON (data ["points"].list);
		Vector2[] colliderPoints = GetVectorsFromJSON (data ["corners"].list);
		SpawnCustomObject (points, colliderPoints, getColorFromJSON(data));
	}

	public void SpawnCustomObject(Vector2[] points)
	{
		SpawnCustomObject (points, points, Color.cyan);
	}

	public void SpawnCustomObject(Vector2[] points, Vector2[] colliderPoints, Color color)
	{
		Debug.Log ("spawning custom object");

		GameObject go = new GameObject ();
		MeshFilter filter = go.AddComponent<MeshFilter>();
		filter.mesh = ExtrudeMeshFromPoints (points);

		go.name = "Junk";
		go.AddComponent<Rigidbody> ();
		MeshRenderer r = go.AddComponent<MeshRenderer> ();
		r.material = customObjectMaterial;
		MeshCollider col = go.AddComponent<MeshCollider> ();
		col.sharedMesh = ExtrudeMeshFromPoints (colliderPoints);
		col.convex = true;

		TurnIntoPlayerMadeObject (go, color, playerObjectLiveTime);
	}

	public Mesh ExtrudeMeshFromPoints (Vector2[] points2d)
	{
		Mesh mesh = new Mesh ();

		Vector3[] vertices = new Vector3[4 * points2d.Length];

		// Vertex list
		for (int i = 0; i < points2d.Length; i++) {
			Vector2 point = points2d[i];
			vertices[2 * i] = new Vector3(point.x, point.y, 0);
			vertices[2 * i + 1] = new Vector3(point.x, point.y, extrusionDepth);
		}
		// Gotta reuse points for top and bottom. UV mapping and normals differs for the repeats.
		int topPtOffset = 2 * points2d.Length;
		int bottomPtOffset = 3 * points2d.Length;
		for (int i = 0; i < points2d.Length; i++) {
			Vector2 point = points2d[i];
			vertices[topPtOffset + i] = new Vector3(point.x, point.y, 0); // top
			vertices[bottomPtOffset + i] = new Vector3(point.x, point.y, extrusionDepth); // bottom
		}
		int j = 0;
		foreach (Vector3 v in vertices) {
			j++;
		}

		// Triangle corner indices into vertex list
		// 1 rectangle for each edge = 2 triangles = 6 points
		// Also triangles for the cylinder top and bottom
		int sideTriPts = 6 * (points2d.Length);
		int topTriPts = 3 * (points2d.Length - 2);
		int[] triangles = new int[sideTriPts + 2 * (topTriPts)];
		// sides
		for (int i = 0; i < points2d.Length; i++) {
			triangles[6 * i + 0] = (2 * i + 0) % topPtOffset;
			triangles[6 * i + 1] = (2 * i + 2) % topPtOffset;
			triangles[6 * i + 2] = (2 * i + 1) % topPtOffset;

			triangles[6 * i + 3] = (2 * i + 2) % topPtOffset;
			triangles[6 * i + 4] = (2 * i + 3) % topPtOffset;
			triangles[6 * i + 5] = (2 * i + 1) % topPtOffset;
		}
		// top and bottom
		for (int i = 0; i < points2d.Length - 2; i++) {
			triangles[sideTriPts + 3 * i + 0] = topPtOffset + 0; // Every triangle on top uses this vertex
			triangles[sideTriPts + 3 * i + 1] = topPtOffset + i + 2;
			triangles[sideTriPts + 3 * i + 2] = topPtOffset + i + 1;

			triangles[sideTriPts + topTriPts + 3 * i + 0] = bottomPtOffset + 0; // Every triangle on bottom uses this vertex
			triangles[sideTriPts + topTriPts + 3 * i + 1] = bottomPtOffset + i + 1;
			triangles[sideTriPts + topTriPts + 3 * i + 2] = bottomPtOffset + i + 2;
		}
		j = 0;
		int k = 0;
		foreach (int t in triangles) {
//			Debug.Log (k.ToString() + ":"+ j.ToString() + "-" + t.ToString());
			j = (j + 1) % 3;
			if (j == 0)
				k++;
		}

		// UVs (texture mapping points) - Just half-assing for now. Easy to improve later!
		Vector2[] uv = new Vector2[vertices.Length];
		for (int i = 0; i < topPtOffset; i += 2) {
			uv[i + 0] = new Vector2((float)i/topPtOffset, 0);
			uv[i + 1] = new Vector2((float)i/topPtOffset, 1);
		}
		for (int i = topPtOffset; i < bottomPtOffset; i++) {
			uv[i] = new Vector2(0,0);
		}
		for (int i = bottomPtOffset; i < vertices.Length; i++) {
			uv[i] = new Vector2(0,0);
		}

		// Assign to mesh
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals ();
		mesh.uv = uv;
		mesh.RecalculateBounds();
		mesh.Optimize();

		return mesh;
	}

	private Color getColorFromJSON(JSONObject data)
	{
		List<JSONObject> colorData = data ["color"].list;
		return new Color (colorData[0].n, colorData[1].n, colorData[2].n);
	}

	public void SpawnDrawnObject(JSONObject data)
	{
		Color color = getColorFromJSON (data);
		float sizeX = data ["sizeX"].n;
		float sizeY = data ["sizeY"].n;

		string name = data ["name"].str;

		switch (name.ToLower ()) {
		case "square":
			SpawnKnownGameObject (cube, color, sizeX, sizeY, playerObjectLiveTime);
			break;
		case "circle":
			SpawnKnownGameObject (sphere, color, sizeX, sizeY, playerObjectLiveTime);
			break;
		case "gun":
			SpawnKnownGameObject (gun, color, sizeX, sizeY, playerObjectLiveTime * 3);
			break;
		default:
			Debug.LogError ("Unknown drawn object: " + name);
			break;
		}
	}
}
