using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloattingCircle : MonoBehaviour
{
	private Vector2 _basePos;
	private Vector2 _currentTarget;    // Current target position
	private Vector2 _velocity;        // Tracks the velocity of the object

	[SerializeField] private float radius = 1.0f;         // Radius of the circle
	[SerializeField] private float targetChangeInterval = 2.0f; // Time interval to change target
	[SerializeField] private float speed = 3.0f;         // Speed of the motion
	[SerializeField] private float smoothing = 0.5f;     // Blend factor for smoothing motion

	private float _nextTargetTime;    // Timer for the next target change

	void Start()
	{
		_basePos = transform.position;
		SetNewTarget();
		_nextTargetTime = Time.time + targetChangeInterval;
	}

	void Update()
	{
		// Check if it's time to select a new target
		if (Time.time >= _nextTargetTime)
		{
			SetNewTarget();
			_nextTargetTime = Time.time + targetChangeInterval;
		}

		// Smoothly interpolate towards the target while keeping part of the previous velocity
		Vector2 desiredPosition = Vector2.Lerp(transform.position, _currentTarget, smoothing);
		_velocity = desiredPosition - (Vector2)transform.position;

		// Apply the velocity to update the position
		transform.position = (Vector2)transform.position + _velocity * speed * Time.deltaTime;
	}

	// Set a new random target position within the circle
	private void SetNewTarget()
	{
		float angle = Random.Range(0, Mathf.PI * 2); // Random angle
		Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
		_currentTarget = _basePos + offset;
	}
}