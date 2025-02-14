using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutObject : MonoBehaviour
{
	private List<MeshCollider> _storedCollider = new();
	private List<MeshFilter> _storedMeshVisu = new();
	private List<Vector3> _hitPoint = new();

	private void Update()
	{
		Shoot();
	}

	private void Shoot()
	{
		if (!Input.GetMouseButton(1))
		{
			DoTheCut();
			return;
		}

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (!Physics.Raycast(ray, out RaycastHit hit)) return;

		RegisterCutInfo(hit);
	}

	private void RegisterCutInfo(RaycastHit hit)
	{
		MeshFilter filter = hit.collider.GetComponent<MeshFilter>();
		MeshCollider col = hit.collider.GetComponent<MeshCollider>();

		if (filter == null || col == null) return;

		if (!_storedMeshVisu.Contains(filter))
		{
			_storedMeshVisu.Add(filter);
			_storedCollider.Add(col);
		}

		_hitPoint.Add(hit.point);
	}

	private void DoTheCut()
	{
		if (_storedMeshVisu == null || _storedCollider == null || _hitPoint.Count < 2) return;

		for (int i = 0; i < _storedMeshVisu.Count; i++)
		{
			CutIndividualObject(_storedMeshVisu[i], _storedCollider[i]);
		}

		// Clear stored data
		_hitPoint.Clear();
		_storedCollider.Clear();
		_storedMeshVisu.Clear();
	}

	private void CutIndividualObject(MeshFilter filter, MeshCollider col)
	{
		Vector3 planeNormal = Vector3.Cross((_hitPoint[_hitPoint.Count - 1] - _hitPoint[0]).normalized, transform.forward).normalized;
		Plane cuttingPlane = new Plane(planeNormal, _hitPoint[0]);

		Mesh originalMesh = filter.mesh;
		List<Vector3> vertices = new List<Vector3>(originalMesh.vertices);
		List<int> triangles = new List<int>(originalMesh.triangles);

		// Store the new vertices for both parts
		List<Vector3> newVertices1 = new List<Vector3>();
		List<Vector3> newVertices2 = new List<Vector3>();

		// Iterate through the triangles and split edges that intersect the plane
		for (int i = 0; i < triangles.Count; i += 3)
		{
			Vector3 v1 = vertices[triangles[i]];
			Vector3 v2 = vertices[triangles[i + 1]];
			Vector3 v3 = vertices[triangles[i + 2]];

			// Determine on which side of the plane the vertices are
			bool side1 = cuttingPlane.GetSide(filter.transform.TransformPoint(v1));
			bool side2 = cuttingPlane.GetSide(filter.transform.TransformPoint(v2));
			bool side3 = cuttingPlane.GetSide(filter.transform.TransformPoint(v3));

			// Store vertices in the corresponding list
			if (side1 && side2 && side3)
			{
				newVertices1.Add(v1);
				newVertices1.Add(v2);
				newVertices1.Add(v3);
			}
			else if (!side1 && !side2 && !side3)
			{
				newVertices2.Add(v1);
				newVertices2.Add(v2);
				newVertices2.Add(v3);
			}
			else
			{
				// Split the triangle based on intersection
				//SplitTriangle(v1, v2, v3, side1, side2, side3, newVertices1, newVertices2, cuttingPlane);
			}
		}

		// Create new meshes for the two parts
		Mesh mesh1 = new Mesh();
		mesh1.vertices = newVertices1.ToArray();
		mesh1.triangles = CalculateTriangles(newVertices1);

		Mesh mesh2 = new Mesh();
		mesh2.vertices = newVertices2.ToArray();
		mesh2.triangles = CalculateTriangles(newVertices2);

		// Assign the first half to the original object
		filter.mesh = mesh1;
		col.sharedMesh = mesh1;

		// Create a new GameObject for the second half
		GameObject secondHalf = Instantiate(filter.gameObject, filter.transform.position, filter.transform.rotation);
		if (filter.transform.parent != null) secondHalf.transform.parent = filter.transform.parent.transform;
		secondHalf.transform.localScale = filter.transform.localScale;
		secondHalf.name = "second half";
		secondHalf.GetComponent<MeshFilter>().mesh = mesh2;
		secondHalf.GetComponent<MeshCollider>().sharedMesh = mesh2;
	}

	private void SplitTriangle(Vector3 v1, Vector3 v2, Vector3 v3, bool side1, bool side2, bool side3,
		List<Vector3> newVertices1, List<Vector3> newVertices2, Plane cuttingPlane)
	{
		Vector3[] vertices = { v1, v2, v3 };
		bool[] sides = { side1, side2, side3 };

		List<Vector3> positiveSide = new List<Vector3>();
		List<Vector3> negativeSide = new List<Vector3>();
		List<Vector3> intersectionPoints = new List<Vector3>();

		// Separate vertices by side and calculate intersection points
		for (int i = 0; i < 3; i++)
		{
			if (sides[i]) positiveSide.Add(vertices[i]);
			else negativeSide.Add(vertices[i]);

			int next = (i + 1) % 3;
			if (sides[i] != sides[next])
			{
				// Find intersection point
				Vector3 intersect;
				cuttingPlane.Raycast(new Ray(vertices[i], vertices[next] - vertices[i]), out float distance);
				intersect = vertices[i] + (vertices[next] - vertices[i]).normalized * distance;
				intersectionPoints.Add(intersect);
			}
		}

		// Create new triangles depending on intersection
		if (positiveSide.Count == 2)
		{
			newVertices1.Add(positiveSide[0]);
			newVertices1.Add(positiveSide[1]);
			newVertices1.Add(intersectionPoints[0]);
			newVertices1.Add(intersectionPoints[0]);
			newVertices1.Add(intersectionPoints[1]);
			newVertices1.Add(positiveSide[1]);

			newVertices2.Add(negativeSide[0]);
			newVertices2.Add(intersectionPoints[0]);
			newVertices2.Add(intersectionPoints[1]);
		}
		else if (negativeSide.Count == 2)
		{
			newVertices2.Add(negativeSide[0]);
			newVertices2.Add(negativeSide[1]);
			newVertices2.Add(intersectionPoints[0]);
			newVertices2.Add(intersectionPoints[0]);
			newVertices2.Add(intersectionPoints[1]);
			newVertices2.Add(negativeSide[1]);

			newVertices1.Add(positiveSide[0]);
			newVertices1.Add(intersectionPoints[0]);
			newVertices1.Add(intersectionPoints[1]);
		}
	}

	private int[] CalculateTriangles(List<Vector3> vertices)
	{
		List<int> triangles = new List<int>();
		for (int i = 0; i < vertices.Count; i += 3)
		{
			triangles.Add(i);
			triangles.Add(i + 1);
			triangles.Add(i + 2);
		}
		return triangles.ToArray();
	}
}