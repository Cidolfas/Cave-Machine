using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	float speed = 10f;

	void Start()
	{
		Screen.lockCursor = true;
	}

	void FixedUpdate()
	{
		float forward = Input.GetAxis ("Vertical");
		float side = Input.GetAxis ("Horizontal");
		float up = Input.GetAxis ("UpDown");

		Vector3 dir = transform.forward * forward + transform.right * side + transform.up * up;

		rigidbody.velocity = dir * speed;
	}

}
