using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour 
{
	[SerializeField] string bundleName;
	[SerializeField] string sceneName;
	[SerializeField] string optionalVariantBundle;
	[SerializeField] string optionalVariantName;

	public bool useBundle = false;
	public bool isComplete;
	public string statusMsg;
	public float progress;

	void Awake()
	{
		this.enabled = false;
	}

	void Update()
	{
		PreSceneDataLoader.instance.uiTxtProgress.text = statusMsg;
	}

	public IEnumerator Execute () 
	{
		this.enabled = true;
		sceneName = PlayerPrefs.GetString("LoadingSceneName");

		BundleManager.instance.currentSceneName = sceneName;

		if (useBundle) 
		{
			bundleName = sceneName.ToLower();

			if (!string.IsNullOrEmpty (optionalVariantBundle) && !string.IsNullOrEmpty (optionalVariantName))
				BundleManager.instance.RegisterVariant (optionalVariantBundle, optionalVariantName);

			while (!BundleManager.instance.isReady)
				yield return null;

			yield return StartCoroutine(BundleManager.instance.LoadBundleCoroutine (bundleName));
		}



		if (Application.platform == RuntimePlatform.IPhonePlayer) 
		{
			SceneManager.LoadScene(sceneName);

		} else {

			AsyncOperation sceneAsync = SceneManager.LoadSceneAsync(sceneName);
		
			do {  
				float sceneProgress = sceneAsync.progress * 100F;
				progress = sceneProgress;
				
				int scenePRounded = Mathf.RoundToInt (sceneProgress);
				
				statusMsg = "스테이지 로딩중:" + sceneName + "(" + scenePRounded + "%)"; 
				yield return null;  
				
			} while(!sceneAsync.isDone);  

		}

//		BundleManager.instance.Unload (bundleName);
		this.enabled = false;
	}
}
