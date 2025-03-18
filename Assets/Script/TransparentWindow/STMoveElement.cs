using UnityEngine;

public class STMoveElement : SeeThroughElement
{
	private bool _inHold;

	private Canvas _parentCanvas;

	private void Start()
	{
		_parentCanvas = GetComponentInParent<Canvas>();
	}

	public void InHold(bool isHold)
	{
		_inHold = isHold;
	}

	private void Update()
	{
		if (_inHold)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentCanvas.transform as RectTransform, Input.mousePosition, _parentCanvas.worldCamera, out Vector2 mousePosition);

			_rectTransform.position = _parentCanvas.transform.TransformPoint(mousePosition);
		}
	}
}
