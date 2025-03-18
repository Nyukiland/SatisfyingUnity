using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class STChangeColorClick : SeeThroughElement
{
	private Image _image;

	private void Start()
	{
		_image = GetComponent<Image>();
	}

	public void SetColor(string color)
	{
		switch (color)
		{
			case "Red":
				_image.color = Color.red;
				break;
			case "Green":
				_image.color = Color.green;
				break;
			case "Blue":
				_image.color = Color.blue;
				break;
			case "White":
				_image.color = Color.white;
				break;
			case "Black":
				_image.color = Color.black;
				break;
			case "Yellow":
				_image.color = Color.yellow;
				break;
			case "Cyan":
				_image.color = Color.cyan;
				break;
			case "Magenta":
				_image.color = Color.magenta;
				break;
			case "Grey":
				_image.color = Color.grey;
				break;
		}
	}
}