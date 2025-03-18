using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SeeThroughWindow : MonoBehaviour
{
	/*
	PlayerSettings
		- Disable "Use DXGI Flip Model SwapChain for D3D11"
		- Fullscreen window
		- Enable run in backgrounds

	Graphics
		- BuiltInRender pipeline

	Camera
		- back ground color: Black no alpha
	 */

	[DllImport("user32.dll")]
	public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

	[DllImport("user32.dll")]
	private static extern IntPtr GetActiveWindow();

	[DllImport("user32.dll")]
	private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

	[DllImport("user32.dll")]
	private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, uint Y, int cx, int cy, uint uFlags);

	[DllImport("user32.dll")]
	private static extern int SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

	private struct MARGINS
	{
		public int leftWidth;
		public int rightWidth;
		public int topHeight;
		public int bottomHeight;
	}

	[DllImport("Dwmapi.dll")]
	private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

	const int GWL_EXSTYLE = -20;

	const uint WS_EX_LAYERED = 0x00080000;
	const uint WS_EX_TRANSPARENT = 0x00000020;

	static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

	const uint LWA_COLORKEY = 0x00000001;

	private IntPtr hwnd;

	private List<RectTransform> objectOnScreen = new();

	private void Start()
	{
#if UNITY_EDITOR
		
		UnityEditor.EditorApplication.ExitPlaymode();
		return;

#endif

		MessageBox(new IntPtr(0), "Has started", "DebugBox", 0);

		hwnd = GetActiveWindow();

		MARGINS margins = new MARGINS { leftWidth = -1 };
		DwmExtendFrameIntoClientArea(hwnd, ref margins);

		SetWindowLong(hwnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT); //remove transparent if no check
		//SetLayeredWindowAttributes(hwnd, 0, 0, LWA_COLORKEY); //use if no check

		SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, 0);

		Application.runInBackground = true;
	}

	public void RegisterElement(RectTransform transform)
	{
		if (!objectOnScreen.Contains(transform)) objectOnScreen.Add(transform);
	}

	private void Update()
	{
		if (IsHovering())
		{
			SetWindowLong(hwnd, GWL_EXSTYLE, WS_EX_LAYERED);
		}
		else
		{
			SetWindowLong(hwnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
		}
	}

	private bool IsHovering()
	{
		foreach (RectTransform transform in objectOnScreen)
		{
			Vector2 localMousePosition = transform.InverseTransformPoint(Input.mousePosition);
			if (transform.rect.Contains(localMousePosition))
			{
				return true;
			}
		}

		return false;
	}
}