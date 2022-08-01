using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ReplayGameplay))]
public class ReplayGameplayInspector : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		ReplayGameplay myScript = (ReplayGameplay)target;

		if (!myScript.IsReplay) {
			if (GUILayout.Button ("Replay Recordings")) {
				myScript.StartReplay ();
			}
		} else {
			if (GUILayout.Button ("Replaying...")) 
			{
			}
		}

	}
}
