using UnityEngine;

public class ActivateSurounding : ReceiveInput
{
	[SerializeField]
	float _radius;

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
			
			a.BaseInteraction(value);
		}
    }
}
