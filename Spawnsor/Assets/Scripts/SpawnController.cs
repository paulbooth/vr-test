using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NewtonVR;

public class SpawnController : MonoBehaviour
{
    public GameObject zombie;
    public GameObject cube;
    public GameObject sphere;
    public GameObject gun;
    public GameObject sword;
    public GameObject grenade;
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

    public void Start()
    {
        if (zombieCount > 0)
        {
            SpawnZombieWaves(zombieCount, startWait, spawnWait, waveWait, 0);
        }
    }

    public void SpawnZombieWaves(int zombieCount, float startWait, float spawnWait, float waveWait, int numWaves)
    {
        StartCoroutine(SpawnZombieWavesRoutine(zombieCount, startWait, spawnWait, waveWait, numWaves));
    }

    IEnumerator SpawnZombieWavesRoutine(int zombieCount, float startWait, float spawnWait, float waveWait, int numWaves)
    {
        yield return new WaitForSeconds(startWait);
        int waveCount = 0;
        while (numWaves == 0 || waveCount < numWaves)
        {
            waveCount++;
            for (int i = 0; i < zombieCount; i++)
            {
                SpawnZombie();
                yield return new WaitForSeconds(spawnWait);
            }
            yield return new WaitForSeconds(waveWait);
        }
    }

    public void SpawnZombie()
    {
        bool xDir = Random.value > 0.5f;
        float wallDistance = Random.value > 0.5f ? wallLength : -wallLength;
        float wallPosition = Random.Range(-wallLength, wallLength);
        Vector3 spawnPosition = new Vector3(
            xDir ? wallPosition : wallDistance,
            spawnHeight,
            xDir ? wallDistance : wallPosition
        );
        GameObject zombieClone = Instantiate(zombie, spawnPosition, Quaternion.identity) as GameObject;
        zombieClone.GetComponent<NavigateObject>().tracked = zombieTarget;
    }

    private Vector3 getPlayerSpawnObjectPosition()
    {
		return zombieTarget.position + zombieTarget.forward * 0.5f;
    }

    public GameObject SpawnGun(Color color, float sizeX, float sizeY, float liveTime)
    {
        GameObject gunClone = SpawnKnownGameObject(gun, color, sizeX, sizeY, liveTime);
        GunController gc = gunClone.GetComponent<GunController>();
        if (gc)
        {
            gc.BulletColor = color;
        }
        return gunClone;
    }

    public GameObject SpawnKnownGameObject(GameObject go, Color color, float sizeX, float sizeY, float liveTime)
    {
        GameObject clone = Instantiate(go, getPlayerSpawnObjectPosition(), Quaternion.identity) as GameObject;
        clone.transform.localScale = Vector3.Scale(clone.transform.localScale, new Vector3(sizeX, sizeY, sizeX));

        TurnIntoPlayerMadeObject(clone, color, liveTime);
        return clone;
    }

