using UnityEngine;
using System.Collections;

public class RecycleParticleSystem : MonoBehaviour {

	ParticleSystem ps;

	void Start()
	{
		ps = particleSystem;
	}

	void Update()
	{
		if (!ps.isPlaying) {
			ps.Recycle();
		}
	}

}
