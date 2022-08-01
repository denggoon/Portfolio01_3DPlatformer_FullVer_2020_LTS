using UnityEngine;
using System.Collections;

public class AnimIntervalPlay : MonoBehaviour {

	public float playInterval = 1.0F;
	public int loops = -1;
	public int loopCount = 0;

	public Animation anim;
	// Use this for initialization
	void Start () 
	{
		if(loops == 0) return;

		if(anim == null)
		{
			anim = GetComponent<Animation>();
		}

		if(anim == null) return;

		if(loops == -1)
		{
			loopCount = -2;
		}

		StartCoroutine(AnimationLoop());
	}
	
	IEnumerator AnimationLoop () 
	{
		while(loopCount < loops)
		{
			if(loops != -1)
				loopCount ++;

			anim.Play();
			yield return new WaitForSeconds(anim.clip.length);
			yield return new WaitForSeconds(playInterval);
		}
	}
}