    public void TurnIntoPlayerMadeObject(GameObject go, Color color, float liveTime)
    {
        if (liveTime > 0)
        {
            FadeAndDestroy fad = go.AddComponent<FadeAndDestroy>();
            fad.delayTime = liveTime;
//			fad.explosion = playerObjectExplosion;
        }

        NVRInteractableItem interactableItem = go.GetComponent<NVRInteractableItem>();
        if (!interactableItem)
        {
            interactableItem = go.GetComponentInChildren<NVRInteractableItem>();
        }
        if (!interactableItem)
        {
            go.AddComponent<NVRInteractableItem>();
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

		Rigidbody rb = go.GetComponent<Rigidbody> ();
		if (!rb) {
			Rigidbody r = go.GetComponentInChildren<Rigidbody> ();
			if (r.gameObject.GetComponent<NVRInteractable> ()) {
				r.isKinematic = true;
			}
		} else {
			rb.isKinematic = true;
		}

        go.transform.position = getPlayerSpawnObjectPosition();
        go.tag = "PlayerCreated";
    }

    private Vector2[] GetVectorsFromJSON(List<JSONObject> data)
    {
        Vector2[] points = new Vector2[data.Count];
        for (int i = 0; i < data.Count; i++)
        {
            points[i] = new Vector2(data[i]["X"].n, data[i]["Y"].n);
        }
        return points;
    }

    public void SpawnCustomObject(JSONObject data)
    {
        Vector2[] points = GetVectorsFromJSON(data["corners"].list);
        Vector2[] colliderPoints = GetVectorsFromJSON(data["corners"].list);
        SpawnCustomObject(points, colliderPoints, getColorFromJSON(data));
    }

    public void SpawnCustomObject(Vector2[] points)
    {
        SpawnCustomObject(points, points, Color.cyan);
    }

    public void SpawnCustomObject(Vector2[] points, Vector2[] colliderPoints, Color color)
    {
        Debug.Log("spawning custom object");

        GameObject go = new GameObject();
        MeshFilter filter = go.AddComponent<MeshFilter>();
        filter.mesh = ExtrudeMeshFromPoints(points);

        go.name = "Junk";
        go.AddComponent<Rigidbody>();
        MeshRenderer r = go.AddComponent<MeshRenderer>();
        r.material = customObjectMaterial;
        MeshCollider col = go.AddComponent<MeshCollider>();
        col.sharedMesh = ExtrudeMeshFromPoints(colliderPoints);
        col.convex = true;

        TurnIntoPlayerMadeObject(go, color, 0);
    }

    public int[] GetReverseTriangles(int[] indices, int offset)
    {
        int[] newIndices = new int[indices.Length];
        for (int i = 0; i < indices.Length; i += 3)
        {
            newIndices[i] = indices[i] + offset;
            newIndices[i + 1] = indices[i + 2] + offset;
            newIndices[i + 2] = indices[i + 1] + offset;
        }
        return newIndices;
    }

    private bool ShouldFlipPerimeter(int[] triangles, int vectorCount)
    {
        for (int i = 0; i < triangles.Length; i += 3)
        {
            if ((triangles[0] - triangles[1]) % vectorCount == 1 ||
            (triangles[1] - triangles[2]) % vectorCount == 1 ||
            (triangles[2] - triangles[0]) % vectorCount == 1)
                return false;
            if ((triangles[0] - triangles[1]) % vectorCount == -1 ||
            (triangles[1] - triangles[2]) % vectorCount == -1 ||
            (triangles[2] - triangles[0]) % vectorCount == -1)
                return true;
        }
        // Should never get here...
        Debug.Log("ShouldFlipPerimeter CRITICAL FAIL WTF");
        return true;
    }

    public Mesh ExtrudeMeshFromPoints(Vector2[] points2d)
    {
        if (isClockwise(points2d))
        {
            // Debug.Log("It's clockwise!");
            System.Array.Reverse(points2d);
        }

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4 * points2d.Length];

        // Vertex list
        int sideOffset = 0; //2 * points2d.Length;
        for (int i = 0; i < points2d.Length; i++)
        {
            Vector2 point = points2d[i];
            vertices[i] = new Vector3(point.x, point.y, 0);
            vertices[points2d.Length + i] = new Vector3(point.x, point.y, extrusionDepth);
            vertices[sideOffset + i] = new Vector3(point.x, point.y, 0);
            vertices[sideOffset + points2d.Length + i] = new Vector3(point.x, point.y, extrusionDepth);
        }

        Triangulator tr = new Triangulator(points2d);
        int[] backIndices = tr.Triangulate();
        int[] topIndices = GetReverseTriangles(backIndices, points2d.Length);

        int[] sideIndices = new int[6 * points2d.Length];

        //		 sides
        for (int i = 0; i < points2d.Length - 1; i++)
        {
            sideIndices[6 * i + 0] = sideOffset + i;
            sideIndices[6 * i + 1] = sideOffset + i + 1;
            sideIndices[6 * i + 2] = sideOffset + points2d.Length + i;

            sideIndices[6 * i + 3] = sideOffset + points2d.Length + i;
            sideIndices[6 * i + 4] = sideOffset + i + 1;
            sideIndices[6 * i + 5] = sideOffset + points2d.Length + i + 1;
        }
        sideIndices[sideIndices.Length - 6] = sideOffset + points2d.Length - 1;
        sideIndices[sideIndices.Length - 5] = sideOffset + 0;
        sideIndices[sideIndices.Length - 4] = sideOffset + 2 * points2d.Length - 1;

        sideIndices[sideIndices.Length - 3] = sideOffset + 2 * points2d.Length - 1;
        sideIndices[sideIndices.Length - 2] = sideOffset + 0;
        sideIndices[sideIndices.Length - 1] = sideOffset + points2d.Length;

        //if (ShouldFlipPerimeter(sideIndices, points2d.Length))
       // {
       //     sideIndices = GetReverseTriangles (sideIndices, 0);
       //     //return ExtrudeMeshFromPoints(points2d);
       // }

        // put it all together
        int[] triangles = new int[topIndices.Length + backIndices.Length + sideIndices.Length];
        topIndices.CopyTo(triangles, 0);
        backIndices.CopyTo(triangles, topIndices.Length);
        sideIndices.CopyTo(triangles, topIndices.Length + backIndices.Length);
      
        //		// UVs (texture mapping points) - Just half-assing for now. Easy to improve later!
        //		Vector2[] uv = new Vector2[vertices.Length];
        //		for (int i = 0; i < topPtOffset; i += 2) {
        //			uv[i + 0] = new Vector2((float)i/topPtOffset, 0);
        //			uv[i + 1] = new Vector2((float)i/topPtOffset, 1);
        //		}
        //		for (int i = topPtOffset; i < bottomPtOffset; i++) {
        //			uv[i] = new Vector2(0,0);
        //		}
        //		for (int i = bottomPtOffset; i < vertices.Length; i++) {
        //			uv[i] = new Vector2(0,0);
        //		}

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        NormalSolver.RecalculateNormals(mesh, 60f);
        //		mesh.uv = uv;
        mesh.Optimize();

        return mesh;
    }

