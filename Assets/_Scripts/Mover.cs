using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {

	public ParticleSystem splatterPSPrefab;

	public Node nextNode;
	public Node previousNode;
	public float speed;

	void Start()
	{
		ObjectPool.CreatePool (splatterPSPrefab);
	}

	public void Go(Node target) {
		previousNode = nextNode;
		nextNode = target;

		if (previousNode != null) {
			transform.position = previousNode.transform.position;
		}

		transform.LookAt (target.transform.position);
		rigidbody.velocity = transform.forward * speed;
	}

	public void Splatter()
	{
		splatterPSPrefab.Spawn (transform.position, transform.rotation);
	}

	void Update()
	{
		if (Vector3.Distance (transform.position, nextNode.transform.position) < nextNode.arrivalRadius) {
			nextNode.ReceivedMover(this);
		}

		transform.RotateAround (transform.position, transform.forward, Time.deltaTime * speed * 40f);
	}

}
