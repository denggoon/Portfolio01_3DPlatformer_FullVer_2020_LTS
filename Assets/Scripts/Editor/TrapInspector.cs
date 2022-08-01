using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Trap))]
[CanEditMultipleObjects]
public class TrapInspector : Editor {

	bool triggeredFlag = false; //인스펙터 확인용

	public override void OnInspectorGUI()
	{
		if(Application.isPlaying)
		{
			Trap trap = target as Trap;
			GUILayout.BeginHorizontal ();
			if(trap.isTriggered)
			{
				if(GUILayout.Button("Switch OFF Trigger"))
				{
					trap.SwitchTrigger(false);
				}
			} else {
				if(GUILayout.Button("Switch ON Trigger"))
				{
					trap.SwitchTrigger(true);
				}
			}
			GUILayout.EndHorizontal ();
		}

		DrawDefaultInspector ();
	}
}

[CustomEditor(typeof(MovingPlatform))]
[CanEditMultipleObjects]
public class MovingPlatformInspector: TrapInspector {}

[CustomEditor(typeof(FloatingMachine))]
[CanEditMultipleObjects]
public class FloatingMachineInspector: TrapInspector {}

[CustomEditor(typeof(AttractZone))]
[CanEditMultipleObjects]
public class AttractZoneInspector: TrapInspector {}

