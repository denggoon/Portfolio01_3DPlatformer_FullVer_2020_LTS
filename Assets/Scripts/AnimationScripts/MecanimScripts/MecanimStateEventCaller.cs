using UnityEngine;
using System.Collections;

public class MecanimStateEventCaller : StateMachineBehaviour {

	public string enterFuncName;
	public string enterArgsName;

	public string exitFuncName;
	public string exitArgsName;

	public string updateFuncName;
	public float eventFirePointInPercent;
	public string updateArgsName;

	public string moveFuncName;
	public string moveArgsName;

	public string ikFuncName;
	public string ikArgsName;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!string.IsNullOrEmpty (enterFuncName)) 
		{
			MecanimSendMessage(animator, enterFuncName, enterArgsName);
		}
	}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!string.IsNullOrEmpty (exitFuncName)) 
		{
			MecanimSendMessage(animator, exitFuncName, exitArgsName);
		}
	}

	public bool updateEventFired = false;
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!string.IsNullOrEmpty (updateFuncName)) 
		{
			if(eventFirePointInPercent <= 0) return;

			if(stateInfo.normalizedTime % 1.0F >= eventFirePointInPercent)
			{
				if(!updateEventFired)
				{
					MecanimSendMessage(animator, updateFuncName, updateArgsName);
					updateEventFired = true;
				}
			} else {

				if(updateEventFired)
				{
					updateEventFired = false;
				}
			}


		}
	}

//	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//	{
//		if (!string.IsNullOrEmpty (moveFuncName)) 
//		{
//			MecanimSendMessage(animator, moveFuncName, moveArgsName);
//		}
//	}
//
//	override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//	{
//		if (!string.IsNullOrEmpty (ikFuncName)) 
//		{
//			MecanimSendMessage(animator, ikFuncName, ikArgsName);
//		}
//	}

	void MecanimSendMessage(Animator animator, string funcName, string argsName)
	{
		if(string.IsNullOrEmpty(argsName))
		{
			animator.SendMessageUpwards (funcName, SendMessageOptions.DontRequireReceiver);
		} else {
			float result;
			if(float.TryParse(argsName, out result))
			{
				animator.SendMessageUpwards (funcName, result, SendMessageOptions.DontRequireReceiver);
			} else {
				animator.SendMessageUpwards (funcName, argsName, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
