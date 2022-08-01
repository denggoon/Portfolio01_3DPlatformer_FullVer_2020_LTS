using UnityEngine;
using System.Collections;

public class AnimatorEventCaller : MonoBehaviour {

	private new Transform transform;
	public GameObject parentObj;
	public GameObject fxActionObj;

	public string dedicatedFuncName;

	void Awake()
	{
		transform = GetComponent<Transform> ();
		parentObj = transform.parent.gameObject;
	}

	public void CallEventName(string funcName, string argsName)
	{
		if (string.IsNullOrEmpty (argsName)) {
			parentObj.SendMessage (funcName, SendMessageOptions.DontRequireReceiver);
		} else {
			parentObj.SendMessage (funcName, argsName, SendMessageOptions.DontRequireReceiver);
		}
	}

	public void CallDedicatedEvent(string args=null)
	{
		if (string.IsNullOrEmpty (dedicatedFuncName))
			return;

		if (string.IsNullOrEmpty (args)) 
		{
			parentObj.SendMessage (this.dedicatedFuncName, SendMessageOptions.DontRequireReceiver);
		} else {
			parentObj.SendMessage (this.dedicatedFuncName, args, SendMessageOptions.DontRequireReceiver);
		}
	}

	public void SetDedicatedEvent(string funcName)
	{
		this.dedicatedFuncName = funcName;
	}

	#region PlayerSound Functions

	public void PlayOneShot(string soundID)
	{
		if (SoundBoard.instance != null)
			SoundBoard.instance.PlayFromSoundBoard (soundID, this.transform.position);
	}

	#endregion

	#region PlayerFX Functions

	public void PopEffect(string fxName)
	{
		if(ResourcesManager.instance != null)
			ResourcesManager.instance.PopEffect(fxName, this.transform.position);
	}

	public void AttachEffect(string fxName)
	{
		if(ResourcesManager.instance != null)
			ResourcesManager.instance.AttachEffect(fxName, (fxActionObj != null ? fxActionObj.transform : this.transform));
	}

	#endregion
}
