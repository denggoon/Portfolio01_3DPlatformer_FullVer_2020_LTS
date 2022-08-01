using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public enum E_GAME_RULE
{
	COLLECT,
	COLLECT_IN_TIME,
}

public enum E_GAME_STATUS
{
	GAME_READY,
	IN_PLAY,
	GAME_OVER,
	GAME_PAUSED,
}

//[ExecuteInEditMode]
public class GameRuleManager : MonoBehaviour {

	private static GameRuleManager instance_;

	public static GameRuleManager instance
	{
		get
		{
			return instance_;
		}
	}

	public Vector3 initPos;
	public Quaternion initRot;

	void OnDestoy()
	{
		instance_ = null;
	}

	public E_GAME_RULE eGameRule = E_GAME_RULE.COLLECT_IN_TIME;
	public E_GAME_STATUS eGameStatus = E_GAME_STATUS.GAME_READY;

	public float gameReadyTimer;
	public float gameReadyCount = 4F;

	public float goalTime = 60F;
	public float currentTime;

	public int collectGoal = 4; //게임을 종료하기 위해서 필요한 보석 수 
	public int currentGem; //현재 보유한 보석 수 
	public int currentGold; //현재 보유한 골드수 
	public int totalGold; //맵에 존재하는 모든 골드의 수 

	public GameObject player;
	public PlayerMoveCC playerMove;
	public PlayerHealth playerHealth;
	public bool isPlayerScriptSuccess = false;

	public string startPrefabStr;
	public string exitPrefabStr;

	private GameObject startPrefab;
	private GameObject exitPrefab;

	public string BGMPath;
	public string AmbientPath;

	public Vector3 latestCheckPoint;

	string DeathJingle = "Jingle/GameOver";
	string ClearJingle = "Jingle/GameClear";

	public RecordGameplay recorder;
	public ReplayGameplay replayer;

	public bool infiniteMode = false;
	public int infiniteBlockCount = 0;
	public GameObject infinitePrefab;
	public GameObject deathBall;

	public SideScrollCamera sideCam;

	void Awake()
	{
		instance_ = this;
		startPrefabStr = "startprefab";
		exitPrefabStr = "exitprefab";

		recorder = GetComponent<RecordGameplay> ();

		if(recorder == null)
			recorder = this.gameObject.AddComponent<RecordGameplay> ();


		if (infiniteMode == true) {

			GameObject instance = MonoBehaviour.Instantiate (infinitePrefab) as GameObject;
			GameObject rootObj = GameObject.Find("InfiniteMap");

			instance.transform.parent = rootObj.transform;
			instance.name = infinitePrefab.name;
			instance.transform.position = new Vector3(0, 0, 0);

			GameRuleManager.instance.infiniteBlockCount++;
		}
	}

	void Start()
	{
		if(Application.isEditor)
		{
			Application.targetFrameRate = 60;
		}

		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			Application.targetFrameRate = 60;
		}

		BGMPath = "SND_BGM_" + SceneManager.GetActiveScene().name;
		AmbientPath = "SND_AMB_" + SceneManager.GetActiveScene().name;

		eGameStatus = E_GAME_STATUS.GAME_READY;
		Time.timeScale = 1F;

		currentTime = goalTime;

		if (PurchasedItemEffect.instance != null)
			currentTime += PurchasedItemEffect.instance.GetItemEffectValue(PURCHASED_ITEM.EXTRA_TIME);

		gameReadyTimer = gameReadyCount;

		if (SoundBoard.instance != null) 
		{
			SoundBoard.instance.PlayFromSoundBoard (BGMPath);
			SoundBoard.instance.PlayFromSoundBoard (AmbientPath);
		}

		UIManager.instance.whiteMatte.FadeIn(UIManager.instance.fadeInTime);
		UIManager.instance.ShowStageName ();
		UIManager.instance.UpdateGold (currentGold);

		totalGold = FindObjectsOfType<ItemGold>().Length;
		collectGoal = FindObjectsOfType<ItemGem>().Length;

