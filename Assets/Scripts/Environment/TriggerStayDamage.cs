using UnityEngine;
using System.Collections;

public class TriggerStayDamage : MonoBehaviour {

	public float damage;
	public float knockbackDist;
	public Vector3 forceDir = Vector3.zero;

	public virtual void OnTriggerStay(Collider collider)
	{
		if(collider.gameObject.layer == LayerMask.NameToLayer("Player")) //플레이어인 경우 
		{
			if(GameRuleManager.instance.playerHealth.isInvincible == true && damage != 999) return;

			if(forceDir == Vector3.zero)
			{
				forceDir = (GameRuleManager.instance.playerMove.transform.position 
				            - new Vector3(	transform.position.x, 
				              				GameRuleManager.instance.playerMove.transform.position.y, 
				              				transform.position.z)).normalized;
			}

			GameRuleManager.instance.playerMove.KnockBack(forceDir, knockbackDist); //넉백 효과도 줌 
			
			GameRuleManager.instance.playerHealth.TakeDamage(damage); //데미지 줌 
		}
		
		//적인경우 
		if(collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
		{
			EnemyMovement enemyMove = collider.gameObject.GetComponent<EnemyMovement>();
			
			if(enemyMove != null)
			{
				enemyMove.Stun(damage); //적 스턴 
			}
		}
	}
}
