using UnityEngine;
using System.Collections;

public class SinLight : MonoBehaviour {

	float intensity = 1f;
	float offset = 0f;
	public float period = 5f;
	public float strength = 0.3f;
	Light l;

	void Start()
	{
		l = light;
		intensity = l.intensity;
		offset = Random.Range (0f, 2f * Mathf.PI);
	}

	void Update()
	{
		l.intensity = intensity * (1f + Mathf.Sin (Time.time / period * 2f * Mathf.PI + offset) * strength);
	}

}
