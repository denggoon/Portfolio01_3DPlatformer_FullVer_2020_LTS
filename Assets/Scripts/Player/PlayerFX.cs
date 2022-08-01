using UnityEngine;
using System.Collections;

public class PlayerFX : MonoBehaviour {

	private Animator animator;

	public GameObject commonFxObj;
	public string commonFxName;

	public string fxKnockName;

	public string pfxMoveName;
	public ParticleSystem pfxMoveDefault;
	public ParticleSystem pfxMoveCloud;
	private ParticleSystem pfxMove;

	public string trailAirName;
	private TrailRenderer trailAir;

	public string trailSpecAirName;
	private TrailRenderer trailSpecAir;
	
	public ParticleSystem pfxMagnet;

	void Awake()
	{
		if (string.IsNullOrEmpty (commonFxName))
			commonFxName = "PC_Fx";

		if(animator == null)
			animator = GetComponent<Animator> ();
		
		if (animator == null)
			animator = this.transform.GetComponentInChildren<Animator> ();

		if (animator != null) 
		{
			if(!string.IsNullOrEmpty(commonFxName))
			{
				commonFxObj = animator.transform.Find(commonFxName).gameObject;
			}

			if(!string.IsNullOrEmpty(trailAirName))
			{
				Transform trailAirTrans = animator.transform.Find (trailAirName);

				if(trailAirTrans == null)
				{
					trailAir = animator.transform.Find (commonFxName + "/"  + trailAirName).GetComponent<TrailRenderer> ();
				} else {
					trailAir = animator.transform.Find (trailAirName).GetComponent<TrailRenderer> ();
				}

				if (trailAir != null)
					trailAir.enabled = false;

			}

			if(!string.IsNullOrEmpty(trailSpecAirName))
			{
				Transform trailSpecTrans = animator.transform.Find (trailSpecAirName);

				if(trailSpecTrans == null)
				{
					trailSpecAir = animator.transform.Find (commonFxName + "/"  + trailSpecAirName).GetComponent<TrailRenderer> ();
				} else {
					trailSpecAir = animator.transform.Find (trailSpecAirName).GetComponent<TrailRenderer> ();
				}
			
				if (trailSpecAir != null)
					trailSpecAir.enabled = false;
			}

			if(!string.IsNullOrEmpty(pfxMoveName))
			{
				Transform pfxMoveTrans = animator.transform.Find(pfxMoveName);

				if(pfxMoveTrans == null)
				{
					pfxMove = animator.transform.Find(commonFxName + "/" + pfxMoveName).GetComponent<ParticleSystem>();
				} else {
					pfxMove = animator.transform.Find(pfxMoveName).GetComponent<ParticleSystem>();
				}
			}

		}
	}

	public void ToggleCommonFx(bool flag)
	{
		if (commonFxObj != null) 
		{
			commonFxObj.SetActive(flag);
		}
	}

	public void ToggleAirTrail(bool flag)
	{
		if (trailAir == null)
			return;
		
		trailAir.enabled = flag;
	}

	public void ToggleOnAirTrail()
	{
		ToggleAirTrail (true);
	}

	public void ToggleOffAirTrail()
	{
		ToggleAirTrail (false);
	}

	public void ToggleSpecAirTrail(bool flag)
	{
		if (trailSpecAir == null)
			return;
		
		trailSpecAir.enabled = flag;
	}

	public void ToggleOnSpecAirTrail()
	{
		ToggleSpecAirTrail (true);
	}
	
	public void ToggleOffSpecAirTrail()
	{
		ToggleSpecAirTrail (false);
	}

	public void ToggleAllTrail(bool flag)
	{
		ToggleAirTrail (flag);
		ToggleSpecAirTrail (flag);
	}

	public void ToggleOffAllTrail()
	{
		ToggleAllTrail (false);
	}

	public void PlayKnockFX()
	{
		if(!string.IsNullOrEmpty(fxKnockName))
			PopEffect (fxKnockName);
	}

	public void PopEffect(string fxName)
	{
		ResourcesManager.instance.PopEffect (fxName, this.transform.position);
	}

	public void ToggleOnMoveFX(string groundName = "NONE")
	{
		if (groundName.Contains ("Cloud")) {
			pfxMove = pfxMoveCloud;
		} else {
			pfxMove = pfxMoveDefault;
		}

		PlayMoveFX (true);
	}

	public void ToggleOffMoveFX()
	{
		PlayMoveFX (false);
	}

	public void PlayMoveFX(bool flag)
	{
		if (pfxMove == null)
			return;

		if (flag) 
		{
			if(!pfxMove.gameObject.activeSelf)
			{
				pfxMove.gameObject.SetActive(true);
				pfxMove.Play ();
			}
		} else {

			if(pfxMoveDefault.gameObject.activeSelf)
			{
				pfxMoveDefault.Stop();
				pfxMoveDefault.gameObject.SetActive(false);

			} else if(pfxMoveCloud.gameObject.activeSelf) 
			{
				pfxMoveCloud.Stop();
				pfxMoveCloud.gameObject.SetActive(false);
			}
		}
	}

	public void ToggleOnMagnetFX() {
		if (pfxMagnet != null) {
			pfxMagnet.gameObject.SetActive(true);
			pfxMagnet.Play();
		}
	}

	public void ToggleOffMagnetFX() {
		if (pfxMagnet != null) {
			pfxMagnet.Stop();
			pfxMagnet.gameObject.SetActive(false);
		}
	}
}