    private Color getColorFromJSON(JSONObject data)
    {
        List<JSONObject> colorData = data["color"].list;
        return new Color(colorData[0].n, colorData[1].n, colorData[2].n);
    }

    public void SpawnDrawnObject(JSONObject data)
    {
        Color color = getColorFromJSON(data);
        float sizeX = data["sizeX"].n;
        float sizeY = data["sizeY"].n;

        string name = data["name"].str;

        switch (name.ToLower())
        {
            case "square":
                SpawnKnownGameObject(cube, color, sizeX, sizeY, playerObjectLiveTime);
                break;
            case "circle":
                SpawnKnownGameObject(sphere, color, sizeX, sizeY, playerObjectLiveTime);
                break;
            case "grenade":
                SpawnKnownGameObject(grenade, color, sizeX, sizeY, playerObjectLiveTime * 3);
                break;
            case "sword":
                SpawnKnownGameObject(sword, color, sizeX, sizeY, playerObjectLiveTime);
                break;
            case "gun":
                SpawnGun(color, sizeX, sizeY, playerObjectLiveTime * 3);
                break;
            default:
                Debug.LogError("Unknown drawn object: " + name);
                break;
        }
    }

    private void print2dpts(Vector2[] vs)
    {
        string s = "";
        foreach (Vector2 v in vs)
        {
            s += v.ToString() + ",";
        }
        Debug.Log(s);
    }

    private bool isClockwise(Vector2[] vs)
    {
        float rotAcc = 0f;
        for (int i = 0; i < vs.Length; i++)
        {
            Vector2 p1 = vs[i];
            Vector2 p2 = vs[(i + 1) % vs.Length];
            Vector2 p3 = vs[(i + 2) % vs.Length];
            Vector2 v1 = p2 - p1;
            Vector2 v2 = p3 - p2;
            float angle = Vector2.Angle(v1, v2);
            int sign = Vector3.Cross(v1, v2).z > 0 ? -1 : 1;
            rotAcc += angle * sign;
        }

        return rotAcc > 0;
    }
    
    private void analyzeTriangles(Vector3[] vs, int[] indices)
    {
        int numTriangles = indices.Length / 3;
        Debug.Log(numTriangles.ToString() + " triangles.");
        for (int t = 0; t < indices.Length; t += 3)
        {
            // Just use the 1st 2 dimensions. Top and bottom analysis atm only.
            if (isClockwise(new[] {
                new Vector2(vs[indices[t+0]].x, vs[indices[t+0]].y),
                new Vector2(vs[indices[t+1]].x, vs[indices[t+1]].y),
                new Vector2(vs[indices[t+2]].x, vs[indices[t+2]].y)
            }))
            {
                Debug.Log("Triangle " + (t/3).ToString() + " is clockwise.");
            } else
            {
                Debug.Log("Triangle " + (t/3).ToString() + " is NOT clockwise.");
            }
            
        }
    }

    public static int[] SubArray(int[] data, int index, int length)
    {
        int[] result = new int[length];
        System.Array.Copy(data, index, result, 0, length);
        return result;
    }
}
