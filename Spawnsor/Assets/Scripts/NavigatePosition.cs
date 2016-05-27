using UnityEngine;
using System.Collections;

public class NavigatePosition : MonoBehaviour {

	public Vector3 destination;

	private NavMeshAgent agent;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent> ();
		agent.SetDestination (destination);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
