using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//선택한 객체들을 켜고 끄게 하는 스크립트 입니다. 
public class TimerBlock : Trap {

	private Collider[] colliderList; //켜지거나 꺼질 객체를 등록하는 변수 
	private List<GameObject> gameobjectList = new List<GameObject>(); //켜지거나 꺼질 객체를 등록하는 변수 

	public bool isShow = false;

	public float showTime = 1F;
	public float hideStartRate = .8F;
	public float hideTime = 1F;

	public float blinkInterval = .1F;
	private float blinkTimer;

	private float timer;

	public void Start ()
	{
		if (this.transform.childCount > 0) 
		{
			gameobjectList.Add(this.transform.GetChild(0).Find("Block").gameObject);
			colliderList = this.transform.GetChild(0).GetComponentsInChildren<Collider>();
		}

		ToggleTarget (isShow);

	}

	void ToggleTarget(bool flag)
	{
		isShow = flag;
		for(int i=0; i<colliderList.Length; i++)
		{
			colliderList[i].enabled = isShow;
		}

		ShowTarget (flag);
	}

	void ShowTarget(bool flag)
	{
		if (flag) 
		{
			if(SoundBoard.instance != null)
			{
				
				SoundBoard.instance.PlayFromSoundBoard("SND_GMK_007_ON", this.transform.position);
			}

		} else {

			if(SoundBoard.instance != null)
			{
				SoundBoard.instance.PlayFromSoundBoard("SND_GMK_007_OFF", this.transform.position);
			}
		}

		for(int i=0; i<gameobjectList.Count; i++)
		{
			gameobjectList[i].SetActive(isShow);
		}
	}

	void ToggleShow()
	{
		for(int i=0; i<gameobjectList.Count; i++)
		{
			gameobjectList[i].SetActive(!gameobjectList[i].activeSelf);
		}
	}

	void Update()
	{
		if(GameRuleManager.instance.eGameStatus == E_GAME_STATUS.GAME_READY) return;
		if(GameRuleManager.instance.eGameStatus != E_GAME_STATUS.IN_PLAY) return;

		if (isTriggered == false)
			return;

		timer += Time.deltaTime;

		if (isShow) 
		{
			if(timer > showTime * hideStartRate && timer < showTime)
			{
				blinkTimer += Time.deltaTime;

				if(blinkTimer > blinkInterval)
				{
					blinkTimer = 0F;
					ToggleShow();
				}
			}

			if (timer > showTime) 
			{
				timer = 0F;
				ToggleTarget (false);
			}

		} else {

			if( timer > hideTime)
			{
				timer = 0F;
				ToggleTarget(true);
			}
		}
	}
}
