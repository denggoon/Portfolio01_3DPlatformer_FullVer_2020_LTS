using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LightMapScript))]
public class LightMapScriptInspector : Editor {

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		LightMapScript myScript = (LightMapScript)target;

		if(GUILayout.Button("Attach Tiling Script"))
		{
			myScript.AttachTilingScriptToAllRenderers();
		}

		if(GUILayout.Button("Fill Lightmap Data"))
		{
			myScript.FillLightMapData();
		}

		if(GUILayout.Button("Remove Tiling Script"))
		{
			myScript.RemoveTilingScriptToAllRenderers();
		}
	}
}
