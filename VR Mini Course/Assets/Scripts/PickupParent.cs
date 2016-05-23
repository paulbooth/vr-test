using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class PickupParent : MonoBehaviour {

	SteamVR_TrackedObject trackedObj;
	SteamVR_Controller.Device device;

	// Use this for initialization
	void Awake () {
		trackedObj = GetComponent<SteamVR_TrackedObject> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		device = SteamVR_Controller.Input ((int)trackedObj.index);
		if (device.GetTouch (SteamVR_Controller.ButtonMask.Trigger)) {
			Debug.Log ("holding down the trigger");
		}
	}

	void OnTriggerStay (Collider col)
	{
		if (device.GetTouch (SteamVR_Controller.ButtonMask.Trigger)) {
			col.attachedRigidbody.isKinematic = true;
			col.gameObject.transform.SetParent (gameObject.transform);
		}
		if (device.GetTouchUp (SteamVR_Controller.ButtonMask.Trigger)) {
			col.gameObject.transform.SetParent (null);
			col.attachedRigidbody.isKinematic = false;
		}
	}
}
