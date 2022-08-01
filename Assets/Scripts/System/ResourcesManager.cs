using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourcesManager : MonoBehaviour {

	private static ResourcesManager instance_;
	
	public static ResourcesManager instance
	{
		get
		{
			if(instance_ == null)
			{
				GameObject resourcesManagerObj = new GameObject("_ResourcesManager");
				Transform resourcesManagerTrans = resourcesManagerObj.GetComponent<Transform>();

				GameObject bundleManagerObj = GameObject.Find("_BundleManager");

				if(bundleManagerObj != null)
				{
					Transform bundleManagerTrans = bundleManagerObj.GetComponent<Transform>();

					resourcesManagerTrans.SetParent(bundleManagerTrans);
				}

				instance_ = resourcesManagerObj.AddComponent<ResourcesManager>();
			}

			return instance_;
		}
	}

	void OnDestoy()
	{
		instance_ = null;
	}

	public bool useAssetBundle = false;
//	private float globalframeTimeOut = 100F;

	void Awake()
	{
		instance_ = this;
	}

	public void PopEffect(GameObject fxObj, Vector3 pos)
	{
		PopEffect (fxObj.name, pos);
	}

	public void PopEffect(string fxName, Vector3 pos)
	{
		GameObject loadedObj = null;
		
		if (ObjectPooler.instance != null) 
		{
			loadedObj = ObjectPooler.instance.ObjPop (fxName, pos);
		}
		
		if(loadedObj == null)
		{
			loadedObj = LoadGameObject("fxs/" + fxName);
			
			if(loadedObj != null)
			{
				GameObject.Instantiate(loadedObj, pos, loadedObj.transform.rotation);
			}
		}
	}

	public void AttachEffect(string fxName, Transform parent)
	{
		GameObject loadedObj = null;
		GameObject attatchFx = null;
		
		if (ObjectPooler.instance != null) 
		{
			loadedObj = ObjectPooler.instance.ObjPop (fxName, parent.position);

			attatchFx =  loadedObj;
		}
		
		if(loadedObj == null)
		{
			loadedObj = LoadGameObject("fxs/" + fxName);
			
			if(loadedObj != null)
			{
				attatchFx = GameObject.Instantiate(loadedObj, parent.position, parent.rotation) as GameObject;
			}
		}

		attatchFx.transform.SetParent(parent);
	}

	public AudioClip LoadAudioClip(string path)
	{
		return (AudioClip)ResourcesLoadCached(path);
	}

	public GameObject LoadGameObject(string path)
	{
		string[] pathArray = path.Split ('/');
		
		GameObject obj = null;
		
		if (useAssetBundle) 
		{
			if(pathArray.Length > 0)
			{
				obj = (GameObject)BundleManager.instance.GetAssetFromLoadedBundle (path, pathArray [pathArray.Length-1]);
			} else {
				obj = (GameObject)BundleManager.instance.GetAssetFromLoadedBundle (path, pathArray [0]);
			}
		}
		
		if (obj == null) 
		{
			obj = (GameObject)ResourcesLoadCached(path);
		}

		return obj;
	}

	private Dictionary<string, Object> resourceCache = new Dictionary<string, Object>();
	public Object ResourcesLoadCached(string path)
	{
		Object resourceObj = null;
		if (!resourceCache.TryGetValue (path, out resourceObj)) 
		{
			resourceObj = Resources.Load<Object> (path);
			if (resourceObj != null) 
			{
				resourceCache.Add (path, resourceObj);
			}
		}

		return resourceObj;
	}

	public void ClearCache()
	{
		resourceCache.Clear ();
		Resources.UnloadUnusedAssets ();
	}
}
