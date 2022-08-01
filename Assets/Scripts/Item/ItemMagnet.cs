using UnityEngine;
using System.Collections;

public class ItemMagnet : KeyItem {

	public override void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.layer == LayerMask.NameToLayer("Player") == false) return; //골드에 닿은 상대가 플레이어일 경우 
		
		GameRuleManager.instance.playerMove.AddPassive(System.Convert.ToInt32(PLAYER_PASSIVE.PlayerPassive_Magnet), amount);
		
		base.OnTriggerEnter(collider);
	}
}
