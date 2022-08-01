using UnityEngine;
using System.Collections;

public class InfiniteTrigger : MonoBehaviour {

	public InfiniteMaps rootObj;
	private BoxCollider boxCollider = null;
	// Use this for initialization
	void Start () {
		boxCollider = GetComponent<BoxCollider>();
		rootObj = GameObject.Find("InfiniteMap").GetComponent<InfiniteMaps>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public virtual void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.layer == LayerMask.NameToLayer ("Player")) { //플레이어인 경우

			boxCollider.enabled = false;

//			GameObject instance = MonoBehaviour.Instantiate (GameRuleManager.instance.infinitePrefab) as GameObject;
			GameObject instance = rootObj.getPooledObject();
			instance.SetActive(false);

//			GameObject rootObj = GameObject.Find("InfiniteMap");
			instance.transform.parent = rootObj.transform;
			instance.name = GameRuleManager.instance.infinitePrefab.name + "_" + GameRuleManager.instance.infiniteBlockCount;
			instance.transform.position = new Vector3(GameRuleManager.instance.infiniteBlockCount * 250, 0, 0);
			instance.SetActive(true);

			Transform oldTransForms = rootObj.transform.Find(GameRuleManager.instance.infinitePrefab.name + "_" + (GameRuleManager.instance.infiniteBlockCount - 2));
			if (oldTransForms != null) {
//				Destroy(oldTransForms.gameObject);
				oldTransForms.gameObject.SetActive(false);
			}

			GameRuleManager.instance.infiniteBlockCount++;
		}
	}
}
