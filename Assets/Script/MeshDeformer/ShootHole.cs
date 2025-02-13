using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootHole : MonoBehaviour
{
	[SerializeField] private float _dentDepth = 0.2f;

	private void Update()
	{
		Shoot();
	}

	private void Shoot()
	{
		if (!Input.GetMouseButtonDown(0)) return;

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (!Physics.Raycast(ray, out RaycastHit hit)) return;

		MeshFilter meshFilter = hit.collider.GetComponent<MeshFilter>();
		MeshCollider meshCollider = hit.collider.GetComponent<MeshCollider>();

		if (meshFilter == null || meshCollider == null) return;

		DeformMesh(hit.collider.gameObject, hit.point, hit.normal, hit.triangleIndex);
	}

	private void DeformMesh(GameObject targetObject, Vector3 hitPoint, Vector3 normal, int hitTriangleIndex)
	{
		MeshFilter meshFilter = targetObject.GetComponent<MeshFilter>();
		MeshCollider meshCollider = targetObject.GetComponent<MeshCollider>();

		Mesh mesh = meshFilter.mesh;
		Vector3[] vertices = mesh.vertices;
		int[] triangles = mesh.triangles;

		List<Vector3> modifiedVertices = new List<Vector3>(vertices);
		List<int> modifiedTriangles = new List<int>(triangles);

		//--------------------------
		// Get the vertices of the impacted face
		int index0 = triangles[hitTriangleIndex * 3];
		int index1 = triangles[hitTriangleIndex * 3 + 1];
		int index2 = triangles[hitTriangleIndex * 3 + 2];

		//--------------------------
		// Remove the exact triangle
		for (int i = 0; i < modifiedTriangles.Count; i += 3)
		{
			if ((modifiedTriangles[i] == index0 && modifiedTriangles[i + 1] == index1 && modifiedTriangles[i + 2] == index2) ||
				(modifiedTriangles[i] == index1 && modifiedTriangles[i + 1] == index2 && modifiedTriangles[i + 2] == index0) ||
				(modifiedTriangles[i] == index2 && modifiedTriangles[i + 1] == index0 && modifiedTriangles[i + 2] == index1))
			{
				modifiedTriangles.RemoveRange(i, 3);
				break;
			}
		}

		// new vertice
		Vector3 localHitPoint = targetObject.transform.InverseTransformPoint(hitPoint);
		Vector3 localNormal = targetObject.transform.InverseTransformDirection(normal);
		Vector3 newVertex = localHitPoint - (localNormal * _dentDepth);

		int newVertexIndex = modifiedVertices.Count;
		modifiedVertices.Add(newVertex);

		//--------------------------
		// create face
		modifiedTriangles.Add(index0);
		modifiedTriangles.Add(index1);
		modifiedTriangles.Add(newVertexIndex);

		modifiedTriangles.Add(index1);
		modifiedTriangles.Add(index2);
		modifiedTriangles.Add(newVertexIndex);

		modifiedTriangles.Add(index2);
		modifiedTriangles.Add(index0);
		modifiedTriangles.Add(newVertexIndex);

		//--------------------------
		// upd mesh and col
		mesh.vertices = modifiedVertices.ToArray();
		mesh.triangles = modifiedTriangles.ToArray();
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();

		meshCollider.sharedMesh = null;
		meshCollider.sharedMesh = mesh;
	}
}