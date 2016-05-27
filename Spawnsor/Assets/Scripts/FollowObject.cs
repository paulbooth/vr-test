using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour {

	public GameObject tracked;

	private Vector3 offset;

	// Use this for initialization
	void Start () {
		offset = transform.position - tracked.transform.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		transform.position = tracked.transform.position + offset;
	}
}
