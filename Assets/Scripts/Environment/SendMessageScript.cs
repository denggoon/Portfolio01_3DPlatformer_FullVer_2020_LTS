using UnityEngine;
using System.Collections;

public class SendMessageScript : MonoBehaviour {

	public GameObject targetObj;
	public string funcName;
	public string param;

	public void SendMessage()
	{
		if (targetObj == null)
			return;

		if (funcName == null || funcName == "" || string.IsNullOrEmpty (funcName))
			return;

		if (param == null || param == "" || string.IsNullOrEmpty (param)) 
		{
			targetObj.SendMessage (funcName, SendMessageOptions.DontRequireReceiver);
		} else {
			targetObj.SendMessage (funcName, param, SendMessageOptions.DontRequireReceiver);
		}
	}
}
