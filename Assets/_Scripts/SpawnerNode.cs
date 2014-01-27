using UnityEngine;
using System.Collections;

public class SpawnerNode : Node {

	public float rate = 1f;
	public GameObject prefab;

	public Node target;

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
		m.Go (target);

		SendMessage ("OnSpawn", SendMessageOptions.DontRequireReceiver);
	}

	public virtual IEnumerator SpawnSequence()
	{
		while (neighbors.Count < 1) {
			yield return null;
		}
		for (;;) {
			PickTarget ();
			iTween.LookTo (gameObject, target.transform.position, rate);
			yield return new WaitForSeconds (rate);
			Spawn();
		}
	}

}
