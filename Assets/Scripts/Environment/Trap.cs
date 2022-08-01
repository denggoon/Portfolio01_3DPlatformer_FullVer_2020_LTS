using UnityEngine;
using System.Collections;


public class Trap : MonoBehaviour {

	new public Transform transform;
	private SoundEventCaller soundCaller;

	public virtual void Awake()
	{
		transform = GetComponent<Transform>();
		soundCaller = GetComponent<SoundEventCaller>();
		if (soundCaller == null) {
			soundCaller = gameObject.AddComponent<SoundEventCaller>();
		}
	}

	public bool isTriggered;

	public virtual void SwitchTrigger(bool flag)
	{
		if (isTriggered == flag)
			return;

		isTriggered = flag;
	}

	public virtual void ToggleTrigger()
	{
		isTriggered = !isTriggered;
	}

	public virtual void SoundTrigger ()
	{
		soundCaller.PlaySound (this.transform.position);
	}

}
