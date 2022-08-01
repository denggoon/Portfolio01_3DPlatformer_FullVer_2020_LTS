using UnityEngine;
using System.Collections;

public class StomperDamageTrigger : MonoBehaviour {

	new public Transform transform;
	public Stomper stomper;

	public float damage = 1F; //스파이크가 주는 데미지 
	public float knockbackDist; //스파이크와 부딪혔을때 넉백 크기

	void Awake()
	{
		transform = GetComponent<Transform>();
	}

	void Start()
	{
		this.stomper = transform.parent.GetComponent<Stomper>();
	}

	void OnTriggerEnter(Collider collider)
	{
		if(this.stomper == null) return;

		if(this.stomper.isLifting) return;

		if(collider.gameObject.layer == LayerMask.NameToLayer("Player")) //플레이어인 경우 
		{
			if(GameRuleManager.instance.playerHealth.isInvincible == true) return;
			
			GameRuleManager.instance.playerMove.KnockBack((GameRuleManager.instance.playerMove.transform.position - transform.position).normalized, knockbackDist); //넉백 효과도 줌 
			
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
