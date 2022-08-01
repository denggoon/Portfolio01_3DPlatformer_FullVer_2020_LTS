using UnityEngine;
using System.Collections;

public class TimedFunctionCaller : Trap {

	public float waitTime;
	public float timer;
	public bool isLoop;

	public Component funcitonTarget;
	public string functionName;

	void Update () 
	{
		if(GameRuleManager.instance.eGameStatus == E_GAME_STATUS.GAME_READY) return;
		if(GameRuleManager.instance.eGameStatus != E_GAME_STATUS.IN_PLAY) return;

		if (isTriggered == false)
			return;

		if(funcitonTarget == null) return;
		if(functionName == null) return;
		if(functionName == "") return;
		if(waitTime == 0F) return;

		timer += Time.deltaTime;

		if(timer > waitTime)
		{
			funcitonTarget.SendMessage(functionName, SendMessageOptions.DontRequireReceiver);
			timer = 0F;

			if(isLoop == false)
				SwitchTrigger(false);
		}
	}
}
