using UnityEngine;
using System.Collections;

public class AnimatedDamageZone : Trap {

	private Animator animator;
	private Collider triggerCollider;
	private TriggerStayDamage triggerStayDamage;

	private int idleHash;

	private AnimatorStateInfo curState;
//	private int curHash;

	public float damage = 1F; //스파이크가 주는 데미지 
	public float knockbackDist = 20F; //스파이크와 부딪혔을때 넉백 크기 
	public Vector3 forceDir = Vector3.zero;

	public bool isLoop = false;
	private bool isSwung = false;
	public float swingAngle = 90F;

	public float cycleWaitTime = 1.0F;
	private float timer;

	public float speedMultiplier = 1.0F;

	void Start()
	{
		animator = transform.GetComponentInChildren<Animator> ();
		animator.Play (idleHash);

		idleHash = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;

		animator.SetFloat ("loopWaitTime", cycleWaitTime);

		Collider[] collArr = transform.GetComponentsInChildren<Collider> ();

		for (int i=0; i<collArr.Length; i++) 
		{
			if(string.Equals(collArr[i].name, "trigger"))
				triggerCollider = collArr[i];
		}

		if (triggerCollider != null) 
		{
			triggerStayDamage = triggerCollider.gameObject.AddComponent<TriggerStayDamage> ();
			
			SyncValues();
		}

		fmodSoundIDs = new string[]{"SND_GMK_005_PLAY_001", "SND_GMK_005_PLAY_002"};
	}

	void SyncValues()
	{
		if (triggerStayDamage == null)
			return;

		if(triggerStayDamage.damage != this.damage)
			triggerStayDamage.damage = this.damage;
		
		if (triggerStayDamage.knockbackDist != this.knockbackDist)
			triggerStayDamage.knockbackDist = this.knockbackDist;
		
		if (triggerStayDamage.forceDir != this.forceDir)
			triggerStayDamage.forceDir = this.forceDir;

		animator.speed = speedMultiplier;

		animator.SetFloat ("swingAngle", swingAngle);
		animator.SetBool ("isLoop", isLoop);
		animator.SetFloat ("loopWaitTime", cycleWaitTime);
	}



	void LateUpdate()
	{
		if(GameRuleManager.instance.eGameStatus == E_GAME_STATUS.GAME_READY) return;
		if(GameRuleManager.instance.eGameStatus != E_GAME_STATUS.IN_PLAY) return;

		animator.enabled = isTriggered;

		if (isTriggered) {
			SyncValues ();

			curState = animator.GetCurrentAnimatorStateInfo(0);
//			curHash = curState.shortNameHash;

			if (curState.shortNameHash == idleHash)
			{
				if (isLoop)
				{
					timer -= Time.deltaTime;

					animator.SetFloat("loopWaitTime", timer);
				} else {

					if(isSwung == false)
					{
						animator.SetTrigger("tgrSwing");
//						isSwung = true;
					} else {

						isTriggered = false;
					}
				}

				isSwung = false;

			} else {

				timer = cycleWaitTime;
				isSwung = true;
			}
		}
	}

	public string[] fmodSoundIDs;
	private int enterSoundIdx = 0;
	public void PlayAnimSound()
	{
		enterSoundIdx ++;

		if (enterSoundIdx >= fmodSoundIDs.Length)
			enterSoundIdx = 0;

		SoundBoard.instance.PlayFromSoundBoard (fmodSoundIDs [enterSoundIdx], this.transform.position);

	}
}
