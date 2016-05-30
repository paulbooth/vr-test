using UnityEngine;
using System.Collections;

public class GrenadeController : MonoBehaviour {

	public GameObject explosion;
	public float fuseTime = 3f;

	private FixedJoint fixedJoint;

	// Use this for initialization
	void Start () {
		fixedJoint = GetComponent<FixedJoint>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnJointBreak(float breakForce) {
		ParticleSystem ps = GetComponent<ParticleSystem> ();
		if (ps) {
			ps.enableEmission = true;
		}
		GameObject pin = fixedJoint.connectedBody.gameObject;
		FadeAndDestroy pinFade = pin.AddComponent<FadeAndDestroy> ();
		pinFade.delayTime = 1f;
		pinFade.rate = 0.25f + Random.value * 0.1f;
		DestroyByTime explode = gameObject.AddComponent<DestroyByTime> ();
		explode.lifetime = fuseTime;
		explode.explosion = explosion;
		explode.toDestroy = transform.parent.gameObject;
	}
}
