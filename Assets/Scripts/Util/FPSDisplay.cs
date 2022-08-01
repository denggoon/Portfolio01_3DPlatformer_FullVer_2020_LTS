using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour {

	float deltaTime = 0.0F;

	void Update () 
	{
		deltaTime += (Time.deltaTime - deltaTime) * .1F;
	}

	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;
		
		GUIStyle style = new GUIStyle();
		
		Rect rect = new Rect(0, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = Color.red;
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		GUI.Label(rect, text + " Resolution: " + Screen.width + " X " + Screen.height + " (" + Screen.dpi + " dpi - " + Screen.width / Screen.dpi + " Inch Wide)", style);
	}
}
