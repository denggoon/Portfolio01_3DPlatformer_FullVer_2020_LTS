using UnityEngine;
using System.Collections;

public class GamePad : MonoBehaviour {

	public SpriteRenderer[] gamePads;
	public SpriteRenderer[] joystick;
	public SpriteRenderer gamePadSample;
	public SpriteRenderer gamePadCenter;

	Vector2 inputVector;

	void Start()
	{
		gamePads = GetComponentsInChildren<SpriteRenderer> ();

		for (int i=0; i<gamePads.Length; i++) 
		{
			if(!gamePads[i].name.Contains("_C"))
			{
				if(gamePadSample == null)
				{
					gamePadSample = gamePads[i];
				}
//				GamePadButtons gpBtns = gamePads[i].gameObject.AddComponent<GamePadButtons>();
				gamePads[i].gameObject.AddComponent<GamePadButtons>();
//				gpBtns.SetGamePadSystem(this);
			} else {
				gamePadCenter = gamePads[i];
			}
		}

		if (((INPUT_MODE)UIManager.instance.optionPanel.eConfirmedInputMode) != INPUT_MODE.GAMEPAD) {
			ToggleGamePadSprites (false);
		}
	}

	public void ToggleGamePadSprites(bool flag)
	{
		for(int i=0; i<gamePads.Length; i++)
		{
			gamePads[i].enabled = flag;
		}

		if (GameRuleManager.instance != null && GameRuleManager.instance.playerMove != null) 
		{
			GameRuleManager.instance.playerMove.cnJoystick.ToggleJoystickSprites (!flag);
		}
	}
}
