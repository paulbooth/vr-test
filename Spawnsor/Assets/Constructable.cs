using UnityEngine;
using System.Collections;
using NewtonVR;

public class Constructable : NVRInteractableItem
{
	public float breakForce = 100f;

	public override void UseButtonDown()
	{
		base.UseButtonDown ();
		NVRPlayer player = AttachedHand.GetComponentInParent<NVRPlayer> ();
		NVRInteractable left = player.LeftHand.CurrentlyInteracting;
		NVRInteractable right = player.RightHand.CurrentlyInteracting;
		Constructable leftCons = left.gameObject.GetComponent<Constructable> ();
		Constructable rightCons = right.gameObject.GetComponent<Constructable> ();
		if (leftCons && rightCons && player.LeftHand.UseButtonPressed && player.RightHand.UseButtonPressed) {
			Debug.Log ("MAKING CONNECTION");
			FixedJoint f = left.gameObject.AddComponent<FixedJoint> ();
			f.breakForce = breakForce;
			f.connectedBody = right.gameObject.GetComponent<Rigidbody> ();
		}
	}
}
