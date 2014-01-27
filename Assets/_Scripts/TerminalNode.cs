using UnityEngine;
using System.Collections;

public class TerminalNode : SpawnerNode {

	public GameObject pingPrefab;
	public float minTime = 8f;
	public float maxTime = 25f;

	public override void ReceivedMover (Mover m)
	{
		Destroy (m.gameObject);
		Spawn (pingPrefab);
	}

	public override IEnumerator SpawnSequence ()
	{
		while (neighbors.Count < 1) {
			yield return null;
		}
		target = neighbors [0];
		for (;;) {
			rate = Random.Range(minTime, maxTime);
			yield return new WaitForSeconds (rate);
			Spawn();
		}
	}

}
