using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerminalNode : SpawnerNode {

	public Mover pingPrefab;
	public GameObject[] crystalPrefabs;
	public float minTime = 8f;
	public float maxTime = 25f;

	public List<GameObject> crystals = new List<GameObject>();

	public ParticleSystem absorbPS;

	protected override void Start ()
	{
		ObjectPool.CreatePool (pingPrefab);
		base.Start ();
	}

	public override void ReceivedMover (Mover m)
	{
		base.ReceivedMover (m);
		if (Random.value > 0.5f) {
			Spawn (pingPrefab);
		}
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
		Vector3 back = neighbors[0].transform.position - transform.position;
		back.Normalize();

		int count = Random.Range(4, 8);

		int giveup = 0;
		for (int i = 0; i < count;) {
			giveup++;
			if (giveup > 100) break;

			Vector3 dir = Random.onUnitSphere;
			if (Vector3.Dot(back, dir) > -0.1f) continue;

			RaycastHit hit;
			if (Physics.Raycast(transform.position, dir, out hit, 20f)) {
				i++;
				int p = Random.Range(0, crystalPrefabs.Length);
				GameObject go = (GameObject)Instantiate(crystalPrefabs[p], hit.point, Quaternion.identity);
				go.transform.up = hit.normal;
				crystals.Add(go);
			}
		}
	}

	public override IEnumerator SpawnSequence ()
	{
		while (neighbors.Count < 1) {
			yield return null;
		}
		target = neighbors [0];
		SetupReceivers ();
		SetupCrystals();
		yield return null;
		for (;;) {
			if (crystals.Count > 0) {
				int c = Random.Range(0, crystals.Count);
				iTween.LookTo(mesh, crystals[c].transform.position, 3f);
				iTween.LookTo(mesh, iTween.Hash("looktarget", crystals[c].transform.position, "time", 3f, "easetype", iTween.EaseType.linear));
				absorbPS.transform.position = crystals[c].transform.position;
				absorbPS.transform.LookAt(transform.position);
				float dist = Vector3.Distance(crystals[c].transform.position, transform.position);
				absorbPS.startLifetime = (dist-0.5f) / absorbPS.startSpeed;
			}
			yield return new WaitForSeconds (3f);
			rate = Random.Range(minTime * 2f, maxTime * 2f);
			rate = Mathf.Ceil(rate) / 2f;
			light.intensity = 3f;
			GetComponent<SoundEntity>().mode = 1;
			if (crystals.Count > 0) {
				absorbPS.Play();
			}
			float t = Time.time;
			while(Time.time < t + rate) {
				float tsq = (Time.time - t) * 10f;
				light.intensity = 4f + Mathf.Sin(tsq) * 0.5f;
				yield return null;
			}
			light.intensity = 1f;
			absorbPS.Stop();
			rate = Mathf.Ceil(absorbPS.startLifetime * 2f) / 2f;
			yield return new WaitForSeconds (absorbPS.startLifetime);
			GetComponent<SoundEntity>().mode = 0;
			yield return new WaitForSeconds(rate - absorbPS.startLifetime);
			iTween.LookTo(mesh, iTween.Hash("looktarget", target.transform.position, "time", 3f, "easetype", iTween.EaseType.linear));
			yield return new WaitForSeconds (3f);
			int n = Random.Range(1, 5);
			for (int i = 0; i < n; i++) {
				Spawn();
				yield return new WaitForSeconds (0.5f);
			}
		}
	}

}
