using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

public enum BUNDLE_LOAD_MODE
{
	EDITOR_LOCAL,
	PERSISTENT,
	ONLINE,
	STREAMING
}

public class AssetBundleRef
{
	public AssetBundle bundle;
	public string bundleName;

	public AssetBundleRef(AssetBundle bundle, string bundleName)
	{
		this.bundle = bundle;
		this.bundleName = bundleName;
	}
}

public class AssetBundleVars
{
	public string bundleName;
	public string variantName;

	public AssetBundleVars(string bundleName, string variantName)
	{
		this.bundleName = bundleName;
		this.variantName = variantName;
	}
}

public class BundleManager : MonoBehaviour 
{
	private static BundleManager instance_;
	
	public static BundleManager instance
	{
		get
		{
			return instance_;
		}
	}

	public BUNDLE_LOAD_MODE eBundleMode = BUNDLE_LOAD_MODE.EDITOR_LOCAL;

	string webPath;
	string persistentPath;
	string streamingPath;
	string localPath;

	public string pathToBundles;
	public float currentBundleProgress;

	public List<AssetBundleRef> bundleRefs;
	public List<AssetBundleVars> variantRefs;

	public AssetBundleManifest manifest = null;
	public string platform;

	public string statusMsg;

	public string currentSceneName;

	public bool isReady 
	{
		get { return !object.ReferenceEquals(manifest, null);}
	}

	void Awake()
	{
		if (object.ReferenceEquals (instance, null)) 
		{
			instance_ = this;
		}
		else if (!object.ReferenceEquals (instance, this))
		{
			Destroy (gameObject);
			return;
		}

		instance_ = this;
		
		DontDestroyOnLoad (gameObject);

		webPath = "http://192.168.1.56/bundles/assetbundles/";

		persistentPath = Application.persistentDataPath + "/bundles/";
		streamingPath = Application.streamingAssetsPath + "/bundles/";

		if (Application.isEditor) 
		{
			persistentPath = "file:///" + persistentPath;
			streamingPath = "file:///" + streamingPath;
		}

		localPath = "file:///" + Application.dataPath;
		localPath = localPath.Replace("Assets", "AssetBundles/");
		localPath = localPath.Replace('\\', '/');


		#if UNITY_IOS
				platform = "iOS";
				eBundleMode = BUNDLE_LOAD_MODE.STREAMING;

//				switch (DataFileManager.instance.eFileLoadMode) 
//				{
//					case FILE_LOAD_MODE.ONLINE:
//						break;
//						
//					default:
//						eBundleMode = BUNDLE_LOAD_MODE.STREAMING;
//						break;
//				}

		#elif UNITY_ANDROID && !UNITY_EDITOR
				platform = "Android";

				switch (DataFileManager.instance.eFileLoadMode) 
				{
					case FILE_LOAD_MODE.ONLINE:
						break;
					
					default:
						eBundleMode = BUNDLE_LOAD_MODE.STREAMING;
						break;
				}

		#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
				platform = "PC";
				
				//에디터 테스트인 경우 온라인이나 스트리밍 에셋 모드가 아니라면 bundles 폴더를 생성합니다. 
				if(eBundleMode == BUNDLE_LOAD_MODE.PERSISTENT)
				{
					if (!Directory.Exists (persistentPath))
					{
						Directory.CreateDirectory (persistentPath);
					}
				} else 
				if(eBundleMode == BUNDLE_LOAD_MODE.EDITOR_LOCAL)
				{
					string localDirectory = localPath.Replace("file:///", string.Empty);
					if (!Directory.Exists (localDirectory))
					{
						Directory.CreateDirectory (localDirectory);
					}
				}

		#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
				platform = "OSX";
		#elif UNITY_WEBPLAYER
				platform = "Web";
		#elif UNITY_WP8
				platform = "WP8";
		#else
				platform = "error";
				Debug.Log("unsupported platform");
		#endif

		switch (eBundleMode) 
		{
			case BUNDLE_LOAD_MODE.EDITOR_LOCAL:
				pathToBundles = localPath;
				break;
				
			case BUNDLE_LOAD_MODE.PERSISTENT:
				pathToBundles = persistentPath;
				break;
				
			case BUNDLE_LOAD_MODE.ONLINE:
				pathToBundles = webPath;
				break;
				
			case BUNDLE_LOAD_MODE.STREAMING:
				if(Application.platform == RuntimePlatform.IPhonePlayer) {
					pathToBundles = "file:///" + streamingPath;
				} else {
					pathToBundles = streamingPath;
				}
				break;
			
		}

//		pathToBundles += platform + "/";
		pathToBundles = System.IO.Path.Combine(pathToBundles, platform);

		bundleRefs = new List<AssetBundleRef> ();
		variantRefs = new List<AssetBundleVars> ();

//		bundles = new Dictionary<string, AssetBundle> ();
//		bundleVariants = new Dictionary<string, string> ();
//		StartCoroutine (LoadManifest(platform));
	}

