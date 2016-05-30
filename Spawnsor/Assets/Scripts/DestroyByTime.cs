using UnityEngine;
using System.Collections;

public class DestroyByTime : MonoBehaviour {
	public float lifetime;
	public GameObject explosion;
	public GameObject toDestroy;


	private float startTime;
	void Start ()
	{
		startTime = Time.time;
		if (toDestroy == null) {
			toDestroy = gameObject;
		}

	}

	void Update () {
		if (Time.time > startTime + lifetime) {
			Destroy (toDestroy);
			if (explosion) {
				GameObject exp = Instantiate (explosion, transform.position, transform.rotation) as GameObject;
				Detonator d = exp.GetComponent<Detonator> ();
				if (d) {
					MeshRenderer renderer = GetComponent<MeshRenderer>();
					if (!renderer)
					{
						renderer = GetComponentInChildren<MeshRenderer>();
					}
					if (renderer)
					{
						d.color = renderer.material.color;
					}
				}
			}
		}
	}
}
