using UnityEngine;
using System;
using System.Collections;

//플레이어가 획득하는 골드에 부착되는 스크립트 
public class ItemGold : KeyItem {

	public bool timedCollectTrigger  = false;

	public void Start()
	{
		if(this.gameObject.CompareTag("Gold") == false)
		{
			this.gameObject.tag = "Gold";
		}

		getSoundStr = "SND_ITM_COIN_GET";
	}

	public override void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.layer == LayerMask.NameToLayer("Player") == false) return; //골드에 닿은 상대가 플레이어일 경우 

		try {

			if (this.gameObject.transform.parent.parent.parent.localScale.x > 1.0 && this.gameObject.transform.parent.parent.parent.localScale.y > 1.0) {

				GameRuleManager.instance.AddGold (amount * 5); //Big Coin

			} else { 

				GameRuleManager.instance.AddGold(amount); //게임 룰 매니저에 접근, 골드 획등 총량을 늘림 
			}
		}
		catch (NullReferenceException e) {
			Debug.LogError("ItemGold OnTriggerEnter NullReferenceException : " + e.Message);
		}


		base.OnTriggerEnter(collider);
	}
}
