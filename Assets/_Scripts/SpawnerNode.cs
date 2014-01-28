using UnityEngine;
using System.Collections;

public class SpawnerNode : Node {

	public float rate = 1f;
	public Mover moverPrefab;

	public Node target;

	public GameObject receiverPrefab;

	public GameObject mesh;

	protected virtual void Start()
	{
		ObjectPool.CreatePool (moverPrefab);
		StartCoroutine (SpawnSequence());
	}

	public override void ReceivedMover (Mover m)
	{
		m.Splatter ();
		m.Recycle ();
	}

	public virtual void PickTarget()
	{
		target = neighbors [Random.Range (0, neighbors.Count)];
	}

	public virtual void Spawn()
	{
		Spawn (moverPrefab);
	}

	public virtual void Spawn(Mover pfb)
	{
		Mover m = pfb.Spawn(transform.position, transform.rotation);
		m.nextNode = this;
		m.Go (target);

		BroadcastMessage ("OnSpawn", SendMessageOptions.DontRequireReceiver);
	}

	public virtual void SetupReceivers()
	{
		for (int i = 0; i < neighbors.Count; i++) {
			GameObject go = (GameObject)Instantiate (receiverPrefab);
			go.transform.parent = transform;
			Vector3 diff = neighbors [i].transform.position - transform.position;
			diff.Normalize ();
			diff *= arrivalRadius * 0.9f;
			go.transform.localPosition = diff;
			go.transform.LookAt (neighbors [i].transform);
		}
	}

	public virtual IEnumerator SpawnSequence()
	{
		while (neighbors.Count < 1) {
			yield return null;
		}
		SetupReceivers ();
		for (;;) {
			PickTarget ();
			iTween.LookTo (mesh, target.transform.position, rate);
			yield return new WaitForSeconds (rate);
			Spawn();
		}
	}

}
