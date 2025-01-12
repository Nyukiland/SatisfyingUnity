using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFollowCursor : MonoBehaviour
{
	private MouseControl _mouseControl;

	private void Start()
	{
		_mouseControl = MouseControl.Instance;
	}

	void Update()
    {
        transform.position = _mouseControl._mousePosition;
    }
}
