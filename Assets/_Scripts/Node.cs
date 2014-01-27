using UnityEngine;
using System.Collections.Generic;

public class Node : MonoBehaviour {

	public List<Node> neighbors = new List<Node>();

	public virtual void ReceivedMover(Mover m)
	{
		if (neighbors.Count <= 1) {
			Destroy (m.gameObject);
		} else {
			if (m.previousNode != null) {
				int i = neighbors.IndexOf(m.previousNode);
				int r = Random.Range(0, neighbors.Count-1);
				if (r >= i) {
					r++;
				}
				m.Go(neighbors[r]);
			} else {
				int r = Random.Range(0, neighbors.Count);
				m.Go(neighbors[r]);
			}
		}

		SendMessage ("OnReceived", m, SendMessageOptions.DontRequireReceiver);
	}
}
