using UnityEngine;
using System.Collections;

public class CameraEvent : Trap {

	public Transform cameraTrans;
	public bool freezePlayerOnCameraEvent;
	public int maxLoop = 1; //카메라 루트가 설정된 행동을 반복하는 횟수 - 0: 무한 
	public int loopCount = 0; //그 횟수를 카운트하는 카운터 

	public override void SwitchTrigger(bool flag)
	{
		if(GameRuleManager.instance.sideCam == null) return;

		base.SwitchTrigger (flag);

		if (freezePlayerOnCameraEvent)
			GameRuleManager.instance.playerMove.onFreezeEvent = true;

		if (flag == false) 
		{
			GameRuleManager.instance.playerMove.onFreezeEvent = false;
		}

		Debug.Log ("CamEvent: Trigger: " + this.GetType() + " - " + flag);
	}
	// Update is called once per frame

	public virtual void Update () 
	{
		if (cameraTrans == null)
			cameraTrans = GameRuleManager.instance.sideCam.transform;
	}
}
