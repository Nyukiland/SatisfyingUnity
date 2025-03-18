using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SeeThroughElement : MonoBehaviour
{
	private void Awake()
	{
		FindObjectOfType<SeeThroughWindow>().RegisterElement(GetComponent<RectTransform>());
	}
}
