using System;
using System.Collections.Generic;
using UnityEngine;

public class ShootDeform : MonoBehaviour
{
	[SerializeField] private float _depthShoot = 0.2f;
	[SerializeField] private float _distDeform = 0.2f;

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

		FilterAction(hit.collider.gameObject, hit.point, hit.normal, hit.triangleIndex);
	}

	private void FilterAction(GameObject targetObject, Vector3 hitPoint, Vector3 normal, int hitTriangleIndex)
	{
		MeshFilter meshFilter = targetObject.GetComponent<MeshFilter>();
		MeshCollider meshCollider = targetObject.GetComponent<MeshCollider>();
		meshCollider.convex = false;

		if (hitTriangleIndex < 0) return;

		Mesh mesh = meshFilter.mesh;
		Vector3[] vertices = mesh.vertices;
		int[] triangles = mesh.triangles;

		List<Vector3> modifiedVertices = new List<Vector3>(vertices);
		List<int> modifiedTriangles = new List<int>(triangles);

		//--------------------------
		// Get the vertices of the impacted face
		UnityEngine.Debug.Log($"{triangles.Length} / {hitTriangleIndex*3}");
		int index0 = triangles[hitTriangleIndex * 3];
		int index1 = triangles[hitTriangleIndex * 3 + 1];
		int index2 = triangles[hitTriangleIndex * 3 + 2];

		bool change = false;
		Vector3 modifiedVerticePos = modifiedVertices[index0];
		if (MoveVertice(ref modifiedVerticePos, hitPoint, normal, targetObject)) change = true;
		modifiedVertices[index0] = modifiedVerticePos;

		modifiedVerticePos = modifiedVertices[index1];
		if (MoveVertice(ref modifiedVerticePos, hitPoint, normal, targetObject)) change = true;
		modifiedVertices[index1] = modifiedVerticePos;

		modifiedVerticePos = modifiedVertices[index2];
		if (MoveVertice(ref modifiedVerticePos, hitPoint, normal, targetObject)) change = true;
		modifiedVertices[index2] = modifiedVerticePos;



		if (!change) DeformMesh(meshFilter, meshCollider, hitPoint, normal, hitTriangleIndex);
		else
		{
			mesh.vertices = modifiedVertices.ToArray();
			mesh.triangles = modifiedTriangles.ToArray();
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();

			meshCollider.sharedMesh = null;
			meshCollider.sharedMesh = mesh;
		}
		meshCollider.convex = true;
	}

	private bool MoveVertice(ref Vector3 verticePos, Vector3 hitPoint, Vector3 normal, GameObject targetObject)
	{
		if (Vector3.Distance(targetObject.transform.TransformPoint(verticePos), hitPoint) <= _distDeform)
		{
			Vector3 localHitPoint = verticePos;
			Vector3 localNormal = targetObject.transform.InverseTransformDirection(normal);
			Vector3 newVertex = localHitPoint - (localNormal * _depthShoot);

			verticePos = newVertex;
			return true;
		}
		else
			return false;
	}

	private void DeformMesh(MeshFilter filter, MeshCollider col, Vector3 hitPoint, Vector3 normal, int hitTriangleIndex)
	{
		Mesh mesh = filter.mesh;
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
		Vector3 localHitPoint = col.gameObject.transform.InverseTransformPoint(hitPoint);
		Vector3 localNormal = col.gameObject.transform.InverseTransformDirection(normal);
		Vector3 newVertex = localHitPoint - (localNormal * _depthShoot);

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

		col.sharedMesh = null;
		col.sharedMesh = mesh;
	}
}