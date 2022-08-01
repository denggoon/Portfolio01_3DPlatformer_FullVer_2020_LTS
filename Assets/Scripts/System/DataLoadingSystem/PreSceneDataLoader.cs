using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class PreSceneDataLoader : MonoBehaviour 
{
	private static PreSceneDataLoader instance_;

	public static PreSceneDataLoader instance
	{
		get
		{
			return instance_;
		}
	}

	void OnDestroy()	
	{
		instance_ = null;
	}

	public BankLoader bankLoader;
	public SceneLoader sceneLoader;

	public Text uiTxtProgress;

	public bool finishedDownloadingScene = false;

	void Awake() 
	{
		instance_ = this;
	}

	IEnumerator Start () 
	{
		string sceneName = PlayerPrefs.GetString("LoadingSceneName");

//		yield return StartCoroutine(BundleManager.instance.LoadBundleCoroutine (sceneName.ToLower()));

		yield return StartCoroutine (bankLoader.Execute ());

		yield return StartCoroutine (sceneLoader.Execute ());

	}

	void Update()
	{
		if(finishedDownloadingScene == false)
			uiTxtProgress.text = BundleManager.instance.statusMsg;
	}
}
