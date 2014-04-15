using UnityEngine;
using System.Collections;

public class ConnectorGrow : MonoBehaviour {

	// Public inspector variables
	public Color lowColor;
	public Color highColor;
	public float lowIntensity;
	public float highIntensity;
	public float lowScale;
	public float highScale;
	public float decay;
	public float current;

	// Private variables
	private Light l;
	private Material m;
	private SoundEntity e;

	void Start()
	{
		// Cache variables
		l = light;
		m = renderer.material;
		e = GetComponent<SoundEntity> ();
	}

	void OnReceived()
	{
		// Node goes to full strength
		current = 1f;
	}

	void LateUpdate()
	{
		// Update sphere and light
		Color c = Color.Lerp (lowColor, highColor, current);

		l.color = c;
		l.intensity = Mathf.Lerp (lowIntensity, highIntensity, current);
		m.color = c;
		transform.localScale = Vector3.one * Mathf.Lerp (lowScale, highScale, current);

		// Update sound
		e.strength = current;

		// Update current strength
		current = Mathf.Clamp01 (current - Time.deltaTime * decay);
		current = Mathf.Max (current, (Mathf.Sin (Time.time / 2f) + 1f) * 0.1f);
	}

}
