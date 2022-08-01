using UnityEngine;
using System.Collections;

public class EnemyContactDamage : MonoBehaviour 
{

	public EnemyVision vision;
	public EnemyMovement movement;

	public float damage;
	public float knockbackDist;

	new public Transform transform;

	new public CapsuleCollider collider;

	public int playerLayer;

	void Awake()
	{
		collider = GetComponent<CapsuleCollider>();
		transform = GetComponent<Transform>();

		playerLayer = LayerMask.NameToLayer("Player");

		movement = GetComponentInParent<EnemyMovement> ();
	}

	void OnTriggerStay(Collider collider)
	{
		if(vision == null)
			vision = movement.GetVision();

		if(movement.IsStunned()) return; //적이 스턴 상태일때는 플레이어에게 접촉데미지를 주지 않습니다. 
		if (movement.health < 1) //적이 사망하였을때는 플레이어에게 데미지를 줘서는 안됩니다. 
			return; 

		if(collider.gameObject.layer == LayerMask.NameToLayer("Player")) //플레이어인 경우 
		{
			bool canBeAttacked = false;
			bool canPlayerBeDamaged = true;
			bool canKnockBackPlayer = false;

			switch(GameRuleManager.instance.playerMove.ePlayerAtkType) //플레이어의 공격 타입에 따라 갈립니다. 
			{
//				case E_PLAYER_ATTACK_TYPE.HOPNBOP: //플레이어가 적을 점프해서 밟는 공격형태인경우 
//
//					Vector3 enemyUpVector = transform.TransformDirection(Vector3.up); 
//					RaycastHit hit;
//
//				//적 현재 위치 기준으로 위로 레이캐스팅하여 플레이어가 적 머리 위에 있는지 체크 
//					if(Physics.SphereCast(transform.position, this.collider.radius, enemyUpVector, out hit, playerLayer))
//					{
//						canBeAttacked = true; //참이면 적은 공격당하는 입장이며 
//						canPlayerBeDamaged = false; //플레이어는 공격당할수 없는 입장이다 
//						canKnockBackPlayer = false; //플레이어를 넉백할수 없다 
//						GameRuleManager.instance.playerMove.Jump (true); //적을 밟으면 뚫고 땅에 착지하는것이 아닌 밟고 뛰어오르는 효과를 위해 점프를 시킴 
//					}
//
//				break;

				case E_PLAYER_ATTACK_TYPE.RUNNBUMP: //플레이어가 적에게 다가가 부딪히는 경우 
					

//					if(movement.eAIStatus != E_AI_STATUS.ATTACK)
//					{
					canBeAttacked = true;
					canPlayerBeDamaged = false;
//					}

					canKnockBackPlayer = true;
				break;

				default:
				break;
			}

			//무적 상태이거나 적이 공격을 당하는 입장인경우에는 플레이어는 넉백당하지 않습니다. 
			if(GameRuleManager.instance.playerHealth.isInvincible == false && canBeAttacked == false || canKnockBackPlayer) 
			{
				GameRuleManager.instance.playerMove.KnockBack((GameRuleManager.instance.playerMove.transform.position - transform.position).normalized, knockbackDist); //넉백 효과도 줌 
				
				if(vision == null) return;
				
				Vector3 playerPosYFixed = new Vector3(GameRuleManager.instance.playerMove.transform.position.x, this.transform.position.y, GameRuleManager.instance.playerMove.transform.position.z);
				//플레이어의 높이는 고려하지 않습니다. 현재 적이 위치한 동등한 높이에서만 시야를 확인합니다. 
				Vector3 dir = playerPosYFixed - vision.transform.position; //플레이어 위치에서 현위치를 빼면 현위치에서 플레이어 위치까지의 방향 벡터가 나옴 
				
				vision.transform.rotation = Quaternion.LookRotation (dir);
			}


			if(canPlayerBeDamaged) //플레이어가 데미지를 입을수 있는 상황이면 
			{
				GameRuleManager.instance.playerHealth.TakeDamage(damage); //데미지 줌 //무적이면 어차피 데미지가 들어가지는 않지만 프로세스상 넉백 뒤에 들어가야 하므로 여기 위치 
			}

			if(GameRuleManager.instance.playerMove.playerSkill != null && 
			   GameRuleManager.instance.playerMove.playerSkill is PlayerSkill_Harden && 
			   GameRuleManager.instance.playerMove.playerSkill.isActivated ||
			   canBeAttacked == true ||
			   GameRuleManager.instance.playerHealth.isItemInvincible)
			{
				//플레이어가 금강기술을 사용하고 있거나 
				//적이 공격당하는 입장에 처해있는경우 
				movement.Stun(damage); //스턴  
				return;
			}


		}
	}
}
