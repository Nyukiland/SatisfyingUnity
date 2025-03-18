using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SeeThroughElement : MonoBehaviour
{
	protected RectTransform _rectTransform;

	private void Awake()
	{
		FindObjectOfType<SeeThroughWindow>().RegisterElement(GetComponent<RectTransform>());
		_rectTransform = GetComponent<RectTransform>();
	}
}
