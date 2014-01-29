using UnityEngine;
using System.Collections.Generic;

public enum SoundType { None, Spawner, Port, Connection, Miner, Data, Ping, Command }

public class SoundMaster : MonoBehaviour {

	public static List<SoundMaster> MasterList = new List<SoundMaster> ();

	public SoundType type = SoundType.None;

	[Range(0f, 1f)]
	public float maxVolume = 1f;

	[Range(0f, 1f)]
	public float minVolume = 0f;

	public float innerRadius = 5f;
	public float outerRadius = 20f;

	public List<SoundEntity> entities = new List<SoundEntity>();
	public SoundEntity currentClosest;

	public List<AudioSource> sources;

	void Awake()
	{
		MasterList.Add (this);
	}

	void Update()
	{
		if (sources.Count == 0) return;
		if (entities.Count == 0) return;
		if (currentClosest == null) currentClosest = entities[0];

		// Find the closest
		Vector3 playerPos = Player.Instance.transform.position;
		SoundEntity closest = currentClosest;
		float distSq = Vector3.SqrMagnitude (currentClosest.transform.position - playerPos);
		for (int i = 0; i < entities.Count; i++) {
			SoundEntity e = entities[i];
			float d = Vector3.SqrMagnitude (e.transform.position - playerPos);
			if (d < distSq) {
				closest = e;
				distSq = d;
			}
		}
		currentClosest = closest;

		int mode = currentClosest.mode;
		if (mode < 0 || mode > sources.Count - 1) {
			mode = 0;
		}
		for (int i = 0; i < sources.Count; i++) {
			sources[i].volume = 0f;
		}

		if (distSq < outerRadius * outerRadius) {
			if (distSq <= innerRadius * innerRadius) {
				sources[mode].volume = maxVolume * currentClosest.strength;
			} else {
				float d = Vector3.Distance(closest.transform.position, playerPos);
				float v = 1f - (d - innerRadius) / (outerRadius - innerRadius); // 1 when close, 0 when far
				v = Mathf.Lerp(minVolume, maxVolume, v);
				sources[mode].volume = v * currentClosest.strength;
			}
		} else {
			sources[mode].volume = minVolume;
		}
	}
}
