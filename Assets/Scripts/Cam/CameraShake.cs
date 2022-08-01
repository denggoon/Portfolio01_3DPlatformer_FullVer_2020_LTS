using UnityEngine;
using System.Collections;

public class CameraShake : CameraEvent 
{
	// How long the object should shake for.
	public float shakeTime = 0f;
	private float shakeTimer;
	
	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.7f;
	public float decreaseFactor = 1.0f;

	[SerializeField]
	private Vector3 originalPos;

	public override void SwitchTrigger (bool flag)
	{
		base.SwitchTrigger (flag);

		if (flag == true) 
		{
			originalPos = cameraTrans.localPosition;
			shakeTimer  = shakeTime;
		}
	}
	
	public override void Update()
	{
		base.Update ();

		if(GameRuleManager.instance.eGameStatus != E_GAME_STATUS.IN_PLAY) return;

		if (cameraTrans == null)
			return;

		if (isTriggered == false) 
			return;

		if (loopCount < maxLoop || maxLoop == 0) 
		{
			if (shakeTimer > 0F) 
			{
				cameraTrans.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
				shakeTimer -= Time.deltaTime * decreaseFactor;

			} else {
				shakeTimer = 0F;
				cameraTrans.localPosition = originalPos;
				loopCount ++;
			}
		}

		if(maxLoop >=1 && loopCount >= maxLoop) //반복 횟수가 설정되어 있고 그 횟수에 도달했을경우 
		{
			SwitchTrigger(false); //트리거 발동 장치를 다시 리셋 
			loopCount = 0; //반복 횟수도 리셋 
		}
	}
}
