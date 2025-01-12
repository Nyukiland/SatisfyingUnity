using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LineToClosest : Activable
{
	[SerializeField]
	float _numberOfLine;

	[SerializeField]
	float _radius;

	[SerializeField]
	LineRenderer _lineRenderer;

	List<LineRenderer> _lines = new();

	[SerializeField]
	List<Activable> _activables = new();

	private void Start()
	{
		for (int i = 0; i < _numberOfLine; i++)
		{
			_lines.Add(Instantiate(_lineRenderer));
			_lines[i].transform.parent = transform;
		}

		List<Activable> temp = FindObjectsOfType<Activable>().ToList();
		List<GameObject> tempG = new();
		temp.ForEach(activable =>
		{
			if (!tempG.Contains(activable.gameObject))
			{
				_activables.Add(activable);
				tempG.Add(activable.gameObject);
			}
		});
	}

	private void Update()
	{
		_activables.Sort((a, b) =>
		{
			float distanceA = Vector3.Distance(a.transform.position, transform.position);
			float distanceB = Vector3.Distance(b.transform.position, transform.position);
			return distanceA.CompareTo(distanceB); // Tri croissant
		});

		for (int i = 0; i < _lines.Count; i++)
		{
			if (Vector3.Distance(_activables[i].transform.position, transform.position) > _radius) continue;
			Vector3 pos = new Vector3(transform.position.x, transform.position.y, 1);
			_lines[i].SetPosition(0, pos);
			pos = new Vector3(_activables[i].transform.position.x, _activables[i].transform.position.y, 1);
			_lines[i].SetPosition(1, pos);
		}
	}
}