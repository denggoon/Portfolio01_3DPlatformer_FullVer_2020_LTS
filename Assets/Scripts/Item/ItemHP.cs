using UnityEngine;
using System.Collections;

public class ItemHP : KeyItem {

	public override void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.layer == LayerMask.NameToLayer("Player") == false) return; //골드에 닿은 상대가 플레이어일 경우 
		
		GameRuleManager.instance.playerHealth.AddHealth (amount);
		
		base.OnTriggerEnter(collider);
	}
}
