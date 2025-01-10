using UnityEngine;

public class ChangeSizeSurrounding : ReceiveInput
{
	[SerializeField]
	float _radius;

	[SerializeField]
	bool _invertScale;

	Activable[] _activables;

	private void Start()
	{
		_activables = FindObjectsOfType<Activable>();
	}

	void Update()
    {
        foreach (Activable a in _activables)
		{
			float dist = Vector3.Distance(a.transform.position, transform.position);
			dist = Mathf.Clamp(dist, 0, _radius);

			if (dist > _radius) continue;

			float value = Mathf.InverseLerp(0, _radius, dist);

			if (_invertScale) value = Mathf.Abs(value - 1);

			a.BaseInteraction(value);
		}
    }
}
