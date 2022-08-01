using UnityEngine;
using System.Collections;

public class TrapTrigger : ToggleTrigger {

	public bool disableOnEnter = false;
	public bool doReverseOnExit = false;
	public bool isInstant = false;
	private bool doneAction = false;

	public float delayTime = 0F;

	IEnumerator OnTriggerEnter(Collider collider) //트랩을 밟으면 트리거가 작동하도록 하는 함수 
	{
		if (isInstant && doneAction)
			yield break;

		if (collider.gameObject.CompareTag ("Player") == false)
			yield break;

//		Debug.LogError("Enter: " + collider.gameObject.name);

		yield return new WaitForSeconds (delayTime);

		if (disableOnEnter) 
		{
			DisableTrigger();
		} else {
			EnableTrigger ();
		}

		if (isInstant)
			doneAction = true;
	}

	IEnumerator OnTriggerExit(Collider collider)
	{
		if (isInstant && doneAction)
			yield break;

		if (!doReverseOnExit)
			yield break;

		if(collider.gameObject.tag != "Player") yield break;

		yield return new WaitForSeconds (delayTime);

		if (disableOnEnter) 
		{
			EnableTrigger ();
		} else {
			DisableTrigger();

		}
	}
}
