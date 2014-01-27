using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {

	public Node nextNode;
	public Node previousNode;
	public float speed;

	public void Go(Node target) {
		previousNode = nextNode;
		nextNode = target;

		transform.LookAt (target.transform.position);
		rigidbody.velocity = Vector3.forward * speed;
	}

	void Update()
	{
		if (Vector3.Distance (transform.position, nextNode.transform.position) < 0.2f) {
			nextNode.ReceivedMover(this);
		}
	}

}
