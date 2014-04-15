using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {

	// Public inspector variables
	public ParticleSystem splatterPSPrefab;
	
	public Node nextNode;
	public Node previousNode;
	public float speed;

	void Start()
	{
		// Make sure we have a pool for the death particles
		ObjectPool.CreatePool (splatterPSPrefab);
	}

	public void Go(Node target) {
		// Set targets
		previousNode = nextNode;
		nextNode = target;

		if (previousNode != null) {
			transform.position = previousNode.transform.position;
		}

		// Get us moving
		transform.LookAt (target.transform.position);
		rigidbody.velocity = transform.forward * speed;
	}

	public void Splatter()
	{
		// SPLAT!
		splatterPSPrefab.Spawn (transform.position, transform.rotation);
	}

	void Update()
	{
		// Are we there yet?
		if (Vector3.Distance (transform.position, nextNode.transform.position) < nextNode.arrivalRadius) {
			nextNode.ReceivedMover(this);
		}

		// Spin
		transform.RotateAround (transform.position, transform.forward, Time.deltaTime * speed * 40f);
	}

}
