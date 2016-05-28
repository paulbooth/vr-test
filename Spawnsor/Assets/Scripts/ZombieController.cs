using UnityEngine;
using System.Collections;

public class ZombieController : MonoBehaviour {

	public NavigateObject navigator;
	public GameObject headExplosion;
	public GameObject bodyExplosion;

	private float uprightThreshold = 0.6f;
	private Rigidbody rb;
	private FixedJoint fixedJoint;
	private bool headAttached = true;

	void Start() {
		rb = GetComponent<Rigidbody>();
		fixedJoint = GetComponent<FixedJoint>();
	}

	void Update ()
	{
		if (IsUpright () && headAttached) {
			navigator.StartTracking ();
		} else {
			navigator.StopTracking ();
		}
	}

	void FixedUpdate ()
	{
		rb.AddForce(Vector3.up);
	}

	bool IsUpright()
	{
		return transform.up.y > uprightThreshold;
	}

	void OnJointBreak(float breakForce) {
		navigator.StopTracking ();
		headAttached = false;
		GameObject head = fixedJoint.connectedBody.gameObject;
		ShrinkAndDestroy headShrink = head.AddComponent<ShrinkAndDestroy> ();
		headShrink.explosion = headExplosion;
		headShrink.rate = 0.25f + Random.value * 0.1f;
		headShrink.toDestroy = transform.parent.gameObject;
		ShrinkAndDestroy bodyShrink = gameObject.AddComponent<ShrinkAndDestroy> ();
		bodyShrink.explosion = bodyExplosion;
		bodyShrink.rate = 0.75f + Random.value * 0.1f;
	}
}
