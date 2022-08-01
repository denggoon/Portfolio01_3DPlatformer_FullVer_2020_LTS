using UnityEngine;
using System.Collections;

public class PlayerSkill_Harden : PlayerSkill 
{
	public UnityEngine.AI.NavMeshObstacle navMeshObs;

	public override void Start()
	{
		base.Start();

		navMeshObs = GetComponent<UnityEngine.AI.NavMeshObstacle>();
		navMeshObs.enabled = false;
	}

	public override void Activate ()
	{
		base.Activate ();

		StartCoroutine("Harden");

	}

	public IEnumerator Harden()
	{
		GameRuleManager.instance.playerMove.blockMovement = true;

		GameRuleManager.instance.playerHealth.StartInvincible(skillDuration);

		while(skillTimer > 0)
		{
			GameRuleManager.instance.playerMove.animator.SetFloat("Speed", 0F);

			yield return null;
		}

		GameRuleManager.instance.playerMove.blockMovement = false;

		if(navMeshObs != null)
		{
			navMeshObs.enabled = false;
		}


	}

	public void StartBarrier()
	{
		StartCoroutine("BarrierOn");
	}

	IEnumerator BarrierOn() //공격 받을 그 프레임때만 잠깐 적을 밀치는 배리어를 켜는 방식 
	{
		if(navMeshObs != null)
		{
			navMeshObs.enabled = true;
		}

		yield return null;

		if(navMeshObs != null)
		{
			navMeshObs.enabled = false;
		}

	}
}
