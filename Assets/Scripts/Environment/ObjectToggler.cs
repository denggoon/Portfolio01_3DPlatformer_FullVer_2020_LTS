using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//선택한 객체들을 켜고 끄게 하는 스크립트 입니다. 
public class ObjectToggler : Trap {

	public List<GameObject> target = new List<GameObject>(); //켜지거나 꺼질 객체를 등록하는 변수 
	public bool isToggled = false; //최초 설정으로 꺼져있던가 켜져있도록 할수 있는 함수 입니다. 
	public bool activateOnToggle = true;

	public virtual void Start()
	{
		if(target == null) return;
		if(target.Count == 0) return;

		ToggleTarget(isToggled); //최초 설정 
	}

	public override void SwitchTrigger (bool flag)
	{
		if(target == null) return;

		isToggled = flag;

		ToggleTarget(flag);
	}

	public override void ToggleTrigger ()
	{
		if(target == null) return;
		
		isToggled = !isToggled;
		
		ToggleTarget(isToggled);
	}

	public virtual void ToggleTarget(bool flag) //설정한 객체들을 켜거나 끌수 있도록 하는 함수 
	{
		for (int i=0; i<target.Count; i++) 
		{
			Trap trap = target[i].GetComponent<Trap>();

			target [i].SetActive (flag);

			if(trap != null)
			{
				if(activateOnToggle)
				{
					if(flag)
					{
						trap.SwitchTrigger(activateOnToggle);
					} else {
						trap.SwitchTrigger(flag);
					}
				}
			}
		}
	}
}
