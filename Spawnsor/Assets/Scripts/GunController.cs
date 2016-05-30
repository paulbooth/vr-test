using UnityEngine;
using System.Collections;
using NewtonVR;

public class GunController : NVRInteractableItem
{
	public GameObject BulletPrefab;

	public Transform FirePoint;

	public Vector3 BulletForce = new Vector3(0, 0, 700);

	public Color BulletColor = Color.gray;

	public override void UseButtonDown()
	{
		base.UseButtonDown();

		GameObject bullet = GameObject.Instantiate(BulletPrefab);
		Renderer renderer = bullet.GetComponent<Renderer> ();
		if (renderer) {
			renderer.material.color = BulletColor;
		}
		bullet.transform.position = FirePoint.position;
		bullet.transform.forward = FirePoint.forward;

		bullet.GetComponent<Rigidbody>().AddRelativeForce(BulletForce);
	}
}