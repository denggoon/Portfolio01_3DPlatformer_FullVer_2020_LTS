using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum INPUT_MODE
{
	FREE_JOYSTICK,
	STATIC_JOYSTICK,
	GAMEPAD
}

public class OptionPanel : MonoBehaviour {

	public INPUT_MODE eConfirmedInputMode = INPUT_MODE.GAMEPAD;
	public INPUT_MODE eChoiceInputMode = INPUT_MODE.GAMEPAD;

	public float playerSpeed;
	public float rotateSpeed;
	public float rotateAngle;
	public float idleLimit;
	public float airIdleLimit;
	public float maxSpeedReachPercentage;

	public bool freeJoystick = true;
	public bool gamePadOverlay = false;

	public Slider playerSpdSlider;
	public Slider rotateSpdSlider;
	public Slider rotateAngleSlider;
	public Slider idleLimitSlider;
	public Slider airIdleLimitSlider;
	public Slider maxSpeedReachSlider;

	public Text uiTxtPlayerSpd;
	public Text uiTxtRotateSpd;
	public Text uiTxtRotateAngle;
	public Text uiTxtIdleLimit;
	public Text uiTxtAirIdleLimit;
	public Text uiTxtMaxSpeedReach;

	public GamePad gamePadSys;

	public Toggle freeJoystickToggle;
	public Toggle staticJoystickToggle;
	public Toggle gamePadOverlayToggle;

	void Start()
	{
		this.gameObject.SetActive(false);
	}

	public void ReturnToDefaultSettings()
	{
		GameRuleManager.instance.playerMove.RegisterDefaultValues();

		SyncPlayerValues();
	}

	public void SyncPlayerValues()
	{
		playerSpdSlider.value = GameRuleManager.instance.playerMove.speed / GameRuleManager.instance.playerMove.maxSpeed * playerSpdSlider.maxValue;
		rotateSpdSlider.value = GameRuleManager.instance.playerMove.rotateSpeed / GameRuleManager.instance.playerMove.maxRotateSpeed * rotateSpdSlider.maxValue;
		rotateAngleSlider.value = GameRuleManager.instance.playerMove.minRotateDegree / GameRuleManager.instance.playerMove.maxRotateDegree * rotateAngleSlider.maxValue;
		idleLimitSlider.value = GameRuleManager.instance.playerMove.idleLimitValue / GameRuleManager.instance.playerMove.maxIdleLimit * idleLimitSlider.maxValue;
		airIdleLimitSlider.value = GameRuleManager.instance.playerMove.airIdleLimitValue / GameRuleManager.instance.playerMove.maxAirIdleLimit * airIdleLimitSlider.maxValue;
		maxSpeedReachSlider.value = GameRuleManager.instance.playerMove.maxSpeedReachPercentage / GameRuleManager.instance.playerMove.maxSpeedReachStandard * maxSpeedReachSlider.maxValue;

		switch (eConfirmedInputMode) 
		{
			case INPUT_MODE.FREE_JOYSTICK:
				freeJoystickToggle.isOn = true;
				staticJoystickToggle.isOn = false;
				gamePadOverlayToggle.isOn = false;
			break;

			case INPUT_MODE.STATIC_JOYSTICK:
				freeJoystickToggle.isOn = false;
				staticJoystickToggle.isOn = true;
				gamePadOverlayToggle.isOn = false;
			break;

			case INPUT_MODE.GAMEPAD:
				freeJoystickToggle.isOn = false;
				staticJoystickToggle.isOn = false;
				gamePadOverlayToggle.isOn = true;

			break;
		}
	}

	public void SetRotateSpeed(float value)
	{
		if(GameRuleManager.instance == null) return;
		if(rotateSpdSlider == null) return;

		if(value != 0F)
		{
			rotateSpeed = value / rotateSpdSlider.maxValue * GameRuleManager.instance.playerMove.maxRotateSpeed;
		} else {
			rotateSpeed = 0F;
		}

		uiTxtRotateSpd.text = rotateSpeed.ToString();
	}

	public void SetRotateAngle(float value)
	{
		if(GameRuleManager.instance == null) return;
		if(rotateAngleSlider == null) return;
		
		if(value != 0F)
		{
			rotateAngle = value / rotateAngleSlider.maxValue * GameRuleManager.instance.playerMove.maxRotateDegree;
		} else {
			rotateAngle = 0F;
		}
		
		uiTxtRotateAngle.text = rotateAngle.ToString();
	}

	public void SetPlayerSpeed(float value)
	{
		if(GameRuleManager.instance == null) return;
		if(playerSpdSlider == null) return;
		
		if(value != 0F)
		{
			playerSpeed = value / playerSpdSlider.maxValue * GameRuleManager.instance.playerMove.maxSpeed;
		} else {
			playerSpeed = 0F;
		}

//		Debug.Log (playerSpeed);
		uiTxtPlayerSpd.text = playerSpeed.ToString();
	}

