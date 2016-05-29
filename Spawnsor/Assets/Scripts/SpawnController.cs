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
		SpawnCustomObject ();
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
		if (liveTime > 0) {
			FadeAndDestroy fad = clone.AddComponent<FadeAndDestroy> ();
			fad.delayTime = liveTime;
			fad.explosion = playerObjectExplosion;
		}

		NVRInteractableItem interactableItem = clone.GetComponent<NVRInteractableItem> ();
		if (!interactableItem) {
			clone.AddComponent<NVRInteractableItem> ();
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

	public void SpawnCustomObject(JSONObject data) {
	}
	public void SpawnCustomObject() {
		Debug.Log ("spawning custom object");
		GameObject go = new GameObject ();
		MeshFilter filter = go.AddComponent<MeshFilter>();
		filter.mesh = ExtrudeMeshFromPoints ();

		go.name = "Junk";
		go.tag = "PlayerCreated";
		go.AddComponent<Rigidbody> ();
		MeshRenderer r = go.AddComponent<MeshRenderer> ();
		r.material = customObjectMaterial;
		MeshCollider col = go.AddComponent<MeshCollider> ();
		col.sharedMesh = ExtrudeMeshFromPoints ();
		col.convex = true;
		go.transform.position = new Vector3 (0, 1, 0);
	}

	public Mesh ExtrudeMeshFromPoints ()
	{
		Mesh mesh = new Mesh ();
		float height = 1f;
		float bottomRadius = .25f;
		float topRadius = .05f;
		int nbSides = 18;
		int nbHeightSeg = 1; // Not implemented yet

		int nbVerticesCap = nbSides + 1;
		#region Vertices

		// bottom + top + sides
		Vector3[] vertices = new Vector3[nbVerticesCap + nbVerticesCap + nbSides * nbHeightSeg * 2 + 2];
		int vert = 0;
		float _2pi = Mathf.PI * 2f;

		// Bottom cap
		vertices[vert++] = new Vector3(0f, 0f, 0f);
		while( vert <= nbSides )
		{
			float rad = (float)vert / nbSides * _2pi;
			vertices[vert] = new Vector3(Mathf.Cos(rad) * bottomRadius, 0f, Mathf.Sin(rad) * bottomRadius);
			vert++;
		}

		// Top cap
		vertices[vert++] = new Vector3(0f, height, 0f);
		while (vert <= nbSides * 2 + 1)
		{
			float rad = (float)(vert - nbSides - 1)  / nbSides * _2pi;
			vertices[vert] = new Vector3(Mathf.Cos(rad) * topRadius, height, Mathf.Sin(rad) * topRadius);
			vert++;
		}

		// Sides
		int v = 0;
		while (vert <= vertices.Length - 4 )
		{
			float rad = (float)v / nbSides * _2pi;
			vertices[vert] = new Vector3(Mathf.Cos(rad) * topRadius, height, Mathf.Sin(rad) * topRadius);
			vertices[vert + 1] = new Vector3(Mathf.Cos(rad) * bottomRadius, 0, Mathf.Sin(rad) * bottomRadius);
			vert+=2;
			v++;
		}
		vertices[vert] = vertices[ nbSides * 2 + 2 ];
		vertices[vert + 1] = vertices[nbSides * 2 + 3 ];
		#endregion

		#region Normales

		// bottom + top + sides
		Vector3[] normales = new Vector3[vertices.Length];
		vert = 0;

		// Bottom cap
		while( vert  <= nbSides )
		{
			normales[vert++] = Vector3.down;
		}

		// Top cap
		while( vert <= nbSides * 2 + 1 )
		{
			normales[vert++] = Vector3.up;
		}

		// Sides
		v = 0;
		while (vert <= vertices.Length - 4 )
		{			
			float rad = (float)v / nbSides * _2pi;
			float cos = Mathf.Cos(rad);
			float sin = Mathf.Sin(rad);

			normales[vert] = new Vector3(cos, 0f, sin);
			normales[vert+1] = normales[vert];

			vert+=2;
			v++;
		}
		normales[vert] = normales[ nbSides * 2 + 2 ];
		normales[vert + 1] = normales[nbSides * 2 + 3 ];
		#endregion

		#region UVs
		Vector2[] uvs = new Vector2[vertices.Length];

		// Bottom cap
		int u = 0;
		uvs[u++] = new Vector2(0.5f, 0.5f);
		while (u <= nbSides)
		{
			float rad = (float)u / nbSides * _2pi;
			uvs[u] = new Vector2(Mathf.Cos(rad) * .5f + .5f, Mathf.Sin(rad) * .5f + .5f);
			u++;
		}

		// Top cap
		uvs[u++] = new Vector2(0.5f, 0.5f);
		while (u <= nbSides * 2 + 1)
		{
			float rad = (float)u / nbSides * _2pi;
			uvs[u] = new Vector2(Mathf.Cos(rad) * .5f + .5f, Mathf.Sin(rad) * .5f + .5f);
			u++;
		}

		// Sides
		int u_sides = 0;
		while (u <= uvs.Length - 4 )
		{
			float t = (float)u_sides / nbSides;
			uvs[u] = new Vector3(t, 1f);
			uvs[u + 1] = new Vector3(t, 0f);
			u += 2;
			u_sides++;
		}
		uvs[u] = new Vector2(1f, 1f);
		uvs[u + 1] = new Vector2(1f, 0f);
		#endregion 

		#region Triangles
		int nbTriangles = nbSides + nbSides + nbSides*2;
		int[] triangles = new int[nbTriangles * 3 + 3];

		// Bottom cap
		int tri = 0;
		int i = 0;
		while (tri < nbSides - 1)
		{
			triangles[ i ] = 0;
			triangles[ i+1 ] = tri + 1;
			triangles[ i+2 ] = tri + 2;
			tri++;
			i += 3;
		}
		triangles[i] = 0;
		triangles[i + 1] = tri + 1;
		triangles[i + 2] = 1;
		tri++;
		i += 3;

		// Top cap
		//tri++;
		while (tri < nbSides*2)
		{
			triangles[ i ] = tri + 2;
			triangles[i + 1] = tri + 1;
			triangles[i + 2] = nbVerticesCap;
			tri++;
			i += 3;
		}

		triangles[i] = nbVerticesCap + 1;
		triangles[i + 1] = tri + 1;
		triangles[i + 2] = nbVerticesCap;		
		tri++;
		i += 3;
		tri++;

		// Sides
		while( tri <= nbTriangles )
		{
			triangles[ i ] = tri + 2;
			triangles[ i+1 ] = tri + 1;
			triangles[ i+2 ] = tri + 0;
			tri++;
			i += 3;

			triangles[ i ] = tri + 1;
			triangles[ i+1 ] = tri + 2;
			triangles[ i+2 ] = tri + 0;
			tri++;
			i += 3;
		}
		#endregion

		mesh.vertices = vertices;
		mesh.normals = normales;
		mesh.uv = uvs;
		mesh.triangles = triangles;

		mesh.RecalculateBounds();
		mesh.Optimize();

		return mesh;
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
