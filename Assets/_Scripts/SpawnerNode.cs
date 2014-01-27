using UnityEngine;
using System.Collections;

public class SpawnerNode : Node {

	public float rate = 1f;
	public GameObject prefab;

	public Node target;

	public GameObject receiverPrefab;

	public GameObject mesh;

	protected virtual void Start()
	{
		StartCoroutine (SpawnSequence());
	}

	public override void ReceivedMover (Mover m)
	{
		Destroy (m.gameObject);
	}

	public virtual void PickTarget()
	{
		target = neighbors [Random.Range (0, neighbors.Count)];
	}

	public virtual void Spawn()
	{
		Spawn (prefab);
	}

	public virtual void Spawn(GameObject pfb)
	{
		GameObject go = (GameObject)Instantiate (pfb, transform.position, transform.rotation);
		Mover m = go.GetComponent<Mover> ();
		m.nextNode = this;
		m.Go (target);

		SendMessage ("OnSpawn", SendMessageOptions.DontRequireReceiver);
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