	public bool IsManifestLoaded = false;
	public IEnumerator LoadManifest (string platform) 
	{
		Debug.Log( "Loading Manifest");
		IsManifestLoaded = false;

		statusMsg = "Manifest 파일 로딩중..."; 
//		using (WWW www = new WWW(pathToBundles + platform)) 
		using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(System.IO.Path.Combine(pathToBundles, platform), 0)) 
		{

			yield return www.SendWebRequest();

			if (!string.IsNullOrEmpty (www.error)) {
				Debug.Log (www.error);
				yield break;
			}

			AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);

			manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

			yield return null;

			if (!isReady)
			{
				Debug.Log ("There was an error loading manifest");
				statusMsg = "Manifest 파일을 로드하는데에 문제가 있었습니다."; 
			}
			else {
				Debug.Log ("Manifest loaded successfully");
			}

			www.Dispose ();
		}

		IsManifestLoaded = true;
	}

	public bool IsBundleLoaded(string bundleName)
	{
		bool flag = false;
		for (int i=0; i<bundleRefs.Count; i++) 
		{
//			Debug.LogWarning(bundleRefs[i].bundleName + " / "  + bundleName);
			if(bundleRefs[i].bundleName == bundleName)
			{
				flag = true;
				break;
			}
		}

//		Debug.LogWarning ("Was " + bundleName + " already loaded? : " + flag);
		return flag;

//		return bundles.ContainsKey (bundleName);
	}

	public void RegisterVariant(string bundleName, string variantName)
	{
		for (int i=0; i<variantRefs.Count; i++) 
		{
			if(variantRefs[i].bundleName == bundleName)
			{
				Debug.Log(string.Format("Variant for {0} cannot be added. {1} already registered. " +
				                        "Two vartiants of same bundle cannot be loaded (this is a safety check)", bundleName, variantName));
				return;
			}
		}

		AssetBundleVars variantRef = new AssetBundleVars (bundleName, variantName);
		variantRefs.Add (variantRef);

//		if (bundleVariants.ContainsValue (bundleName)) 
//		{
//			Debug.Log(string.Format("Variant for {0} cannot be added. {1} already registered. " +
//				"Two vartiants of same bundle cannot be loaded (this is a safety check)", bundleName, variantName));
//			return;
//		}
//
//		bundleVariants.Add (bundleName, variantName);
	}

	public Object GetAssetFromLoadedBundle(string bundleName, string assetName)
	{
		if (!IsBundleLoaded (bundleName))
			return null;
	
		AssetBundleRef bundleRef = null;

		for(int i=0; i<bundleRefs.Count; i++)
		{
			if(bundleRefs[i].bundleName == bundleName)
			{
				bundleRef = bundleRefs[i];
			}
		}

		return bundleRef.bundle.LoadAsset (assetName);
	}


	public IEnumerator LoadBundleCoroutine(string bundleName, bool withDependencies = true)
	{
		Debug.LogWarning ("LoadBundleCoroutine: " + bundleName + " with Dependices: " + withDependencies.ToString());
		statusMsg = "에셋번들 로딩중: " + bundleName; 

		if (withDependencies) {
			string[] dependencies = manifest.GetAllDependencies (bundleName);
			for (int i = 0; i < dependencies.Length; i++)
			{
				Debug.LogWarning(bundleName + "'s dependency " + i + ": " + dependencies[i]);
				yield return StartCoroutine (LoadBundleCoroutine (dependencies [i]));
			}
		}

		bundleName = RemapVariantName (bundleName);
//		string url = pathToBundles + bundleName;
		string url = Path.Combine(pathToBundles, bundleName);
		Debug.LogError ("Beginning to load " + bundleName + " / " + manifest.GetAssetBundleHash(bundleName));

		bool cachingExists = false;
		if(Caching.IsVersionCached(bundleName, manifest.GetAssetBundleHash(bundleName)))
		{
			currentBundleProgress = 1.0F;
			cachingExists = true;
			do
			{
				Debug.LogError("Version is Cached for " + bundleName + "! Wait Until Caching is Ready");

				yield return null;
			}while(!Caching.ready);
			Debug.LogError(bundleName + "Caching Done!");
		}

		using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url, manifest.GetAssetBundleHash(bundleName))) 
		{
			if (cachingExists == false) 
			{
				do {
					currentBundleProgress = www.downloadProgress;
					Debug.LogWarning (bundleName + " - loading Bundle Progress: " + currentBundleProgress + " done? : " + www.isDone);
					statusMsg = "에셋 번들 로딩중: " + bundleName + " - " + (currentBundleProgress * 100F).ToString() + "%"; 

					yield return null;

				} while(!www.isDone);

				Debug.LogWarning (bundleName + " - loading Bundle Progress: " + currentBundleProgress + " done? : " + www.isDone);
			}

			if (!string.IsNullOrEmpty (www.error)) {
				Debug.Log (www.error);
				yield break;
			}

			if (IsBundleLoaded (bundleName) == false) {

				AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);

				AssetBundleRef bundleRef = new AssetBundleRef (bundle, bundleName);
				bundleRefs.Add (bundleRef);
			}
