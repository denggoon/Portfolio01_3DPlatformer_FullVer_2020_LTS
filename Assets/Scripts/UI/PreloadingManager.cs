using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class PreloadingManager : MonoBehaviour {

	public Text statusText;
	public bool statusFine;

	public bool isConnectedToNet = false;
	private float netCheckTimeOut = 5F;

	public bool assetLoadingComplete = false;

    public TextAsset triggerSpawnDataFile;

	void Start()
	{
        TriggerSpawnDataLoader.Instance.LoadScript(triggerSpawnDataFile.text);

		StartCoroutine (PhaseOne ());
	}

	IEnumerator PhaseOne()
	{
		yield return StartCoroutine (CheckInternetReachability ());

		if (!isConnectedToNet) 
		{
			InquireOfflineMode();

			yield break;
		}

        //NviusNetManager.instance.Login(true);

        //while (NviusNetManager.instance.isLoginProcessDone == false) //성공-실패여부가 판가름 날때까지 대기 
        //{
        //    Debug.Log("Login Process: " + NviusNetManager.instance.isLoginProcessDone);
        //    statusText.text = "로그인 진행중..."; 
        //    yield return null;
        //}

        //if (NviusNetManager.instance.isLoggedIn) 
        //{
			yield return StartCoroutine (PhaseTwo ());
        //} else {

        //    InquireOfflineMode(); //로그인 실패시 오프라인모드로 마저 진행할것인지 물어본다 

        //}
	}

	public void InquireOfflineMode() //오프라인으로 진행할것인지 물어보는 곳 
	{
		UIPopupMsgManager.instance.PopQuestion 
			("인터넷에 연결되어있지 않거나 로그인에 실패하여 서버로부터 유저정보를 받아올 수 없습니다. 오프라인모드로 진행합니까?",
			 () => StartPhaseTwo (),
			 () => UIPopupMsgManager.instance.PopMessage ("확인을 눌러 프로그램을 종료합니다", () => Application.Quit ()));
	}

	public void StartPhaseTwo()
	{
		StartCoroutine (PhaseTwo ());
	}

	IEnumerator PhaseTwo()
	{
//		yield return StartCoroutine (LoadAssetBundles ()); //commented on 15-11-13
//
//		if (!assetLoadingComplete) 
//		{
//			UIPopupMsgManager.instance.PopQuestion 
//				("에셋을 로드해오는데 실패하였습니다. 재시도합니까? 사유: " + strBundleFailReason,
//				 () => StartPhaseTwo (),
//				 () => UIPopupMsgManager.instance.PopMessage ("확인을 눌러 프로그램을 종료합니다", () => Application.Quit ()));
//			
//			yield break;
//		}

		statusFine = true;

		UITimelineManager.instance.ExecuteTask ();
		statusText.text = "시작하려면 화면을 터치하세요."; 

		yield return null;
	}
	

	IEnumerator CheckInternetReachability()
	{
		float timer = netCheckTimeOut;
		
		Debug.Log("Checking Internet Connection...");
		statusText.text = "인터넷 연결 상태 확인중..."; 
		while (Application.internetReachability == NetworkReachability.NotReachable && timer > 0) 
		{
			statusText.text = "인터넷 연결 상태 확인중... (타임아웃 : " + timer + " 초.)"; 
			timer -= 1.0F;
			
			yield return new WaitForSeconds(1.0f);
		}
		
		if (timer <= 0) {
			isConnectedToNet = false;
			
		} else {
			isConnectedToNet = true;
		}
	}

	string strBundleFailReason;

	IEnumerator LoadAssetBundles()
	{
		strBundleFailReason = string.Empty;

		yield return StartCoroutine(BundleManager.instance.LoadManifest(BundleManager.instance.platform));

		if (!BundleManager.instance.IsManifestLoaded) 
		{
			strBundleFailReason = "Manifest 로딩 실패"; 
			yield break;
		}
		
		yield return StartCoroutine(BundleManager.instance.LoadBundleCoroutine("commonprefabs"));

		if (!BundleManager.instance.IsBundleLoaded ("commonprefabs")) 
		{
			strBundleFailReason = "공용 프리팹 로딩 실패"; 
			yield break;
		}
		
		yield return StartCoroutine(ObjectPooler.instance.Initialze());

		if (!ObjectPooler.instance.IsPoolerInitialized) 
		{
			strBundleFailReason = "오브젝트 풀러 초기화 실패"; 
			yield break;
		}

		assetLoadingComplete = true;
	}

	// Update is called once per frame
	void Update () 
	{
		if (!statusFine) 
		{
			if(!string.IsNullOrEmpty(BundleManager.instance.statusMsg))
				statusText.text = BundleManager.instance.statusMsg;
		}
		
		if (statusFine) 
		{
			if (Input.GetMouseButtonDown (0)) 
			{
				SceneManager.LoadScene("MainScene");
			}
			
		}
		
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}
}
