using UnityEngine;
using System.Collections;

public class ShrinkAndDestroy : MonoBehaviour {

	public float rate = 0.4f;
	public float destroyCutoff = 0.05f;
	public float delayTime = 3f;
	public GameObject explosion;
	public GameObject toDestroy;

	private float startTime;

	// Use this for initialization
	void Start () {
		startTime = Time.time;
		if (toDestroy == null) {
			toDestroy = gameObject;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > startTime + delayTime) {
			transform.localScale -= Vector3.one * rate * Time.deltaTime;
			if (transform.localScale.x <= destroyCutoff) {
				Destroy (toDestroy);
				Instantiate (explosion, transform.position, transform.rotation);
			}
		}
	}
}