//			bundles.Add(bundleName, www.assetBundle);
			Debug.LogError ("Finished loading bundle: " + bundleName + " to a memory.");
			www.Dispose ();
		}
	}

//	void OnDisable()
//	{
//		Debug.Log ("Unloading Bundles");
//
////		foreach(KeyValuePair<string, AssetBundle> entry in bundles)
////			entry.Value.Unload(false);
//
//		for (int i=0; i<bundleRefs.Count; i++) 
//		{
//			bundleRefs[i].bundle.Unload(true); //false
//		}
//		bundleRefs.Clear ();
//
//		Resources.UnloadUnusedAssets ();
//		System.GC.Collect ();
//
////		bundles.Clear ();
//
//		Debug.Log ("Bundles Unloaded");
//	}

	public void Unload(string bundleName)
	{
		int unloadIndex = -1;

		for (int i=0; i<bundleRefs.Count; i++) 
		{
			if(bundleRefs[i].bundleName == bundleName)
			{
				bundleRefs[i].bundle.Unload(false);
				unloadIndex = i;
				break;
			}
		}

		if(unloadIndex >= 0)
		{
			bundleRefs.RemoveAt(unloadIndex);
		}
	}
	
	string RemapVariantName(string assetBundleName)
	{
		string[] splitBundleName = assetBundleName.Split('.');
		string variant = string.Empty; 

		for (int i=0; i<variantRefs.Count; i++) 
		{
			if(variantRefs[i].bundleName == splitBundleName[0])
			{
				variant = variantRefs[i].variantName;
			}
		}

		if (string.IsNullOrEmpty(variant)) 
		{
			return assetBundleName;
		}

//		if (!bundleVariants.TryGetValue(splitBundleName[0], out variant))
//			return assetBundleName;

		string[] bundlesWithVariant = manifest.GetAllAssetBundlesWithVariant();
		string newBundleName = splitBundleName [0] + "." + variant;

		if (System.Array.IndexOf(bundlesWithVariant, newBundleName) < 0 )
			return assetBundleName;

		return newBundleName;
	}
}




