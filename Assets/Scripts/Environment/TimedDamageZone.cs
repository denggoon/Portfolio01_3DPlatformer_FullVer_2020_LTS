using UnityEngine;
using System.Collections;

public class TimedDamageZone : TriggerStayDamage {

	public bool isTriggered = false; //트리거 되어있는 상태인가? 
	public bool isDangerous = false; //현재 닿으면 위험한 상태인가? 

	public bool receiveCallFromAnim = false; //애니메이션으로 부터 이벤트 호출을 받는 방식인지? 
	public Animation animClip;

	//이벤트 호출을 받는 방식이 아닌 수동방식이라면 타이머를 걸어주어야합니다. 
	public float damageTimer; //타이머 (값을 입력하지 않습니다.)
	public float damageInterval; //데미지를 주는 시간 간격 

	void Update()
	{
		if(GameRuleManager.instance.eGameStatus == E_GAME_STATUS.GAME_READY) return;
		if(GameRuleManager.instance.eGameStatus != E_GAME_STATUS.IN_PLAY) return;
		
		if(isTriggered == false) return; 
		//트리거가 발동되지 않았으면 작동하지 않음. 

		if (receiveCallFromAnim == true && animClip != null) //애니메이션으로 이벤트 호출을 받는 방식이고 애니메이션 클립이 등록되어있는경우 
		{
			if(animClip.isPlaying == false) //애니메이션이 아직 플레이 되지 않았다면 
				animClip.Play(); //플레이 

			return; //그 이후로는 애니메이션의 몫, 아무것도 하지 않습니다. 
		}

		if (damageInterval <= 0) //수동방식인데 타이머 설정이 되어있지 않으면 작동하지 않습니다. 
			return;

		damageTimer += Time.deltaTime;

		if (damageTimer >= damageInterval) //시간에 다다르면 
		{
			ToggleDangerous(); //위험 여부를 변동합니다. (위험한, 안전한 모드로 매 타임마다 바꿉니다.) 
			damageTimer = 0F;
		}
	}

	public override void OnTriggerStay (Collider collider)
	{
		if(isDangerous) //현재 위험한 상태일때만 TriggerStayDamage 클래스의 본분을 다합니다. 
			base.OnTriggerStay (collider);
	}

	public void ToggleDangerous() //애니메이션 호출 방식으로 할경우, 본 함수를 호출하도록 합니다. 
	{
		isDangerous = !isDangerous;
		Debug.Log("ToggleDangerous: " + isDangerous);
	}
}
