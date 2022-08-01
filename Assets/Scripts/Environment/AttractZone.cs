using UnityEngine;
using System.Collections;

public class AttractZone : Trap {

	private Animator animator;
	public Transform attractPoint;
	public float attractSpeed = 1F;
	private float animAttractFactor;

	public string fmodEventStr = "SND_GMK_004_LOOP";

	private FMOD.Studio.EventInstance loopEvent;
	public override void Awake ()
	{
		base.Awake ();

		animator = GetComponentInChildren<Animator> ();

		if (animator == null)
			animator = transform.parent.GetComponentInChildren<Animator> ();

		animAttractFactor = 1F / 7F;

		if (animator != null)
			animator.speed *= animAttractFactor;
	}

	public float initialWaitTime;
	IEnumerator Start()
	{
		while (GameRuleManager.instance.eGameStatus != E_GAME_STATUS.IN_PLAY) 
		{
			yield return null;
		}
		
		if (isTriggered) 
		{
			if(initialWaitTime > 0F)
			{
				StartCoroutine (WaitCoroutine ());
			} else {
				StartCoroutine (MainCoroutine());
			}
			
		}
	}

	public override void SwitchTrigger (bool flag)
	{
		base.SwitchTrigger (flag);
		
		if (flag) 
		{
			StartCoroutine (WaitCoroutine ());
		} else {
			StopCoroutine (WaitCoroutine ());
			StopCoroutine(MainCoroutine());
		}
	}
	
	IEnumerator WaitCoroutine()
	{
		yield return new WaitForSeconds (initialWaitTime);
		
		StartCoroutine (MainCoroutine ());
		
	}
	
	public int maxLoop;
	private int loopCount;
	public float showTime;
	public float hideTime;
	IEnumerator MainCoroutine()
	{
		ToggleOperate (true);
		
		if (showTime <= 0F || hideTime <= 0F) 
		{
			yield break;
		}
		
		if(maxLoop > 0)
			loopCount ++;
		
		yield return new WaitForSeconds (showTime);
		
		ToggleOperate (false);
		
		yield return new WaitForSeconds (hideTime);
		
		if (loopCount < maxLoop || maxLoop == 0) 
		{
			StartCoroutine (MainCoroutine ());
		} else {
			loopCount = 0;
		}
	}
	
	void ToggleOperate(bool flag)
	{
		isOperating = flag;

		if (flag) 
		{
			animator.speed = attractSpeed * animAttractFactor;

			if (SoundBoard.instance != null) {
				loopEvent = SoundBoard.instance.PlayLoopSoundFromBoard (fmodEventStr, this.gameObject);
			}
		} else {

			animator.speed = 0F;

			if (SoundBoard.instance != null) 
			{
				loopEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			}
		}
	}

	void Update()
	{
		if(GameRuleManager.instance.eGameStatus == E_GAME_STATUS.GAME_READY) return;
		if(GameRuleManager.instance.eGameStatus != E_GAME_STATUS.IN_PLAY) return;

		if(isTriggered == false) return;
		if(attractPoint == null) return;

		if(isOperating)
			animator.speed = attractSpeed * animAttractFactor;
	}

	[SerializeField]
	private bool isOperating;

	void OnTriggerStay(Collider col)
	{
		if(GameRuleManager.instance.eGameStatus == E_GAME_STATUS.GAME_READY) return;
		if(GameRuleManager.instance.eGameStatus != E_GAME_STATUS.IN_PLAY) return;

		if (isTriggered == false)
			return;

		if(attractPoint == null) return;

		if (col.gameObject != GameRuleManager.instance.player)
			return;

		if (isOperating) 
		{
			Vector3 dir = (this.attractPoint.position - GameRuleManager.instance.playerMove.transform.position).normalized;
			GameRuleManager.instance.playerMove.transform.Translate (dir * Time.deltaTime * attractSpeed, Space.World);	
		}
	}
}
