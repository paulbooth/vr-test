using UnityEngine;
using System.Collections;

public class ObjectiveController : MonoBehaviour {

	public SpawnController spawnController;
	public MagicDoorController magicDoor;
	public float completeTime = 3f;

	private float allCompleteStartTime = 0f;
	private bool objectiveMet = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (objectiveMet) {
			return;
		}
		bool allComplete = AllRequirementsMet ();
		if (allComplete) {
			if (allCompleteStartTime > 0f) {
				if (Time.time >= allCompleteStartTime + completeTime) {
					objectiveMet = true;
//					spawnController.SpawnGun (Color.green, 10, 10, 0);
					magicDoor.Enable();
				}
			} else {
				allCompleteStartTime = Time.time;
			}
		} else {
			allCompleteStartTime = 0f;
		}
	}

	bool AllRequirementsMet()
	{
		bool allComplete = true;
		foreach (ObjectDetector o in GetComponentsInChildren<ObjectDetector>()) {
			if (!o.areRequirementsMet ()) {
				allComplete = false;
				break;
			}
		}
		return allComplete;
	}
}
