using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InfiniteMaps : MonoBehaviour {

	private int currentIndex = 0;
	private int pooledAmount = 10;
	List<GameObject> pooledObjects;

	// Use this for initialization
	void Start () {
	
		pooledObjects = new List<GameObject> ();
		for (int i = 0; i < pooledAmount; i++) {

			GameObject instance = MonoBehaviour.Instantiate (GameRuleManager.instance.infinitePrefab) as GameObject;
			instance.SetActive(false);
			pooledObjects.Add(instance);
		}
	}

	// Update is called once per frame
	void Update () {
		
	}

	public GameObject getPooledObject() {

		GameObject instance = pooledObjects[currentIndex];
		currentIndex++;

		return instance;
	}
	

}
