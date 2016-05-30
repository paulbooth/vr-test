using UnityEngine;
using System.Collections;

public class ObjectDetector : MonoBehaviour {
	public bool shouldCollide = true;
	public bool isColliding = false;

	private int numColliding = 0;

	void Start () {
		updateState ();
	}

	public bool areRequirementsMet() {
		return shouldCollide == isColliding;
	}

	void OnTriggerEnter(Collider col) {
		if (eligibleForCollision(col)) {
			isColliding = true;
			numColliding++;
			updateState ();
		}
	}

	void OnTriggerExit(Collider col) {
		if (eligibleForCollision(col)) {
			numColliding--;
			if (numColliding <= 0) {
				isColliding = false;
				updateState ();
			}
		}
	}

	bool eligibleForCollision(Collider col) {
		return col.gameObject.GetComponent<Constructable> ();
	}

	void updateState() {
		bool requirementsMet = areRequirementsMet ();
		MeshRenderer renderer = GetComponent<MeshRenderer> ();
		Color color = renderer.material.color;
		if (requirementsMet) {
			color.r = 0;
			color.g = 255;
		} else {
			color.r = 255;
			color.g = 0;
		}
		renderer.material.color = color;

		Light l = GetComponent<Light> ();
		if (l) {
			Color lightColor = l.color;
			lightColor.r = requirementsMet ? 0 : 1;
			lightColor.g = requirementsMet ? 1 : 0;
			l.color = lightColor;
		}
	}
}
