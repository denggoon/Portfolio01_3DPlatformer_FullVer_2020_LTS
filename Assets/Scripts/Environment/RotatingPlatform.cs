using UnityEngine;
using System.Collections;

public enum E_SPINNING_AXIS
{
	UP,
	FORWARD,
	RIGHT,
}

public class RotatingPlatform : Trap {
	
	private Transform rotatingTarget;
	private Collider[] slidingColliders;
		
	public int maxLoop = 0; //스파이크가 설정된 행동을 반복하는 횟수 - 0: 무한 
	public int loopCount = 0; //그 횟수를 카운트하는 카운터 
	
	public float rotateSpeed = 100F; //스파이크의 회전 속도 
	public bool isClockwise = true;
	private float preservedAngle;
	
	public E_SPINNING_AXIS eAxis = E_SPINNING_AXIS.UP;

	public bool isPlayingSound = false;
	
	private FMOD.Studio.EventInstance loopEvent;
	
	public override void Awake ()
	{
		base.Awake ();
		
		if(transform.childCount > 0)
			rotatingTarget = transform.GetChild (0);
		
		if(rotatingTarget != null)
			slidingColliders = rotatingTarget.GetComponentsInChildren<Collider> ();
		
		if (slidingColliders != null) 
		{
			for (int i=0; i<slidingColliders.Length; i++) 
			{
				slidingColliders [i].tag = "MovingPlatform";
			}
		}
	}
	
	void Update()
	{
		if(GameRuleManager.instance.eGameStatus == E_GAME_STATUS.GAME_READY) return;
		if(GameRuleManager.instance.eGameStatus != E_GAME_STATUS.IN_PLAY) return;

		if (isTriggered) 
		{
			if (!isPlayingSound) 
			{
				if (SoundBoard.instance != null) 
				{
					loopEvent = SoundBoard.instance.PlayLoopSoundFromBoard ("SND_GMK_006_LOOP", this.gameObject);
					isPlayingSound = true;
				}
			}
		} else {
			if (isPlayingSound) 
			{
				if (SoundBoard.instance != null) 
				{
					loopEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
					isPlayingSound = false;
				}
			}
		}

		if(isTriggered == false) return; 
		if (rotatingTarget == null)
			return;
		if(rotateSpeed == 0F) return;
		
		int clockWiseInt = 1;
		
		if(!isClockwise)
		{
			clockWiseInt = -1;
		}
		
		if(loopCount < maxLoop || maxLoop == 0)
		{
			Vector3 spinningAxis = rotatingTarget.up;
			
			switch(eAxis)
			{
			case E_SPINNING_AXIS.UP:
				spinningAxis = rotatingTarget.up;
				break;
				
			case E_SPINNING_AXIS.FORWARD:
				spinningAxis = rotatingTarget.forward;
				break;
				
			case E_SPINNING_AXIS.RIGHT:
				spinningAxis = rotatingTarget.right;
				break;
			}
			
			rotatingTarget.Rotate(spinningAxis, clockWiseInt * rotateSpeed * Time.deltaTime, Space.World);
			
			preservedAngle += rotateSpeed * Time.deltaTime;
			
			if(preservedAngle > 360F)
			{
				if(maxLoop != 0)
					loopCount++;
				
				preservedAngle = 0F;
			}
			
		}
		
		if(maxLoop >=1 && loopCount >= maxLoop) //반복 횟수가 설정되어 있고 그 횟수에 도달했을경우 
		{
			SwitchTrigger(false); //트리거 발동 장치를 다시 리셋 
			loopCount = 0; //반복 횟수도 리셋 
		}
	}
}
