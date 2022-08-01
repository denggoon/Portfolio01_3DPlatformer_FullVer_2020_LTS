using UnityEngine;
using System.Collections;

public class PlayerSkill : MonoBehaviour 
{
	public float skillDuration;
	public float skillTimer;

	public bool isActivated = false;
	// Use this for initialization
	public virtual void Start () 
	{
		skillTimer = skillDuration;
	}

	public virtual void Activate()
	{
		if(isActivated) return;

		StartCoroutine("StartTimer");
	}

	public IEnumerator StartTimer()
	{
		isActivated = true;
		skillTimer = skillDuration;

		while(skillTimer > 0F)
		{
			yield return null;
			skillTimer -= Time.deltaTime;

		}

		isActivated = false;
	}

	public virtual void CancelSkill()
	{
		StopCoroutine("StartTimer");

		skillTimer = 0F;
		isActivated = false;
	}
}
