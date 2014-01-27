using UnityEngine;
using System.Collections.Generic;

public class NodeManager : MonoBehaviour {

	public static NodeManager Instance;

	public List<Node> nodes = new List<Node>();
	public List<NodeConnection> connections = new List<NodeConnection>();

	HexClosePackGrid _hcpg = new HexClosePackGrid();

	public GameObject spawnerPrefab;
	public GameObject connectionPrefab;
	public GameObject terminalPrefab;

	void Awake()
	{
		Instance = this;
	}

	public void BuildNetwork(int count)
	{
		HexClosePackCell zerocell = new HexClosePackCell ();
		_hcpg.grid.Add (new Vector3 (0, 0, 0), zerocell);
		
		Vector3 newspot = Vector3.zero;
		List<Vector3> keylist;
		for (int i = 40; i > 0;) {
			keylist = new List<Vector3>(_hcpg.grid.Keys);
			Vector3 oldspot = keylist[Random.Range(0, keylist.Count)];
			if (_hcpg.WalkToEmptyNeighbor(oldspot, out newspot)) {
				i--;
				HexClosePackCell newcell = new HexClosePackCell();
				_hcpg.grid.Add(newspot, newcell);
				connections.Add(new NodeConnection(_hcpg.grid[oldspot], newcell));
				_hcpg.grid[oldspot].value += 1;
				newcell.value += 1;
            }
        }

		List<Vector3> cellPositions = _hcpg.CellPositions;
		cellPositions.Sort (delegate(Vector3 x, Vector3 y) {
			return -_hcpg.grid[x].value.CompareTo(_hcpg.grid[y].value);
			});

		// Put the spawners in the most connected nodes
		for (int i = 0; i < 3; i++) {
			GameObject go = (GameObject)Instantiate(spawnerPrefab, _hcpg.GridToWorldPosition(cellPositions[i], 1f), Quaternion.identity);
			_hcpg.grid[cellPositions[i]].content = go;
			nodes.Add(go.GetComponent<Node>());
		}

		// For the rest, use normal nodes
		for (int i = 3; i < cellPositions.Count; i++) {
			GameObject pfb = connectionPrefab;
			if (_hcpg.grid[cellPositions[i]].value == 1) {
				pfb = terminalPrefab;
			}
			GameObject go = (GameObject)Instantiate(pfb, _hcpg.GridToWorldPosition(cellPositions[i], 1f), Quaternion.identity);
			_hcpg.grid[cellPositions[i]].content = go;
			nodes.Add(go.GetComponent<Node>());
		}
	}
}

public class NodeConnection {

	public HexClosePackCell primary;
	public HexClosePackCell secondary;

	public NodeConnection(HexClosePackCell p, HexClosePackCell s) {
		primary = p;
		secondary = s;
	}

}