using UnityEngine;
using System.Collections.Generic;

public class TESTspitGrid : MonoBehaviour {

	public GameObject nodePrefab;
	public GameObject connectionPrefab;
	public GameObject meshPrefab;
	public float scalar = 10f;
	public float connectionThickness = 1f;
	public float shakeAmount = 0.2f;

	public float innerThickness = 5f;
	public float outerThickness = 10f;
	public float noiseAmount = 0f;
	public float marchingCubesThreshold = 0.5f;

	HexClosePackGrid _hcpg = new HexClosePackGrid();
	public int voxelCountPerChunk = 16;
	public int chunkStride = 2;
	public float voxelResolution = 2f; // units per point
	float[,,] voxels;

	public int octNum = 3;
	public float noiseFreq = 10f;

	void Start()
	{
		int v = voxelCountPerChunk * chunkStride;
		voxels = new float[v, v, v];

		HexClosePackCell zerocell = new HexClosePackCell ();
		zerocell.content = (GameObject)Instantiate (nodePrefab, Vector3.zero, Quaternion.identity);
		_hcpg.grid.Add (new Vector3 (0, 0, 0), zerocell);

		Vector3 newspot = Vector3.zero;
		List<Vector3> keylist;
		for (int i = 40; i > 0;) {
			keylist = new List<Vector3>(_hcpg.grid.Keys);
			Vector3 oldspot = keylist[Random.Range(0, keylist.Count)];
			if (_hcpg.WalkToEmptyNeighbor(oldspot, out newspot)) {
				i--;
				HexClosePackCell newcell = new HexClosePackCell();
				Vector3 shake = Random.insideUnitSphere * shakeAmount * scalar;
				newcell.content = (GameObject)Instantiate(nodePrefab, newspot * scalar + shake, Quaternion.identity);
				_hcpg.grid.Add(newspot, newcell);
				BuildConnection(_hcpg.grid[oldspot].content.transform.position, newcell.content.transform.position);
			}
		}

//		BuildConnection (Vector3.one * 20f, Vector3.one * -20f);

		AddNoiseToVoxels ();
		SealEdges ();
		MarchingCubesGo ();
	}

	void BuildConnection(Vector3 start, Vector3 end)
	{
		Vector3 midpoint = (start + end) / 2f;
		Vector3 diff = midpoint - start;
		float dist = diff.magnitude;

		Quaternion look = Quaternion.LookRotation (diff);

		GameObject go = (GameObject)Instantiate (connectionPrefab, midpoint, look);
		Mesh msh = new Mesh ();
		Vector3 p0 = Vector3.zero;
		Vector3 p1 = new Vector3 (connectionThickness, 0f, dist);
		Vector3 p2 = new Vector3 (0f, connectionThickness, dist);
		Vector3 p3 = new Vector3 (-connectionThickness, 0f, dist);
		Vector3 p4 = new Vector3 (0f, -connectionThickness, dist);
		Vector3[] verts = new Vector3[]{p0, p4, p3, p2, p1, -p4, -p3, -p2, -p1};
		int[] tris = new int[]{0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 1, 0, 6, 5, 0, 7, 6, 0, 8, 7, 0, 5, 8};
		Vector2 v = Vector2.zero;
		Vector2[] uvs = new Vector2[] {v, v, v, v, v, v, v, v, v};
		msh.vertices = verts;
		msh.triangles = tris;
		msh.uv = uvs;
		msh.RecalculateNormals();
		msh.RecalculateBounds();
		go.GetComponent<MeshFilter> ().mesh = msh;

		RasterizeLine (start, end, innerThickness, outerThickness);
	}

	void RasterizeLine(Vector3 start, Vector3 end, float iThick, float oThick)
	{
		if (oThick < iThick) {
			oThick = iThick;
		}

		Vector3 minExtantPos = Vector3.one * voxelCountPerChunk * chunkStride * 0.5f * -voxelResolution;
		Vector3 stepX = Vector3.right * voxelResolution;
		Vector3 stepY = Vector3.up * voxelResolution;
		Vector3 stepZ = Vector3.forward * voxelResolution;

		Vector3 currentPos = Vector3.zero;
		float dist = 0f;
		int v = voxelCountPerChunk * chunkStride;
		for (int x = 0; x < v; x++) {
			for (int y = 0; y < v; y++) {
				for (int z = 0; z < v; z++) {
					currentPos = minExtantPos + stepX * x + stepY * y + stepZ * z;
					dist = PointToLineSegmentDist(start, end, currentPos);
					if (dist <= iThick) {
						voxels[x, y, z] = 1f;
					} else if (dist <= oThick) {
						voxels[x, y, z] = Mathf.Max(1f - ((dist - iThick) / (oThick - iThick)), voxels[x, y, z]);
					}
				}
			}
		}
	}

