using UnityEngine;
using System.Collections;

public class NavigateObject : MonoBehaviour {

	public Transform tracked;
	public float updateInterval = 0.5f;
	public bool onlyUpright = true;

	private NavMeshAgent agent;
	private bool stopped = false;

	float lastUpdateTime = 0;

	public void StopTracking() {
		if (!stopped) {
			stopped = true;
			agent.Stop ();
		}
	}

	public void StartTracking() {
		if (stopped) {
			stopped = false;
			agent.Resume ();
		}
	}
		
	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent> ();
	}

//	void UpdateFixed () {
//		agent.SetDestination (tracked.transform.position);
//	}

	void Update() {
		if (!stopped && tracked && Time.time - lastUpdateTime > updateInterval) {
			agent.SetDestination (tracked.position);
			lastUpdateTime = Time.time;
		}
	}

}
