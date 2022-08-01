using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif
using System.Collections.Generic;

[ExecuteInEditMode]
public class PrefabInstance : MonoBehaviour
{
	public GameObject parentPrefab;
	public GameObject fxPrefab;
	private new Transform transform;
	private bool isMake = false;
	private const string fxPrefabName = "FXPrefab";
	
#if UNITY_EDITOR 
	void OnValidate () {
		if (enabled) {
			transform = GetComponent<Transform>();
			Rebuild (fxPrefab);
		}
	}
	
	void OnEnable () {
	}
	
	void OnDisable() {
	}
	
	void Rebuild (GameObject source) {
		if (!source)
			return;
		
		bool isEmpty = true;
		foreach (Transform child in transform) {
			if (child.name == fxPrefabName) {
				isEmpty = false;
				break;
			}
		}
		
		if (isEmpty == true) {
			isMake = true;
			makePrefab(source);
		} else {
			
			GameObject tmpObject = GameObject.Find(fxPrefabName) as GameObject;
			Destroy(tmpObject);
			//			PrefabUtility.ReplacePrefab(parentPrefab, PrefabUtility.GetPrefabParent(parentPrefab), ReplacePrefabOptions.ConnectToPrefab);
		}
	}	
	
	void makePrefab(GameObject source){
		
		GameObject prefabObject = MonoBehaviour.Instantiate(source) as GameObject;
		
		prefabObject.name = fxPrefabName;
		prefabObject.transform.position = transform.position;
		prefabObject.transform.rotation = transform.rotation;
		prefabObject.transform.parent = transform;

		//PrefabUtility.ReplacePrefab(parentPrefab, PrefabUtility.GetPrefabParent(parentPrefab), ReplacePrefabOptions.ConnectToPrefab);
		PrefabUtility.SaveAsPrefabAssetAndConnect(parentPrefab, AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(parentPrefab)), InteractionMode.AutomatedAction);
	}

#endif
	
}