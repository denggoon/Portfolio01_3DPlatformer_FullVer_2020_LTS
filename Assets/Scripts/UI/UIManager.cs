using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

	private static UIManager instance_;

	private Rect RScreenRect; //우측 화면 분할 

	public Text uiTxtFPS;
	public Text uiTxtCounter;
	public Text uiTxtTime;
	public Text uiTxtStageName;
	public Text uiTxtGameClear;
	public Text uiTxtGameOver;
	public Text uiTxtPause;

	public Text uiTxtLivesValue;
	public Text	uiTxtGoldValue;
	public Text	uiTxtPlayerSpeed;

	public GameObject pauseBtn;
	public GameObject mainMenuBtn;
	public GameObject retryBtn;
	public GameObject optionBtn;
	public OptionPanel optionPanel;
	public GameObject respawnBtn;
	public GameObject resumeBtn;



	public bool buttonPressed;
//	public bool specButtonPressed;

//	private float buttonW = Screen.width * .2F;
//	private float buttonH = Screen.height *.2F;

	public bool isGameFinished = false;
	public bool isSuccess = true;

	public int playerHealth;
	private string playerHealthSymbol;

	public int goldCollected;
	public int gemCollected;

	private float deltaTime;

	public MatteFade whiteMatte;
	public float fadeInTime = .3F;
	public float fadeOutTime = .5F;

	public GameObject pnlRecord;

//	public StringBuilder fpsSB = new StringBuilder ();
	public StringBuilder fpsDisplaySB = new StringBuilder ();
	public StringBuilder healthSB = new StringBuilder ();
	public StringBuilder goldSB = new StringBuilder ();

	private int beforeFpsIntValue;
	private float inchWide;
	private float screenWidth;
	private float screenHeight;
	private float screenDpi;

	public static UIManager instance
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

	void Awake()
	{
		instance_ = this;
	}

	void Start()
	{
		screenWidth = Screen.width;
		screenHeight = Screen.height;
		screenDpi =  Screen.dpi;
		inchWide = screenWidth / screenDpi;

		RScreenRect = new Rect (screenWidth * .5F, 0, screenWidth * .5F, screenHeight);

		fpsDisplaySB.Length = 0;
		fpsDisplaySB.Append (" Resolution: ");
		fpsDisplaySB.Append (screenWidth);
		fpsDisplaySB.Append (" X ");
		fpsDisplaySB.Append (screenHeight);
		fpsDisplaySB.Append (" (");
		fpsDisplaySB.Append (screenDpi);
		fpsDisplaySB.Append (" dpi - ");
		fpsDisplaySB.Append (inchWide);
		fpsDisplaySB.Append (" Inch Wide)");

//		StartCoroutine (ShowFPSDisplay ());
	}

