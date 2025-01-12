using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloattingCircle : Activable
{
	private Vector2 _velocity;    
	private Vector2 _currentForce;

	[SerializeField] 
	Vector2 _center = Vector2.zero;
	[SerializeField] 
	float _radius = 1.0f;          
	[SerializeField] 
	Vector2 _boxSize = new Vector2(2.0f, 2.0f);
	[SerializeField] 
	float _forceMagnitude = 1.0f;  
	[SerializeField] 
	float _maxSpeed = 5;        
	[SerializeField] 
	float _forceInterval = 0.2f;   
	[SerializeField] 
	bool _constrainToCircle = true;

	private float _nextForceTime;

	void Start()
	{
		_velocity = Vector2.zero;
		_nextForceTime = Time.time + _forceInterval;
	}

	void Update()
	{
		if (Time.time >= _nextForceTime)
		{
			ApplyRandomForce();
			_nextForceTime = Time.time + _forceInterval;
		}

		if (_velocity.magnitude > _maxSpeed)
		{
			_velocity = _velocity.normalized * _maxSpeed;
		}

		transform.position = (Vector2)transform.position + _velocity * Time.deltaTime;

		if (_constrainToCircle)
			ConstrainToCircle();
		else
			ConstrainToBox();
	}

	private void ApplyRandomForce()
	{
		float angle = Random.Range(0, Mathf.PI * 2);
		Vector2 randomDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

		_currentForce = randomDirection * _forceMagnitude;
		_velocity += _currentForce;
	}

	private void ConstrainToCircle()
	{
		Vector2 currentPosition = transform.position;
		Vector2 offsetFromCenter = currentPosition - _center;

		if (offsetFromCenter.magnitude > _radius)
		{
			offsetFromCenter = offsetFromCenter.normalized * _radius;
			transform.position = _center + offsetFromCenter;

			_velocity = Vector2.Reflect(_velocity, offsetFromCenter.normalized);
		}
	}

	private void ConstrainToBox()
	{
		Vector2 currentPosition = transform.position;
		Vector2 offsetFromCenter = currentPosition - _center;

		float halfWidth = _boxSize.x / 2;
		float halfHeight = _boxSize.y / 2;

		if (Mathf.Abs(offsetFromCenter.x) > halfWidth)
		{
			offsetFromCenter.x = Mathf.Sign(offsetFromCenter.x) * halfWidth;
			_velocity.x = -_velocity.x; 
		}

		if (Mathf.Abs(offsetFromCenter.y) > halfHeight)
		{
			offsetFromCenter.y = Mathf.Sign(offsetFromCenter.y) * halfHeight;
			_velocity.y = -_velocity.y; 
		}

		transform.position = _center + offsetFromCenter;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;

		if (_constrainToCircle)
		{
			Gizmos.DrawWireSphere(_center, _radius);
		}
		else
		{
			Vector3 boxCenter = _center;
			Vector3 boxSizeGizmo = new Vector3(_boxSize.x, _boxSize.y, 0);
			Gizmos.DrawWireCube(boxCenter, boxSizeGizmo);
		}
	}
}