using UnityEngine;
using System.Collections;
using NewtonVR;

public class Constructable : NVRInteractableItem
{
	public float breakForce = 700f;

	public override void UseButtonDown()
	{
		base.UseButtonDown ();
		NVRPlayer player = AttachedHand.GetComponentInParent<NVRPlayer> ();
		if (!player.LeftHand || !player.RightHand) {
			return;
		}
		NVRInteractable left = player.LeftHand.CurrentlyInteracting;
		NVRInteractable right = player.RightHand.CurrentlyInteracting;
		if (!left || !right) {
			return;
		}
		GameObject leftGame = left.gameObject;
		GameObject rightGame = right.gameObject;
		if (!leftGame || !rightGame) {
			return;
		}

		Constructable leftCons = leftGame.GetComponent<Constructable> ();
		Constructable rightCons = rightGame.GetComponent<Constructable> ();
		if (leftCons && rightCons &&
			player.LeftHand.UseButtonPressed && player.RightHand.UseButtonPressed) {

			Debug.Log ("MAKING CONNECTION");
			FixedJoint f = left.gameObject.AddComponent<FixedJoint> ();
			f.breakForce = breakForce;
			f.connectedBody = right.gameObject.GetComponent<Rigidbody> ();
		}
	}
}