	public void SetIdleLimit(float value)
	{
		if(GameRuleManager.instance == null) return;
		if(idleLimitSlider == null) return;

		if(value != 0F)
		{
			idleLimit = value / idleLimitSlider.maxValue * GameRuleManager.instance.playerMove.maxIdleLimit;
		} else {
			idleLimit = 0F;
		}

		uiTxtIdleLimit.text = idleLimit.ToString();
	}

	public void SetAirIdleLimit(float value)
	{
		if(GameRuleManager.instance == null) return;
		if(airIdleLimitSlider == null) return;
		
		if(value != 0F)
		{
			airIdleLimit = value / airIdleLimitSlider.maxValue * GameRuleManager.instance.playerMove.maxAirIdleLimit;
		} else {
			airIdleLimit = 0F;
		}
		
		uiTxtAirIdleLimit.text = airIdleLimit.ToString();
	}

	public void SetMaxSpeedReach(float value)
	{
		if(GameRuleManager.instance == null) return;
		if(maxSpeedReachSlider == null) return;
		
		if(value != 0F)
		{
			maxSpeedReachPercentage = value / maxSpeedReachSlider.maxValue * GameRuleManager.instance.playerMove.maxSpeedReachStandard;
		} else {
			maxSpeedReachPercentage = 0F;
		}
		
		uiTxtMaxSpeedReach.text = maxSpeedReachPercentage.ToString();
	}

	public void FreeJoystickToggle(bool flag)
	{
		if (flag) 
		{
			freeJoystick = true;
			gamePadOverlay = false;
			eChoiceInputMode = INPUT_MODE.FREE_JOYSTICK;
		}
	}

	public void StaticJoystickToggle(bool flag)
	{
		if (flag) 
		{
			freeJoystick = false;
			gamePadOverlay = false;
			eChoiceInputMode = INPUT_MODE.STATIC_JOYSTICK;
		}
	}

	public void GamePadOverlayToggle(bool flag)
	{
		if (flag) 
		{
			gamePadOverlay = true;
			freeJoystick = false;
			eChoiceInputMode = INPUT_MODE.GAMEPAD;
		}
	}

	public void ConfirmOptionSettings()
	{
		GameRuleManager.instance.playerMove.speed = playerSpeed;
		GameRuleManager.instance.playerMove.rotateSpeed = rotateSpeed;
		GameRuleManager.instance.playerMove.minRotateDegree = rotateAngle;
		GameRuleManager.instance.playerMove.idleLimitValue = idleLimit;
		GameRuleManager.instance.playerMove.airIdleLimitValue = airIdleLimit;
		GameRuleManager.instance.playerMove.maxSpeedReachPercentage = maxSpeedReachPercentage;

		switch(eChoiceInputMode)
		{
		case INPUT_MODE.FREE_JOYSTICK:
			GameRuleManager.instance.playerMove.cnJoystick.SnapsToFinger = true;
			GameRuleManager.instance.playerMove.cnJoystick.TouchZoneSize = new Vector2(14F, 16F);

			UIManager.instance.optionPanel.gamePadOverlay = false;
			UIManager.instance.optionPanel.gamePadSys.ToggleGamePadSprites(false);

			break;
			
		case INPUT_MODE.STATIC_JOYSTICK:
			GameRuleManager.instance.playerMove.cnJoystick.SnapsToFinger = false;
			GameRuleManager.instance.playerMove.cnJoystick.TouchZoneSize = new Vector2(5F, 5F);

			UIManager.instance.optionPanel.gamePadOverlay = false;
			UIManager.instance.optionPanel.gamePadSys.ToggleGamePadSprites(false);
			break;
			
		case INPUT_MODE.GAMEPAD:
			GameRuleManager.instance.playerMove.cnJoystick.SnapsToFinger = false;
			GameRuleManager.instance.playerMove.cnJoystick.TouchZoneSize = new Vector2(5F, 5F);

			UIManager.instance.optionPanel.gamePadOverlay = true;
			UIManager.instance.optionPanel.gamePadSys.ToggleGamePadSprites(true);
			break;
		} 

		eConfirmedInputMode = eChoiceInputMode;

		PlayerPrefs.SetFloat("Speed", playerSpeed);
		PlayerPrefs.SetFloat("RotateSpeed", rotateSpeed);
		PlayerPrefs.SetFloat("RotateAngle", rotateAngle);
		PlayerPrefs.SetFloat("IdleLimit", idleLimit);
		PlayerPrefs.SetFloat("AirIdleLimit", airIdleLimit);
		PlayerPrefs.SetFloat("MaxSpeedReach", maxSpeedReachPercentage);

		PlayerPrefs.SetInt ("InputMode", (int)eConfirmedInputMode);
	}

	public void MuteAll(bool isMute)
	{
		if (FMODSoundManager.instance != null)
			FMODSoundManager.instance.SoundMute (isMute);
	}

	public void ShadowOn(bool isShadow)
	{
		GlobalProjectorManager shadow = GlobalProjectorManager.NoNullCheckGet ();

		if (shadow)
			shadow.enabled = isShadow;
			
	}
}
