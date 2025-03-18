using UnityEngine;
using UnityEngine.UI;

public class STChangeColorClick : SeeThroughElement
{
	public void SetRandomColorOnClick()
	{
		ColorBlock color = ColorBlock.defaultColorBlock;
		color.normalColor = new Color(Random.Range(0, 1), Random.Range(0, 1), Random.Range(0, 1), 1);

		GetComponent<Button>().colors = color;
	}
}
