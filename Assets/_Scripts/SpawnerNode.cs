using UnityEngine;
using System.Collections;

public class SpawnerNode : Node {

	public float rate = 1f;
	public GameObject prefab;

	public Node target;

	public virtual void Start()
	{
		StartCoroutine (SpawnSequence());
	}

	public virtual void PickTarget()
	{
		target = neighbors [Random.Range (0, neighbors.Count)];
	}

	public virtual void Spawn()
	{
		GameObject go = (GameObject)Instantiate (prefab, transform.position, transform.rotation);
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
			iTween.LookTo (gameObject, target.gameObject, rate);
			yield return new WaitForSeconds (rate);
		}
	}

}
