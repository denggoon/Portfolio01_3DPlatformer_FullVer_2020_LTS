using UnityEngine;
using System.Collections;

public class PooledFx : PooledObj {

	ParticleSystem pSys;

	public override void Awake()
	{
		base.Awake ();
		pSys = GetComponent<ParticleSystem> ();
	}

	void OnEnable ()
	{
		StartCoroutine (OnEnableCoroutine ());
	}

	public IEnumerator OnEnableCoroutine()
	{
		pSys.Play ();
		
		yield return new WaitForSeconds (pSys.main.duration);
		
		WhenTaskDone (); 
	}
}
