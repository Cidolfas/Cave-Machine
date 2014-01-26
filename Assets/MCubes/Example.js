#pragma strict

var m_material : Material;

private var m_perlin : PerlinNoise;
private var m_mesh : GameObject;

function Start () 
{
	m_perlin = new PerlinNoise(2);
	
	//Target is the value that represents the surface of mesh
	//For example the perlin noise has a range of -1 to 1 so the mid point is were we want the surface to cut through
	//The target value does not have to be the mid point it can be any value with in the range
	MarchingCubes.SetTarget(0.0);
	
	//Winding order of triangles use 2,1,0 or 0,1,2
	MarchingCubes.SetWindingOrder(2, 1, 0);
	
	//Set the mode used to create the mesh
	//Cubes is faster and creates less verts, tetrahedrons is slower and creates more verts but better represents the mesh surface
	//MarchingCubes.SetModeToCubes();
	MarchingCubes.SetModeToTetrahedrons();
	
	//The size of voxel array. Be carefull not to make it to large as a mesh in unity can only be made up of 65000 verts
	var width : int = 32;
	var height : int = 32;
	var length : int = 32;
	
	var voxels : float[,,] = new float[width, height, length];
	
	var x : int;
	var y : int;
	var z : int;
	
	//Fill voxels with values. Im using perlin noise but any method to create voxels will work
	for(x = 0; x < width; x++)
	{
		for(y = 0; y < height; y++)
		{
			for(z = 0; z < length; z++)
			{
				voxels[x,y,z] = m_perlin.FractalNoise3D(x, y, z, 3, 40.0, 1.0); 
			}
		}
	}
	
	var mesh : Mesh = MarchingCubes.CreateMesh(voxels);
	
	//The diffuse shader wants uvs so just fill with a empty array, there not actually used
	mesh.uv = new Vector2[mesh.vertices.Length];
	mesh.RecalculateNormals();
	
	m_mesh = new GameObject("Mesh");
	m_mesh.AddComponent(MeshFilter);
	m_mesh.AddComponent(MeshRenderer);
	m_mesh.renderer.material = m_material;
	m_mesh.GetComponent(MeshFilter).mesh = mesh;
	//Center mesh
	m_mesh.transform.localPosition = Vector3(-width/2, -height/2, -length/2);
	
	
}
