using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSize : MonoBehaviour
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

		float speed = 0.5f;
		float dif = (_sizeToGo - _curSize);
		_curSize += dif * Time.deltaTime * speed;

		_curSize = Mathf.Clamp(_curSize, Mathf.Min(_sizeToGo, _prevSize), Mathf.Max(_sizeToGo, _prevSize));
		transform.localScale = Vector3.one * _curSize;
    }

	public void CallChangeSize(float newSize)
	{
		_prevSize = newSize;
		_sizeToGo = newSize;
	}
}
