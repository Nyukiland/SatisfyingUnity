using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveInput : MonoBehaviour
{
	protected bool _leftPressed, _rightPressed;

    void OnAwake()
    {
		MouseControl.Instance._leftMouseClickPressed += LeftPressed;
		MouseControl.Instance._leftMouseClickPressed += LeftReleased;

		MouseControl.Instance._leftMouseClickPressed += RightPressed;
		MouseControl.Instance._leftMouseClickPressed += RightReleased;
	}

	void OnDestroy()
	{
		MouseControl.Instance._leftMouseClickPressed -= LeftPressed;
		MouseControl.Instance._leftMouseClickPressed -= LeftReleased;

		MouseControl.Instance._leftMouseClickPressed -= RightPressed;
		MouseControl.Instance._leftMouseClickPressed -= RightReleased;
	}

	protected virtual void LeftPressed() { _leftPressed = true; }
	protected virtual void LeftReleased() { _leftPressed = false; }
	protected virtual void RightPressed() { _rightPressed = true; }
	protected virtual void RightReleased() { _rightPressed = false; }
}
