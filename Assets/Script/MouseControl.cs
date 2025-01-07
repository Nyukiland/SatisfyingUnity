using Unity.Collections;
using UnityEngine;

public delegate void LeftMouseClickPressed();
public delegate void LeftMouseClickReleased();
public delegate void RightMouseClickPressed();
public delegate void RightMouseClickReleased();

public class MouseControl : MonoBehaviour
{
	[ReadOnly]
	public Vector2 _mousePosition;

	public LeftMouseClickPressed _leftMouseClickPressed;
	public LeftMouseClickReleased _leftMouseClickReleased;
	public RightMouseClickPressed _rightMouseClickPressed;
	public RightMouseClickReleased _rightMouseClickReleased;

	private Camera _cam;
	private bool _isLeftClick;
	private bool _isRightClick;

	private void Start()
	{
		_cam = Camera.main;
	}

	void Update()
    {
        GetMousePos();
		ManageLeftClick();
		ManageRightClick();
    }

	private void GetMousePos()
	{
		_mousePosition = _cam.ScreenToWorldPoint(Input.mousePosition);
	}

	private void ManageLeftClick()
	{
		if (_isRightClick) return;

		if (Input.GetMouseButtonDown(0))
		{
			_isLeftClick = true;
			_leftMouseClickPressed?.Invoke();
		}
		else if (Input.GetMouseButtonUp(0))
		{
			_isLeftClick = false;
			_leftMouseClickReleased?.Invoke();
		}
	}

	private void ManageRightClick()
	{
		if (_isLeftClick) return;

		if (Input.GetMouseButtonDown(0))
		{
			_isRightClick = true;
			_rightMouseClickPressed?.Invoke();
		}
		else if (Input.GetMouseButtonUp(0))
		{
			_isRightClick = false;
			_rightMouseClickReleased?.Invoke();
		}
	}
}
