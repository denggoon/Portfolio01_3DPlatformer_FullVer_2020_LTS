using UnityEngine;
using System.Collections;

public class PlayerAnimEventCaller : AnimatorEventCaller 
{
	#region PlayerSound Functions

	public void FootSound()
	{
		parentObj.SendMessage ("FootSound", SendMessageOptions.DontRequireReceiver);
	}

	#endregion

	#region PlayerFX Functions

	public void ToggleCommonFx(int intFlag)
	{
		Debug.Log ("ToggleCommonFX");

		bool flag = false;

		if (intFlag == 1)
			flag = true;

		parentObj.SendMessage ("ToggleCommonFx", flag, SendMessageOptions.DontRequireReceiver);
	}

	public void PlayMoveFX(int intFlag)
	{
		bool flag = false;
		
		if (intFlag == 1)
			flag = true;

		parentObj.SendMessage ("PlayMoveFX", flag, SendMessageOptions.DontRequireReceiver);
	}

	public void ToggleAirTrail(int intFlag)
	{
		bool flag = false;
		
		if (intFlag == 1)
			flag = true;

		parentObj.SendMessage ("ToggleAirTrail", flag, SendMessageOptions.DontRequireReceiver);
	}

	public void ToggleSpecAirTrail(int intFlag)
	{
		bool flag = false;
		
		if (intFlag == 1)
			flag = true;

		parentObj.SendMessage ("ToggleSpecAirTrail", flag, SendMessageOptions.DontRequireReceiver);
	}

	public void ToggleAllTrail(int intFlag)
	{
		bool flag = false;
		
		if (intFlag == 1)
			flag = true;

		parentObj.SendMessage ("ToggleAllTrail", flag, SendMessageOptions.DontRequireReceiver);
	}

	#endregion
}