//	void OnGUI()
//	{
//		int w = Screen.width, h = Screen.height;
//		
//		GUIStyle style = new GUIStyle();
//		
//		Rect rect = new Rect(14, h / 100 , w, h * 2 / 100);
//		style.alignment = TextAnchor.MiddleLeft;
//		style.fontSize = h * 2 / 100;
//		style.normal.textColor = new Color (1.0f, 0.0f, 0.0f, 1.0f);
//		float msec = deltaTime * 1000.0f;
//		float fps = 1.0f / deltaTime;
////		string text = string.Format("{0:0.0} ms ({1:0.} fps) {2}", msec, fps, fpsDisplaySB.ToString ());
//		fpsSB.Length = 0;
//		GUI.Label(rect, fpsSB.AppendFormat("{0:0.0} ms ({1:0.} fps) {2}", msec, fps, fpsDisplaySB.ToString ()).ToString(), style);
//	}

	public void ShowClearMsg()
	{
		uiTxtGameClear.enabled = true;
		isGameFinished = true;
		isSuccess = true;
	}

	public void ShowFailMsg()
	{
		uiTxtGameOver.enabled = true;
		isGameFinished = true;
		isSuccess = false;
	}

	public void AddHealth()
	{
		healthSB.Append("♥ ");

		uiTxtLivesValue.text = healthSB.ToString ();
	}

	public void LoseHealth()
	{
		if (healthSB.Length >= 2) {
			healthSB.Remove (healthSB.Length - 2, 2);
		} else {
			healthSB.Remove(0, 1);
		}

//		Debug.Log ("LoseHealth - sblenth: " + healthSB.Length);

		uiTxtLivesValue.text = healthSB.ToString ();
	}

	public void UpdateGold(int gold)
	{
		goldSB = new StringBuilder ();
		goldSB.Append ("$ ");
		goldSB.Append (gold);

		uiTxtGoldValue.text = goldSB.ToString ();
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) //안드로이드 에서의 백버튼을 씬선택메뉴로. 
		{
			TogglePause();
		}

		ShowGameEndBtns();

		ShowGameStartCountDown();

		ShowTime();

		TouchBeginToDragInput();

		ShowFPSDisplay();
	}

	public void TogglePause()
	{
		if(isGameFinished == true) return;

		if(GameRuleManager.instance.eGameStatus == E_GAME_STATUS.IN_PLAY)
		{
			GameRuleManager.instance.eGameStatus = E_GAME_STATUS.GAME_PAUSED;

			GameRuleManager.instance.GarbageCollect(true);
			Resources.UnloadUnusedAssets();

			uiTxtPause.enabled = true;

			if(mainMenuBtn.activeSelf == false)
			{
				mainMenuBtn.SetActive(true);
			}
			
			if(retryBtn.activeSelf == false)
			{
				retryBtn.SetActive(true);
			}

			if(optionBtn.activeSelf == false)
			{
				optionBtn.SetActive(true);
			}

			if(resumeBtn.activeSelf == false)
			{
				resumeBtn.SetActive(true);
			}

			if (pauseBtn.activeSelf == true) 
			{
				pauseBtn.SetActive(false);
			}

			if(pnlRecord.activeSelf == false)
			{
				pnlRecord.SetActive(true);
			}

		} else 
		if(GameRuleManager.instance.eGameStatus == E_GAME_STATUS.GAME_PAUSED)
		{
			ClosePauseAndResumeGame();
		}
	}

	public void ClosePauseAndResumeGame()
	{
		GameRuleManager.instance.eGameStatus = E_GAME_STATUS.IN_PLAY;

		if(uiTxtPause.enabled)
			uiTxtPause.enabled = false;

		if(uiTxtGameOver.enabled)
			uiTxtGameOver.enabled = false;

		if (pauseBtn.activeSelf == false) 
		{
			pauseBtn.SetActive(true);
		}
		
		if(mainMenuBtn.activeSelf == true)
		{
			mainMenuBtn.SetActive(false);
		}

		if(resumeBtn.activeSelf == true)
		{
			resumeBtn.SetActive(false);
		}

		if(retryBtn.activeSelf == true)
		{
			retryBtn.SetActive(false);
		}
		
		if(optionBtn.activeSelf == true)
		{
			optionBtn.SetActive(false);
		}
		
		if(optionPanel.gameObject.activeSelf == true)
		{
			optionPanel.SyncPlayerValues();
			optionPanel.gameObject.SetActive(false);
		}
		
		if(respawnBtn.activeSelf == true)
		{
			respawnBtn.SetActive(false);
		}

		if(pnlRecord.activeSelf == true)
		{
			pnlRecord.SetActive(false);
		}

	}

	public void PressButton()
	{
		buttonPressed = true;
	}

	public void Retry()
	{
		if (BundleManager.instance == null) 
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name); //에셋번들은 안먹힘 

		} else {

			//			AsyncOperation sceneAsync = Application.LoadLevelAsync (BundleManager.instance.currentSceneName);
			SceneManager.LoadSceneAsync (BundleManager.instance.currentSceneName);
		}

		isGameFinished = false;
		isSuccess = false;

		Time.timeScale = 1F;
	}

	public void NextStage() //다음 스테이지로 가는것 없어지고 무조건 메인 메뉴로 복귀. (아이템 설정등의 이유로) 
	{
		isGameFinished = false;
		isSuccess = false;

		SceneManager.LoadScene("MainScene");
		BundleManager.instance.Unload(BundleManager.instance.currentSceneName.ToLower());

//		if(Application.levelCount == Application.loadedLevel + 1)
//		{
//			Application.LoadLevel("MainScene");
//			BundleManager.instance.Unload(PlayerPrefs.GetString("LoadingSceneName").ToLower());
//		} else {
//
//			Application.LoadLevel(Application.loadedLevel + 1);
//		}
	}

	public void MainMenu()
	{
		SceneManager.LoadScene("MainScene");
		isGameFinished = false;
		isSuccess = false;

		Time.timeScale = 1F;
	}

	void ShowGameEndBtns()
	{
		if (isGameFinished) 
		{
			if(mainMenuBtn.activeSelf == false)
			{
				mainMenuBtn.SetActive(true);
			}

			if(retryBtn.activeSelf == false)
			{
				retryBtn.SetActive(true);
			}

			if(isSuccess)
			{
				//
			} else {
				if(GameRuleManager.instance.latestCheckPoint != Vector3.zero)
				{
					if(respawnBtn.activeSelf == false)
						respawnBtn.SetActive(true);
				}
			}
		}
	}

	private float goTime = .5F;
	void ShowGameStartCountDown()
	{
		if(GameRuleManager.instance.eGameStatus == E_GAME_STATUS.GAME_READY)
		{
			if(uiTxtCounter.enabled == false)
			{
				uiTxtCounter.enabled = true;
			}

			int timerCount = (int)GameRuleManager.instance.gameReadyTimer;

			if(timerCount == 1)
			{
				uiTxtCounter.text = "";
			} else {
				uiTxtCounter.text = "READY"; //timerCount.ToString();
			}
		} else {

			if(goTime > 0F)
			{
				uiTxtCounter.text = "GO!";
				goTime -= Time.deltaTime;
			} else {

				if(uiTxtCounter.enabled == true)
				{
					uiTxtCounter.enabled = false;
				}
			}
		}
	}

	void ShowTime()
	{
		if(GameRuleManager.instance.eGameRule == E_GAME_RULE.COLLECT_IN_TIME)
		{
			if(uiTxtTime.enabled == false)
			{
				uiTxtTime.enabled = true;
			}

			uiTxtTime.text = GameRuleManager.instance.currentTime.ToString("F2");
		}
	}

	public void ShowStageName()
	{
		uiTxtStageName.text = "- " + SceneManager.GetActiveScene().name + " -";
	}
	
	void ShowFPSDisplay()
	{
		if (Time.timeScale == 0F)
			return;
		
		deltaTime += (Time.deltaTime - deltaTime) * .1F;
		
		float msec = deltaTime * 1000.0f;
		float newFloatFps = 1.0F / deltaTime;
		if (beforeFpsIntValue != (int)newFloatFps) {
			uiTxtFPS.text = string.Format ("{0:0.0} ms ({1:0.} fps) {2}", msec, newFloatFps, fpsDisplaySB.ToString ());
			beforeFpsIntValue = (int)newFloatFps;
		}
	}

