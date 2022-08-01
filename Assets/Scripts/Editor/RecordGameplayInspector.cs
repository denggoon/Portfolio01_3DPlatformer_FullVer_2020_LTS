using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RecordGameplay))]
public class RecordGameplayInspector : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		RecordGameplay myScript = (RecordGameplay)target;
		
		if(GUILayout.Button("Start Recording"))
		{
			myScript.StartRecording();
		}

		if(GUILayout.Button("Stop Recording"))
		{
			myScript.StopRecording();
		}

		if (myScript.IsPaused ()) 
		{
			if (GUILayout.Button ("Resume Recording")) {
				myScript.ResumeRecording();
			}
		} else {

			if (GUILayout.Button ("Pause Recording")) 
			{
				if(myScript.IsRecording())
					myScript.PauseRecording();
			}
		}
	}
}

