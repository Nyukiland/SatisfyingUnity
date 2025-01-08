using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSize : Activable
{
	private float _sizeToGo;
	private float _curSize;
	private float _prevSize;

	private void Start()
	{
		_curSize = _prevSize = transform.localScale.x;
	}

	void Update()
    {
		if (_curSize == _sizeToGo) return;

		float speed = 5f;
		float dif = (_sizeToGo - _curSize);
		_curSize += dif * Time.deltaTime * speed;

		_curSize = Mathf.Clamp(_curSize, Mathf.Min(_sizeToGo, _prevSize), Mathf.Max(_sizeToGo, _prevSize));
		transform.localScale = Vector3.one * _curSize;
    }

	public override void BaseInteraction(float value)
	{
		base.BaseInteraction(value);
		_prevSize = transform.localScale.x;
		_sizeToGo = value;
	}
}
