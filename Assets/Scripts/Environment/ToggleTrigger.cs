using UnityEngine;
using System.Collections;

//어떠한 조건을 통해 스위치가 켜지거나 꺼지도록 할수 있도록하는 범용 함수 입니다. 
public class ToggleTrigger : MonoBehaviour {

	public bool isTriggered = false;
	public Trap[] traps;

	public virtual void EnableTrigger()
	{
		if(traps == null) return;
		if(traps.Length < 1) return;

		for(int i=0; i<traps.Length; i++)
		{
			traps[i].SwitchTrigger(true);
		}

		isTriggered = true;
	}

	public virtual void DisableTrigger()
	{
		if(traps == null) return;
		if(traps.Length < 1) return;

		for(int i=0; i<traps.Length; i++)
		{
			traps[i].SwitchTrigger(false);
		}

		isTriggered = false;
	}
}
