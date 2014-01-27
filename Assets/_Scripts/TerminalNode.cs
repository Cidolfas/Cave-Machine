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

	public override void SetupReceivers()
	{
		GameObject go = (GameObject)Instantiate (receiverPrefab);
		go.transform.parent = transform;
		Vector3 diff = neighbors [0].transform.position - transform.position;
		diff.Normalize ();
		diff *= arrivalRadius * 0.9f;
		go.transform.localPosition = diff;
		go.transform.LookAt (neighbors [0].transform);
	}

	public void SetupCrystals()
	{

	}

	public override IEnumerator SpawnSequence ()
	{
		while (neighbors.Count < 1) {
			yield return null;
		}
		target = neighbors [0];
		SetupReceivers ();
		for (;;) {
			rate = Random.Range(minTime, maxTime);
			yield return new WaitForSeconds (rate);
			Spawn();
		}
	}

}
