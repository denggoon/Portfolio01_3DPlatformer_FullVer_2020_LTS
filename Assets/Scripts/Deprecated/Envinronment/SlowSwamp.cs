using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlowSwamp : MonoBehaviour {

	public float slowPercentage = 50F;

	public List<EnemyMovement> enemyReservedSpeed = new List<EnemyMovement>();
	public float reservedSpeed = 0F;

	public PlayerMoveCC localPmcc;

	void OnTriggerEnter(Collider collider) //늪에 들어간경우 
	{
		if(collider.gameObject.layer == LayerMask.NameToLayer("Enemy")) //적인경우 
		{
			EnemyMovement enemyMove = collider.gameObject.GetComponent<EnemyMovement>();

			if(enemyMove != null)
			{
				if(enemyMove == null) return;
				
				enemyReservedSpeed.Add(enemyMove); //적의 속도 정보를 저장 
				
				enemyMove.SetModifiedSpeed(enemyMove.currentSpeed * slowPercentage * .01F); //적을 느리게함 
				enemyMove.GetAnimator().SetBool("IsSlowed", true); //감속 전용 애니메이션을 넣음 
			}
		}
	}
	
	void OnTriggerStay(Collider collider) //플레이어는 늪에 들어가있는 와중에도 속도가 변화무쌍하므로 
	{
		if(collider.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			if(localPmcc == null)
				localPmcc = GameRuleManager.instance.playerMove;

			if(localPmcc != null)
			{
				if(reservedSpeed == 0F)
				{
					//스킬을 사용중일때는 현재 스피드를 저장하지 않고 스킬에 저장된 원래 스피드를 저장 
					if(localPmcc.playerSkill != null && localPmcc.playerSkill is PlayerSkill_SpeedUp && localPmcc.playerSkill.isActivated == true)
					{
						PlayerSkill_SpeedUp speedSkill = (PlayerSkill_SpeedUp)localPmcc.playerSkill;
						reservedSpeed = speedSkill.GetReservedSpeed();

					} else {

						reservedSpeed = localPmcc.speed; //스킬을 안썼거나 안들고 있으면 상관없음 
					}
				}

				//스킬을 쓰던말던 얄짤없이 기본 속도의 설정된 감속률 만큼 감속됨 
				localPmcc.speed = reservedSpeed * slowPercentage * .01F;

			} 
		}

	}

	void OnTriggerExit(Collider collider) //늪에서 빠져나올시 
	{
		if(collider.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			if(localPmcc == null)
				localPmcc = GameRuleManager.instance.playerMove;

			if(localPmcc != null)
			{
				//늪에서 나오는 시점에도 스킬을 사용중이면 
				if(localPmcc.playerSkill != null && localPmcc.playerSkill is PlayerSkill_SpeedUp && localPmcc.playerSkill.isActivated == true)
				{
					PlayerSkill_SpeedUp speedSkill = (PlayerSkill_SpeedUp)localPmcc.playerSkill;

					localPmcc.speed = speedSkill.speedUpValue; //스킬 사용효과 속도로 복귀 (그리고 스킬 시한이 지나면 원래 속도로 복귀할것이므로 
				} else {
					localPmcc.speed = reservedSpeed; //스킬이랑 상관없는 상태이면 그냥 눨속도로 복귀 
				}

				reservedSpeed = 0F; //저장한 값은 없앰 
				localPmcc = null;
			}
		}

		if(collider.gameObject.layer == LayerMask.NameToLayer("Enemy")) //늪에서 빠져나온게 적이라면 
		{
			EnemyMovement enemyMove = collider.gameObject.GetComponent<EnemyMovement>();

			if(enemyMove != null)
			{
				for(int i=0; i<enemyReservedSpeed.Count; i++)
				{
					EnemyMovement tempEnemyMove = enemyReservedSpeed[i];

					if(tempEnemyMove.Equals(enemyMove)) //저장된 적 속도 정보를 꺼냄 
					{
						enemyMove.SetModifiedSpeed(-999F); //적이 더이상 감속하지 않음 (-999로 해놓으면 원래의 속도로 돌아감) 
						enemyReservedSpeed.RemoveAt(i);
						enemyMove.GetAnimator().SetBool("IsSlowed", false);

						break;
					}
				}
			}
		}
	}
}
