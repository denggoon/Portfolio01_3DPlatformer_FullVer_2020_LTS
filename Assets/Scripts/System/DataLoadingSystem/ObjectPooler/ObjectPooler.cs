using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour { 
	private static ObjectPooler instance_; 

	public static ObjectPooler instance { 
		get { 
			return instance_; 
		} 
	} 

	void OnDestroy()
	{
		instance_ = null;
	}
	
	[System.Serializable]
	public class ObjPoolInfo
	{
		public string objName;
		public int poolAmount;

		public ObjPoolInfo(string name, int amount)
		{
			objName = name;
			poolAmount = amount;
		}
	}

	[System.Serializable]
	public class ObjStkInfo
	{
		public string stackName;
		public GameObject stackParent;
		public Stack<GameObject> stack = new Stack<GameObject>();

		public ObjStkInfo(GameObject stackParent, Stack<GameObject> stack)
		{
			this.stackParent = stackParent;
			this.stack = stack;
		}

		public GameObject GetStackParent()
		{
			return this.stackParent;
		}

		public Stack<GameObject> GetStack()
		{
			return this.stack;
		}

	}

	public new Transform transform;
	public List<ObjPoolInfo> lstObjInfo = new List<ObjPoolInfo>();
	public int presetCount = 10;
	public string[] poolKeywords; //풀링 되어야할 번들 키워드 

	public bool IsPoolerInitialized = false;

	void Awake()
	{
		instance_ = this;
		transform = GetComponent<Transform> ();
	}
	
	public IEnumerator Initialze()
	{
		AssetBundleManifest manifest = BundleManager.instance.manifest;

		if (manifest != null) {
			for (int j=0; j<poolKeywords.Length; j++) {
				ImportManifest (manifest, poolKeywords [j]);
			}

			for (int i=0; i< lstObjInfo.Count; i++) 
			{
				yield return StartCoroutine(BundleManager.instance.LoadBundleCoroutine(lstObjInfo[i].objName));
				PrepareObjStk (lstObjInfo [i].objName, lstObjInfo [i].poolAmount);
			}

		} else {
			Debug.LogError("Initialize: BundleManager's manifest not loaded");
			yield break;
		}

		IsPoolerInitialized = true;

	}

	public void ImportManifest(AssetBundleManifest manifest, string category)
	{
		bool categoryExists = false;

		for(int j=0; j<lstObjInfo.Count; j++)
		{
			if(lstObjInfo[j].objName.Contains(category))
			{
				categoryExists = true;
				break;
			}
		}

		if (categoryExists) 
		{
			Debug.LogWarning("ImportManifest: category [" + category + "] already exists in objInfo.");

		} else {

			string[] bundleNames = manifest.GetAllAssetBundles ();

			for (int i=0; i<bundleNames.Length; i++) 
			{
				Debug.LogWarning("ImportManifest: " + bundleNames[i]);
				if (bundleNames[i].Contains (category)) 
				{
					ObjPoolInfo poolInfo = new ObjPoolInfo (bundleNames[i], presetCount);
					lstObjInfo.Add (poolInfo);
				}
			}
		}
	}
	
	private Dictionary<string, ObjStkInfo> dicStkInfo = new Dictionary<string, ObjStkInfo>(); 
	public void PrepareObjStk(string name, int count) 
	{ 
		Debug.LogError ("PrepreObjStk");
		GameObject poolParentObj = new GameObject("Pool_"+name); 
		Stack<GameObject> stkPool = new Stack<GameObject>(); 

		Transform poolParentTrans = poolParentObj.GetComponent<Transform> ();

		poolParentTrans.SetParent (this.transform);
		
		for(int i=0; i<count; i++) 
		{ 
			GameObject loadedObj = ResourcesManager.instance.LoadGameObject(name);
			if(loadedObj != null )
			{
				GameObject poolObj = GameObject.Instantiate(loadedObj, Vector3.zero, loadedObj.transform.rotation) as GameObject;
				Transform poolObjTrans = poolObj.GetComponent<Transform>();

				poolObj.name = name;
				poolObj.SetActive(false);
				poolObjTrans.SetParent(poolParentTrans);
				PushObjInStk(poolObj, stkPool); 
			}
		} 

		ObjStkInfo stkInfo = new ObjStkInfo (poolParentObj, stkPool);
		AddStkInfoDic(name, stkInfo); 

		BundleManager.instance.Unload (name);
	} 

	public bool AddStkInfoDic(string name , ObjStkInfo stkInfo) 
	{ 
		if(!dicStkInfo.ContainsKey(name)) 
		{ 
			dicStkInfo.Add(name, stkInfo); 
			return true; 
		} 
		return false;
	}

	public GameObject SearchStkObjInDic(string name)
	{
		GameObject stkObj = null;

		if (dicStkInfo.ContainsKey (name)) 
		{
			stkObj = dicStkInfo[name].GetStackParent();
		}

		return stkObj;
	}
	
	public GameObject ObjPop(string name, Vector3 popPos, bool autoActive = true) 
	{ 
		if (IsPoolerInitialized == false) 
		{
			GameObject loadedObj = ResourcesManager.instance.LoadGameObject(name);

			return loadedObj;
		}

		GameObject objToPop  = null; 
		if(dicStkInfo.ContainsKey(name)) 
		{ 
			if(dicStkInfo[name].GetStack().Count <= 0) 
			{ 
				GameObject stkObj = SearchStkObjInDic(name);
				Transform stkObjTrans = stkObj.GetComponent<Transform>();

				if(stkObj != null )
				{
					GameObject loadedObj = ResourcesManager.instance.LoadGameObject(name);

					if(loadedObj != null)
					{
						objToPop = GameObject.Instantiate(loadedObj, popPos, loadedObj.transform.rotation) as GameObject;
						if(objToPop != null) 
						{ 
							objToPop.name = name; 
							Transform objToPopTrans = objToPop.GetComponent<Transform>();

							objToPopTrans.SetParent(stkObjTrans);

							if(autoActive)
								objToPop.SetActive(true);

							return objToPop; 
						}
					}
				}
				
				return null;

			} else {

				objToPop = dicStkInfo[name].GetStack().Pop() as GameObject; 

				Transform objToPopTrans = objToPop.GetComponent<Transform>();
				objToPopTrans.position = popPos;

				if(autoActive)
					objToPop.SetActive(true);

				return objToPop; 
			}
			
		} else {

//			Debug.Log("ObjPop: Obj [" + name + "] does not exist in dictionary, try with lower case: " + name.ToLower());
			if(ObjPop(name.ToLower(), popPos) == null) //try with lower case 
			{
				Debug.LogError("ObjPop: Obj [" + name + "] does not exist in dictionary, loads from resourcesManager");

				GameObject loadedObj = ResourcesManager.instance.LoadGameObject(name);
				
				if(loadedObj != null)
				{
					objToPop = GameObject.Instantiate(loadedObj, popPos, loadedObj.transform.rotation) as GameObject;

					if(objToPop != null) 
					{ 
						objToPop.name = name; 
						
						if(autoActive)
							objToPop.SetActive(true);
						
						return objToPop; 
					}
				}
			}
		}

		return null;
	}
	
	public bool ObjPush(string name, GameObject go) 
	{ 
		if (IsPoolerInitialized == false)
			return false;

		if (dicStkInfo.ContainsKey (name)) {
			go.SetActive (false);

			PushObjInStk (go, dicStkInfo [name].GetStack ());
			return true; 

		} else {
//			Debug.LogError("ObjPush: Cannot find stack for [" + name + "]. Destroying object instead.");

			Destroy (go);
		}

		return false;
	}
	
	public void PushObjInStk(GameObject go, Stack<GameObject> stack) 
	{
		if( stack != null )
		{
			stack.Push(go);
		}
		else 
			Debug.Log("PushObjInStk: Stack to push obj is null!!"); 
	}
}
