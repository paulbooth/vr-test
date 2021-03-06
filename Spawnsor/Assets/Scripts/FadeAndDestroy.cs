﻿using UnityEngine;
using System.Collections;

public class FadeAndDestroy : MonoBehaviour {


	public float rate = 0.1f;
	public float destroyCutoff = 0f;
	public float delayTime = 3f;
	public GameObject explosion;
	public GameObject toDestroy;

	private float startTime;
	private float shrinkX, shrinkY, shrinkZ;
	private bool setToFade = false;

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

			MeshRenderer renderer = GetComponent<MeshRenderer>();
			bool destroy = false;
			if (!renderer) {
				foreach (MeshRenderer r in GetComponentsInChildren<MeshRenderer>()) {
					if (!setToFade) {
						r.material.EnableKeyword ("_ALPHABLEND_ON");
						r.material.SetFloat ("_Mode", 2f);
					}
					Color color = r.material.color;
					color.a -= rate * Time.deltaTime;
					r.material.color = color;
					destroy = r.material.color.a <= destroyCutoff;
				}
			} else {
				if (!setToFade) {
					renderer.material.EnableKeyword ("_ALPHABLEND_ON");
					renderer.material.SetFloat ("_Mode", 2f);
				}
				Color color = renderer.material.color;
				color.a -= rate * Time.deltaTime;
//				Debug.Log (color.a);
				renderer.material.color = color;
				destroy = renderer.material.color.a <= destroyCutoff;
			}
			setToFade = true;

			if (destroy) {
				Destroy (toDestroy);
				if (explosion) {
					GameObject exp = Instantiate (explosion, transform.position, transform.rotation) as GameObject;
					Detonator d = exp.GetComponent<Detonator> ();
					if (d) {
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
}
