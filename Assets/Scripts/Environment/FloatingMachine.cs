using UnityEngine;
using System.Collections;

public class FloatingMachine : Trap {

	public float gravityAmount;
	private BoxCollider areaCollider;
	IEnumerator Start()
	{
		areaCollider = GetComponent<BoxCollider> ();

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

	[SerializeField]
	private bool isBlowing;

	public float initialWaitTime;

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

		ToggleBlow (true);

		if (showTime <= 0F || hideTime <= 0F) 
		{
			yield break;
		}

		if(maxLoop > 0)
			loopCount ++;

		yield return new WaitForSeconds (showTime);

		ToggleBlow (false);

		yield return new WaitForSeconds (hideTime);

		if (loopCount < maxLoop || maxLoop == 0) 
		{
			StartCoroutine (MainCoroutine ());
		} else {
			loopCount = 0;
		}
	}

	void ToggleBlow(bool flag)
	{
		isBlowing = flag;
	}

	void OnTriggerStay(Collider collider)
	{
		if (isTriggered == false)
			return;
		
		if(GameRuleManager.instance.eGameStatus != E_GAME_STATUS.IN_PLAY) 
			return;

		if (collider.gameObject.layer != LayerMask.NameToLayer ("Player"))
			return;


		if (isBlowing) 
		{
			GameRuleManager.instance.playerMove.ToggleAntiGravity (true, areaCollider.bounds.max.y, gravityAmount);
		} else {
			GameRuleManager.instance.playerMove.ToggleAntiGravity (false);
		}

	}

	void OnTriggerExit(Collider collider)
	{
		if(GameRuleManager.instance.eGameStatus != E_GAME_STATUS.IN_PLAY) 
			return;
		
		if (collider.gameObject.layer != LayerMask.NameToLayer ("Player"))
			return;

		GameRuleManager.instance.playerMove.ToggleAntiGravity (false);
	}
}
