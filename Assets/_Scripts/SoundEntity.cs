using UnityEngine;
using System.Collections;

public class SoundEntity : MonoBehaviour {

	public SoundType type;
	public int mode;

	[Range(0f, 1f)]
	public float strength = 1f;

	void OnEnable()
	{
		for (int i = 0; i < SoundMaster.MasterList.Count; i++) {
			SoundMaster m = SoundMaster.MasterList[i];
			if (m.type == type) {
				m.entities.Add(this);
			}
		}
	}

	void OnDisable()
	{
		for (int i = 0; i < SoundMaster.MasterList.Count; i++) {
			SoundMaster m = SoundMaster.MasterList[i];
			if (m.type == type) {
				m.entities.Remove(this);
				if (m.currentClosest == this) {
					m.currentClosest = null;
				}
			}
		}
	}

}
