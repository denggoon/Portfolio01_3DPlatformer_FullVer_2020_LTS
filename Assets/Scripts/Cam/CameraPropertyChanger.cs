using UnityEngine;
using System.Collections;

public class CameraPropertyChanger : CameraEvent {

	public Vector3 cameraAngle;
	public Vector3 distancePos;

	public float changeTime;
	private float timer;

	private Vector3 initialAngle;
	private Vector3 initialDist;

	public override void SwitchTrigger (bool flag)
	{
		base.SwitchTrigger (flag);

		if (flag) 
		{
			if (changeTime > 0) {
				initialAngle = GameRuleManager.instance.sideCam.transform.eulerAngles;
				initialDist = GameRuleManager.instance.sideCam.distancePos; 
			} else {
				GameRuleManager.instance.sideCam.SetCameraEulerAngle (cameraAngle);
				GameRuleManager.instance.sideCam.SetCameraDistance (distancePos);
				GameRuleManager.instance.sideCam.ForceToCalibration ();
			}
		} else {
			timer = 0F;
		}
	}

	public override void Update ()
	{
		base.Update ();

		if(GameRuleManager.instance.eGameStatus != E_GAME_STATUS.IN_PLAY) return;
		
		if (cameraTrans == null)
			return;
		
		if(isTriggered == false) return; 

		if (changeTime > 0) 
		{
			if (timer <= changeTime) 
			{
				GameRuleManager.instance.sideCam.SetCameraEulerAngle (Vector3.Lerp (initialAngle, cameraAngle, timer / changeTime));
				GameRuleManager.instance.sideCam.SetCameraDistance (Vector3.Lerp (initialDist, distancePos, timer / changeTime));

				timer += Time.deltaTime;
			} else {

				GameRuleManager.instance.sideCam.SetCameraEulerAngle (cameraAngle);
				GameRuleManager.instance.sideCam.SetCameraDistance (distancePos);
//				GameRuleManager.instance.sideCam.ForceToCalibration ();
				SwitchTrigger (false);
			}
		}
	}
}