//	IEnumerator ShowFPSDisplay()
//	{
//		while (true) {
//			if (Time.timeScale == 0F)
//				yield return null;
//
//			deltaTime += (Time.deltaTime - deltaTime) * .1F;
//
//			float msec = deltaTime * 1000.0f;
//			float newFloatFps = 1.0F / deltaTime;
//			if (beforeFpsIntValue != (int)newFloatFps) {
//				uiTxtFPS.text = string.Format("{0:0.0} ms ({1:0.} fps) {2}", msec, newFloatFps, fpsDisplaySB.ToString ());
//				beforeFpsIntValue = (int)newFloatFp
//			}
//
////			yield return new WaitForSeconds(0.1f);
//			yield return new WaitForEndOfFrame ();
//		}
//	}

	void TouchBeginToDragInput() //최초 터치한 지점을 기준으로 드래그해서 움직이는 방식 
	{
		buttonPressed = false; //기본적으로 우측화면 터치를 통한 버튼입력은 매프레임 false이다. 
		
		foreach (Touch touch in Input.touches) //화면에 일어난 모든 터치를 전제로 한다. 
		{
			float yInverse = Screen.height - touch.position.y; //터치 입력의 좌표는 Rect방식과 달라 Y값이 반대이므로 
			
			Vector2 touchPosition = touch.position; //이런식으로 Y값을 재배치한다. 
			
			touchPosition = new Vector2(touch.position.x, yInverse);		
			
			if(touch.phase.Equals(TouchPhase.Began)) //만약 본 터치가 막 터치를 시작한 터치인 경우 
			{
				if (RScreenRect.Contains(touchPosition)) //아닌경우 우측 화면으로 인식, 
				{
					buttonPressed = true; //아니라면 일반 버튼이 눌리었다. 
				}
			}
		}
	}
	
	public Rect GetRRect()
	{
		return this.RScreenRect;
	}
}

