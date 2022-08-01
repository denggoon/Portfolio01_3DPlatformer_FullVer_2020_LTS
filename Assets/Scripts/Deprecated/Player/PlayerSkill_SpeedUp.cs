using UnityEngine;
using System.Collections;

public class PlayerSkill_SpeedUp : PlayerSkill {

	private float reservedSpeed = 0F;
	public float speedUpValue = 5F;

	public override void Activate ()
	{
		base.Activate ();

		reservedSpeed = GameRuleManager.instance.playerMove.speed;
		
		StartCoroutine("SpeedUp");
	}
	
	public IEnumerator SpeedUp()
	{
		GameRuleManager.instance.playerMove.speed = speedUpValue;

		yield return new WaitForSeconds(skillDuration);

		GameRuleManager.instance.playerMove.speed = reservedSpeed;
				
	}

	public float GetReservedSpeed()
	{
		return this.reservedSpeed;
	}
}
