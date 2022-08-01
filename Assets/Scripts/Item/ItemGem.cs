using UnityEngine;
using System.Collections;

public class ItemGem : KeyItem 
{
	public bool isOpenExit;

	public override void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.layer == LayerMask.NameToLayer("Player") == false) return; //골드에 닿은 상대가 플레이어일 경우 

		GameRuleManager.instance.AddGem(amount, this.transform.position, isOpenExit); //게임 룰 매니저에 접근, 골드 획등 총량을 늘림 
		
		base.OnTriggerEnter(collider);
	}
}
