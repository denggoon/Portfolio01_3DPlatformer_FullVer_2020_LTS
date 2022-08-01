using UnityEngine;
using System.Collections;

public class PlayerSkill_Invisible : PlayerSkill {

	public override void Activate ()
	{
		base.Activate ();
		
		StartCoroutine("Invisible");
	}
	
	public IEnumerator Invisible()
	{
		while(skillTimer > 0F)
		{
			for(int i=0; i<GameRuleManager.instance.playerHealth.invincibleBody.Length; i++)
			{
				GameRuleManager.instance.playerHealth.invincibleBody[i].enabled = !GameRuleManager.instance.playerHealth.invincibleBody[i].enabled;
			}
			
			yield return null;
		}
		
		for(int i=0; i<GameRuleManager.instance.playerHealth.invincibleBody.Length; i++)
		{
			GameRuleManager.instance.playerHealth.invincibleBody[i].enabled = true;
		}
	}

	public override void CancelSkill()
	{
		base.CancelSkill();

		StopCoroutine("Invisible");

		for(int i=0; i<GameRuleManager.instance.playerHealth.invincibleBody.Length; i++)
		{
			GameRuleManager.instance.playerHealth.invincibleBody[i].enabled = true;
		}
	}
}
