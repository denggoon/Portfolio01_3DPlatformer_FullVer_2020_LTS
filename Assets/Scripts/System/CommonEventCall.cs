using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(CommonEventCall))]
public class CommonEventCallEditor : Editor
{
	string[] _choices = new [] {"None", "Character", "Monster" , "Gimmick", "Item"};

	public override void OnInspectorGUI()
	{
		CommonEventCall eventCall = (CommonEventCall)target;

		eventCall.choiceIdx = EditorGUILayout.Popup("type", eventCall.choiceIdx, _choices);

		switch (eventCall.choiceIdx) {
			case 0:
				eventCall.spwanFxName = EditorGUILayout.TextField("spwanFxName", eventCall.spwanFxName);
				eventCall.deSpwanFxName = EditorGUILayout.TextField("deSpwanFxName", eventCall.deSpwanFxName);
				eventCall.damageFxName = EditorGUILayout.TextField("damageFxName", eventCall.damageFxName);
				eventCall.collisionFxName = EditorGUILayout.TextField("collisionFxName", eventCall.collisionFxName);
			break;

			case 1:
				eventCall.spwanFxName = EditorGUILayout.TextField("spwanFxName", eventCall.spwanFxName);
				eventCall.deSpwanFxName = EditorGUILayout.TextField("deSpwanFxName", eventCall.deSpwanFxName);
				eventCall.damageFxName = EditorGUILayout.TextField("damageFxName", eventCall.damageFxName);
			break;

			case 2:
				eventCall.spwanFxName = EditorGUILayout.TextField("spwanFxName", eventCall.spwanFxName);
				eventCall.deSpwanFxName = EditorGUILayout.TextField("deSpwanFxName", eventCall.deSpwanFxName);
			break;

			case 3:
				eventCall.spwanFxName = EditorGUILayout.TextField("spwanFxName", eventCall.spwanFxName);
				eventCall.deSpwanFxName = EditorGUILayout.TextField("deSpwanFxName", eventCall.deSpwanFxName);
			break;

			case 4:
				eventCall.spwanFxName = EditorGUILayout.TextField("spwanFxName", eventCall.spwanFxName);
				eventCall.deSpwanFxName = EditorGUILayout.TextField("deSpwanFxName", eventCall.deSpwanFxName);
				eventCall.collisionFxName = EditorGUILayout.TextField("collisionFxName", eventCall.collisionFxName);
			break;

			default:
			break;
		}

	}
}
#endif


public class CommonEventCall : MonoBehaviour {

	public int choiceIdx = 0;
	public string spwanFxName;
	public string deSpwanFxName;
	public string damageFxName;
	public string collisionFxName;

	void OnEnable()
	{
		ResourcesManager.instance.PopEffect (spwanFxName, this.transform.position);
	}

	void OnDisable()
	{
		if(ResourcesManager.instance != null)
			ResourcesManager.instance.PopEffect (deSpwanFxName, this.transform.position);
	}

	public void OnDamageEvent()
	{
		ResourcesManager.instance.PopEffect (damageFxName, this.transform.position);
	}

	public void OnCollisionEvent()
	{
		ResourcesManager.instance.PopEffect (collisionFxName, this.transform.position);
	}

}
