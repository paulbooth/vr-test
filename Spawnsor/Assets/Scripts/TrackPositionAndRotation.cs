using UnityEngine;
using System.Collections;

public class TrackPositionAndRotation : MonoBehaviour {

	public Transform tracked;

	private Vector3 positionOffset;
//	private Quaternion rotationOffset;

	// Use this for initialization
	void Start () {
		positionOffset = transform.position - tracked.transform.position;
//		rotationOffset = transform.rotation - transform.rotation;
	}
	
	// Update is called once per frame
	void LastUpdate () {
		if (tracked) {
			transform.position = tracked.position + positionOffset;
			transform.rotation = tracked.rotation;
		}
	}
}