		LoadPlayerScripts();


	}

	public bool isCountDown = false;

	IEnumerator CountDownSound()
	{
		int count = 3;

		while(count > 0)
		{
			count --;

			yield return new WaitForSeconds(1.0F);
		}

	}

	void LoadPlayerScripts() //플레이어 관련 스크립트를 참조해 넣습니다. 
	{
		if(playerMove == null)
		{
			playerMove = FindObjectOfType<PlayerMoveCC>();
		}
		
		if(playerMove == null) return;
		
		if(player == null)
		{
			player = playerMove.gameObject;
		}

		sideCam = FindObjectOfType (typeof(SideScrollCamera)) as SideScrollCamera;
		
		if (sideCam != null) 
		{
			sideCam.playerMove = playerMove;
		}

		initPos = playerMove.transform.parent.position;
		initRot = playerMove.transform.parent.rotation;
		
		if(player == null) return;

		if(playerHealth == null)
			playerHealth = player.GetComponent<PlayerHealth>();
		
		if(playerHealth == null) return;

		if(SoundBoard.instance != null)
			SoundBoard.instance.PlayFromSoundBoard("Voice_Ready", playerMove.transform.position);

		isPlayerScriptSuccess = true;
	}

	void CheatKeys()
	{
		if (playerMove == null)
			return;

		if (Input.GetKeyDown (KeyCode.KeypadPlus)) {
			playerMove.speed ++;
		}
		
		if (Input.GetKeyDown (KeyCode.KeypadMinus)) {
			playerMove.speed --;
		}
	}

	void Update()
	{
//		CheatKeys ();
//
//		if(isPlayerScriptSuccess == false)
//		{
//			LoadPlayerScripts();
//		}
//
//		if(isPlayerScriptSuccess == false)
//		{
//			return;
//		}
//		GarbageCollect();

		if(eGameStatus == E_GAME_STATUS.GAME_READY)
		{
			gameReadyTimer -= Time.deltaTime;

			if(isCountDown == false)
			{
				isCountDown = true;
				StartCoroutine(CountDownSound());
			}

			if(gameReadyTimer <= 1F)
			{
				eGameStatus = E_GAME_STATUS.IN_PLAY;
				PurchasedItemEffect.instance.StartItemEffectDuration();
			}
		}
		else if(eGameStatus == E_GAME_STATUS.GAME_PAUSED)
		{
			Time.timeScale = 0F;
			return;
		}
		else if(eGameStatus != E_GAME_STATUS.IN_PLAY) return;

		if(Time.timeScale == 0F)
		{
			Time.timeScale = 1F;
		}

		switch(eGameRule)
		{
			case E_GAME_RULE.COLLECT_IN_TIME:

			currentTime -= Time.deltaTime;

//			if(currentTime <= 75F)
//			{
//				FMODSoundManager.instance.GetBGM().pitch = 1.1F;
//			}
//
//			if(currentTime <= 60F)
//			{
//				FMODSoundManager.instance.GetBGM().pitch = 1.2F;
//			}
//
//			if(currentTime <= 45F)
//			{
//				FMODSoundManager.instance.GetBGM().pitch = 1.3F;
//			}
//
//			if(currentTime <= 30F)
//			{
//				FMODSoundManager.instance.GetBGM().pitch = 1.4F;
//			}
//
//			if(currentTime <= 15F)
//			{
//				FMODSoundManager.instance.GetBGM().pitch = 1.5F;
//			}

			if(currentTime <= 0F)
			{
				currentTime = 0F;
				GameOver();
			}

			break;
		}

		if (infiniteMode == true) {
			if (deathBall != null) {
				deathBall.transform.Translate(new Vector3(1, 0 , 0) * Time.deltaTime * (1.5F * infiniteBlockCount));
			}
		}
	}

	public void GarbageCollect(bool forcedCollect = false)
	{
		if(Time.frameCount % 60 == 0 || forcedCollect == true)
		{
//			Debug.Log("Garbage Collection");
			System.GC.Collect();
		}
	}

	public void AddGold(int no)
	{
		currentGold += no;

		UIManager.instance.UpdateGold (currentGold);
	}

	public void AddGem(int no, Vector3 checkpointPos, bool isOpenExit = false)
	{
		currentGem += no;

		latestCheckPoint = checkpointPos;

		if(SoundBoard.instance != null)
			SoundBoard.instance.PlayFromSoundBoard("Voice_Start", playerMove.transform.position);

		UIManager.instance.gemCollected = currentGem;

		switch(eGameRule)
		{
			case E_GAME_RULE.COLLECT:
			case E_GAME_RULE.COLLECT_IN_TIME:
			
			if(this.currentGem >= collectGoal || isOpenExit)
			{
				OpenExit();
//				StageClear();
			}
			break;
		}

	}

	void OpenExit()
	{
		exitPrefab = ResourcesManager.instance.LoadGameObject ("item/" + exitPrefabStr);

		GameObject.Instantiate(exitPrefab, GameRuleManager.instance.player.transform.position + (Vector3.up * 1.5F), 
		                       exitPrefab.transform.rotation);

		if(SoundBoard.instance != null)
			SoundBoard.instance.PlayFromSoundBoard("SND_FX_PORTAL_SPAWN", this.transform.position);
	}


	public void StageClear()
	{
		//이미 게임 오버이면 할필요가 없다.
		if (eGameStatus == E_GAME_STATUS.GAME_OVER)
			return;

		eGameStatus = E_GAME_STATUS.GAME_OVER;

		UIManager.instance.ShowClearMsg();

		if(SoundBoard.instance != null)
			SoundBoard.instance.PlayFromSoundBoard("SND_JGL_GAMECLEAR", this.transform.position);

        // QQQ 임시로 star는 1, currentGold를 score로 대신한다. by kks
		//if(NviusNetManager.instance != null && BundleManager.instance != null)
  //      	NviusNetManager.instance.SaveStageRecord(BundleManager.instance.currentSceneName, 1, currentGold);
	}

	public void GameOver()
	{
		if (eGameStatus == E_GAME_STATUS.GAME_OVER)
			return;

		GameRuleManager.instance.playerMove.cnJoystick.Disable();
		Destroy(GameRuleManager.instance.playerMove.cnJoystick);
		eGameStatus = E_GAME_STATUS.GAME_OVER;

		UIManager.instance.ShowFailMsg();

		if(SoundBoard.instance != null)
			SoundBoard.instance.PlayFromSoundBoard("SND_JGL_GAMEOVER", this.transform.position);
	}

	public void RespawnAtCheckpoint()
	{
		if(latestCheckPoint != Vector3.zero)
		{
			if(playerMove != null)
			{
				Destroy(GameRuleManager.instance.playerMove.mainTransform.gameObject);
			}

			UIManager.instance.isGameFinished = false;
			UIManager.instance.isSuccess = true;

			UIManager.instance.ClosePauseAndResumeGame();

			Vector3 respawnPoint = new Vector3(latestCheckPoint.x, latestCheckPoint.y + 2F, latestCheckPoint.z);

			startPrefab = ResourcesManager.instance.LoadGameObject("item/" + startPrefabStr);

			GameObject.Instantiate(startPrefab, respawnPoint, Quaternion.identity);
		}
	}
}
