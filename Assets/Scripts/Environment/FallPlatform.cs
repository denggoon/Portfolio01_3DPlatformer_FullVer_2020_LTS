using UnityEngine;
using System.Collections;

public class FallPlatform : Trap {

	private Animator animator;
	private Collider triggerCollider;
	private Collider platformCollider;

	private int idleHash;
	public string strWobbleAnimName = "G_002_FallPlatform_Wobble";
	private int wobbleHash;

	public float enduranceTime = 1F;

	private float remainingTime;

	public bool wobbleStarted = false;
	public float fallSpeedMultiplier = 1F;

	public bool isSinkPhase = false;

	public override void Awake ()
	{
		base.Awake ();

		animator = GetComponentInChildren<Animator> ();
		triggerCollider = GetComponent<Collider> ();
	}

	void Start()
	{
		animator.SetFloat ("FallSpeed", fallSpeedMultiplier);
		idleHash = animator.GetCurrentAnimatorStateInfo (0).shortNameHash;
		wobbleHash = Animator.StringToHash (strWobbleAnimName);
	}

	public int collisionTimes;
	void OnTriggerStay(Collider other)
	{
		if(GameRuleManager.instance.eGameStatus == E_GAME_STATUS.GAME_READY) return;
		if(GameRuleManager.instance.eGameStatus != E_GAME_STATUS.IN_PLAY) return;

		if(isTriggered == false) return;
		if (other.gameObject.tag == "Player")
		{
			if(isSinkPhase) return;

			Sink ();
		}
	}

	public void Sink()
	{
		isSinkPhase = true;
		animator.SetTrigger ("Sink");

		if(SoundBoard.instance != null)
			SoundBoard.instance.PlayFromSoundBoard("SND_GMK_002_TOUCH", this.transform.position);
	}

	public void StartWobble()
	{
		if (!wobbleStarted && isSinkPhase) 
		{
			animator.SetFloat ("EnduranceTime", enduranceTime);
			wobbleStarted = true;
			remainingTime = enduranceTime;
		}
	}

	public void EndSinkPhase()
	{
		isSinkPhase = false;
		wobbleStarted = false;
	}

	void Update()
	{
		if(GameRuleManager.instance.eGameStatus == E_GAME_STATUS.GAME_READY) return;
		if(GameRuleManager.instance.eGameStatus != E_GAME_STATUS.IN_PLAY) return;

		if(isTriggered == false) return;

		if (wobbleStarted) 
		{

			if (remainingTime > 0F) 
			{
				remainingTime -= Time.deltaTime;
				animator.SetFloat ("EnduranceTime", remainingTime);

			}

		}  else {

			if(animator.GetCurrentAnimatorStateInfo(0).shortNameHash == wobbleHash)
			{
				StartWobble();
			}
		}
	}


}