	float PointToLineSegmentDist(Vector3 s1, Vector3 s2, Vector3 p)
	{
		if (s1 == s2) {
			return Vector3.Distance(s1, p);
		}

		Vector3 v = s2 - s1;
		Vector3 w = p - s1;

		float c1 = Vector3.Dot (w, v);
		if (c1 <= 0f) {
			return Vector3.Distance(p, s1);
		}

		float c2 = Vector3.Dot (v, v);
		if (c2 <= c1) {
			return Vector3.Distance(p, s2);
		}

		float t = c1 / c2;
		Vector3 proj = s1 + t * v;
		return Vector3.Distance (p, proj);
	}

	void AddNoiseToVoxels()
	{
		if (noiseAmount < 0f) return;

		PerlinNoise noise = new PerlinNoise (Random.seed);

		int v = voxelCountPerChunk * chunkStride;
		for (int i = 0; i < v; i++) {
			for (int j = 0; j < v; j++) {
				for (int k = 0; k < v; k++) {
					if (voxels[i, j, k] == 0f) continue;
					voxels[i, j, k] += noise.FractalNoise3D(i, j, k, octNum, noiseFreq, 1f) * noiseAmount;
					voxels[i, j, k] = Mathf.Clamp01(voxels[i, j, k]);
				}
			}
		}
	}

	void SealEdges()
	{
		int v = voxelCountPerChunk * chunkStride;
		for (int i = 0; i < v; i++) {
			for (int j = 0; j < v; j++) {
				for (int k = 0; k < v; k++) {
					if (i == 0 || i == v-1 || j == 0 || j == v-1 || k == 0 || k == v-1) {
						voxels[i, j, k] = 0f;
					}
                }
            }
        }
	}

	void MarchingCubesGo()
	{
		MarchingCubes.SetTarget (marchingCubesThreshold);
		MarchingCubes.SetWindingOrder(2, 1, 0);
		MarchingCubes.SetModeToTetrahedrons();
		MarchingCubes.SetScalar (voxelResolution);

		Vector3 minExtantPos = Vector3.one * voxelCountPerChunk * chunkStride * -0.5f * voxelResolution;

		for (int i = 0; i < chunkStride; i++) {
			for (int j = 0; j < chunkStride; j++) {
				for (int k = 0; k < chunkStride; k++) {
					Vector3 start = new Vector3(i, j, k);
					Vector3 end = start + Vector3.one;
					start *= voxelCountPerChunk;
					if (i > 0) start.x -= 1;
					if (j > 0) start.y -= 1;
					if (k > 0) start.z -= 1;
					end *= voxelCountPerChunk;
					end -= Vector3.one;
					Mesh msh = MarchingCubes.CreateMesh (voxels, start, end);
					msh.uv = new Vector2[msh.vertices.Length];
					msh.RecalculateNormals();
					msh.RecalculateBounds();
					GameObject go = (GameObject)Instantiate (meshPrefab);
					go.transform.position = minExtantPos;
					go.GetComponent<MeshFilter> ().mesh = msh;
					go.GetComponent<MeshCollider>().sharedMesh = msh;
				}
			}
		}
	}

	void OnDrawGizmosSelected()
	{
		if (voxels == null) return;

		int v = voxelCountPerChunk * chunkStride;
		Vector3 minExtant = Vector3.one * v * -0.5f * voxelResolution;

		for (int x = 0; x < v; x++) {
			for (int y = 0; y < v; y++) {
				for (int z = 0; z < v; z++) {
					float t = voxels[x, y, z];
					if (t == 0f) continue;
					if (t == 1f) {
						Gizmos.color = Color.red;
					} else {
						Gizmos.color = Color.Lerp(Color.white, Color.yellow, t);
					}
					Vector3 pos = new Vector3(x, y, z);
					Gizmos.DrawSphere(pos * voxelResolution + minExtant, 0.2f);
				}
			}
		}
	}
}
