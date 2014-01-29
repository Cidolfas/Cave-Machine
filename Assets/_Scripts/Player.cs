using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public static Player Instance;

	float speed = 10f;
	public GameObject instructions;

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		Screen.lockCursor = true;
	}

	void Update()
	{
		if (Input.anyKey) {
			instructions.SetActive(false);
		}
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
