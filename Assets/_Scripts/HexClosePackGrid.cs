using UnityEngine;
using System.Collections.Generic;

public class HexClosePackGrid {
	
	// Key for neighbors, XZ planar, +Z is north, clockwise starting at 9 O'Clock:
	// 0, 1, 2 - Below
	// 3, 4, 5, 6, 7, 8 - Sides
	// 9, 10, 11 - Top
	
	public Dictionary<Vector3, HexClosePackCell> grid = new Dictionary<Vector3, HexClosePackCell>();
	
	private List<int> _neighborDirections = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 });

	public List<HexClosePackCell> Cells {
		get { return new List<HexClosePackCell> (grid.Values); }
	}

	public List<Vector3> CellPositions {
		get { return new List<Vector3> (grid.Keys); }
	}
	
	public bool WalkToEmptyNeighbor(Vector3 start, out Vector3 end)
	{
		List<int> neighbors = new List<int>(_neighborDirections);
		
		while(neighbors.Count > 0) {
			int r = Random.Range(0, neighbors.Count);
			int d = neighbors[r];
			neighbors.RemoveAt(r);
			
			Vector3 v = CoordinateShift(start, d);
			if (!grid.ContainsKey(v)) {
				end = v;
				return true;
			}
		}
		
		end = Vector3.zero;
		return false;
	}
	
	public bool WalkToBestNeighbor(Vector3 start, out Vector3 end, int target, int min, int max)
	{
		SortedList<int, List<Vector3>> scores = new SortedList<int, List<Vector3>>();
		
		foreach (int d in _neighborDirections) {
			Vector3 v = CoordinateShift(start, d);
			if (grid.ContainsKey(v)) {
				continue;
			}
			int s = GetNeighborCount(v);
			if (s < min || s > max) {
				continue;
			}
			s = Mathf.Abs(s-target);
			if (!scores.ContainsKey(s)) {
				scores.Add(s, new List<Vector3>());
			}
			scores[s].Add(v);
		}
		
		if (scores.Count == 0) {
			end = Vector3.zero;
			return false;
		}
		
		int n = Random.Range(0, scores.Values[0].Count);
		end = scores.Values[0][n];
		return true;
	}
	
	public int GetNeighborCount(Vector3 start)
	{
		int c = 0;
		foreach (int d in _neighborDirections) {
			if (grid.ContainsKey(CoordinateShift(start, d))) {
				c++;
			}
		}
		
		return c;
	}
	
	public bool IsEven(int i)
	{
		if (i == 0) return true;
		
		return Mathf.Abs(i) % 2 == 0;
	}
	
	public bool IsEven(float f)
	{
		return IsEven(Mathf.FloorToInt(f));
	}
	
	public Vector3 GridToWorldPosition(Vector3 input, float r)
	{
		Vector3 output = Vector3.zero;
		float zoff = Mathf.Sqrt(3f);
		float yoff = Mathf.Sqrt(6f) * 2f / 3f;
		
		// X position
		output.x = input.x * 2f * r;
		if (!IsEven(input.z)) { output.x += r; }
		if (!IsEven(input.y)) { output.x += r; }
		
		// Z position
		output.z = input.z * zoff * r;
		if (!IsEven(input.y)) { output.z += zoff * r / 3f; }
		
		// Y position
		output.y = input.y * yoff * r;
		
		return output;
	}
	
	public Vector3 CoordinateShift(Vector3 input, int direction)
	{
		if (IsEven(input.y)) {
			if (IsEven(input.z)) {
				switch(direction) {
				case 0:
					input += new Vector3(-1, -1, 0);
					break;
				case 1:
					input += new Vector3(0, -1, 0);
					break;
				case 2:
					input += new Vector3(-1, -1, -1);
					break;
				case 3:
					input += new Vector3(-1, 0, 1);
					break;
				case 4:
					input += new Vector3(0, 0, 1);
					break;
				case 5:
					input += new Vector3(1, 0, 0);
					break;
				case 6:
					input += new Vector3(0, 0, -1);
					break;
				case 7:
					input += new Vector3(-1, 0, -1);
					break;
				case 8:
					input += new Vector3(-1, 0, 0);
					break;
				case 9:
					input += new Vector3(-1, 1, 0);
					break;
				case 10:
					input += new Vector3(0, 1, 0);
					break;
				case 11:
					input += new Vector3(-1, 1, -1);
					break;
				}
			} else {
				switch(direction) {
				case 0:
					input += new Vector3(-1, -1, 0);
					break;
				case 1:
					input += new Vector3(0, -1, 0);
					break;
				case 2:
					input += new Vector3(0, -1, -1);
					break;
				case 3:
					input += new Vector3(0, 0, 1);
					break;
				case 4:
					input += new Vector3(1, 0, 1);
					break;
				case 5:
					input += new Vector3(1, 0, 0);
					break;
				case 6:
					input += new Vector3(1, 0, -1);
					break;
				case 7:
					input += new Vector3(0, 0, -1);
					break;
				case 8:
					input += new Vector3(-1, 0, 0);
					break;
				case 9:
					input += new Vector3(-1, 1, 0);
					break;
				case 10:
					input += new Vector3(0, 1, 0);
					break;
				case 11:
					input += new Vector3(0, 1, -1);
					break;
				}
			}
		} else {
			if (IsEven(input.z)) {
				switch(direction) {
				case 0:
					input += new Vector3(0, -1, 1);
					break;
				case 1:
					input += new Vector3(1, -1, 0);
					break;
				case 2:
					input += new Vector3(0, -1, 0);
					break;
				case 3:
					input += new Vector3(-1, 0, 1);
					break;
				case 4:
					input += new Vector3(0, 0, 1);
					break;
				case 5:
					input += new Vector3(1, 0, 0);
					break;
				case 6:
					input += new Vector3(0, 0, -1);
					break;
				case 7:
					input += new Vector3(-1, 0, -1);
					break;
				case 8:
					input += new Vector3(-1, 0, 0);
					break;
				case 9:
					input += new Vector3(0, 1, 1);
					break;
				case 10:
					input += new Vector3(1, 1, 0);
					break;
				case 11:
					input += new Vector3(0, 1, 0);
					break;
				}
			} else {
				switch(direction) {
				case 0:
					input += new Vector3(1, -1, 1);
					break;
				case 1:
					input += new Vector3(1, -1, 0);
					break;
				case 2:
					input += new Vector3(0, -1, 0);
					break;
				case 3:
					input += new Vector3(0, 0, 1);
					break;
				case 4:
					input += new Vector3(1, 0, 1);
					break;
				case 5:
					input += new Vector3(1, 0, 0);
					break;
				case 6:
					input += new Vector3(1, 0, -1);
					break;
				case 7:
					input += new Vector3(0, 0, -1);
					break;
				case 8:
					input += new Vector3(-1, 0, 0);
					break;
				case 9:
					input += new Vector3(1, 1, 1);
					break;
				case 10:
					input += new Vector3(1, 1, 0);
					break;
				case 11:
					input += new Vector3(0, 1, 0);
					break;
				}
			}
		}
		
		return input;
	}
	
}

public class HexClosePackCell {
	
	public GameObject content;

	public float value = 0f;
	
}