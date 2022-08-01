using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//키 아이템을 설정한 갯수만큼 모으면 트리거가 켜지도록 하는 함수 입니다. 
public class KeyItemTrigger : ToggleTrigger {

	public List<KeyItem> uncollectedItems = new List<KeyItem>(); //아직 모으지 않은 아이템들의 목록 
	public List<KeyItem> collectItems = new List<KeyItem>(); //모으면 이곳으로 이동 

	public int goalCount; //목표 갯수 
	public int currentCount; //현재 갯수 

	public void UpdateItemCollect(KeyItem keyItem) //해당 키아이템을 모으면 이 함수 호출 
	{
		if(uncollectedItems.Contains(keyItem)) //아이템 목록에 포함되어있으면 
		{
			uncollectedItems.Remove(keyItem); //목록에서 제외 
			collectItems.Add(keyItem); //획득 목록으로 이동 

			currentCount++; //획득 갯수 늘림 
		}

		if(currentCount == goalCount) //골에 도달하면 
		{
			EnableTrigger(); //트리거 발동 
		}
	}
}
