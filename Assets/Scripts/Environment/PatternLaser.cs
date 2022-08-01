using UnityEngine;
using System.Collections;

public class PatternLaser : Trap {

	public int maxLoop = 0;
	public float waitTime = 0.0f;
	public float showTime = 3.0f;
	public float hideTime = 5.0f;
	public bool isShow = false;
	public float damage = 1.0f;
	public float knockbackDist = 3.0f;

	private bool beforeTriggerFlag;
	private GameObject laserBarObject;
	private CommonSpine spine;

	// Use this for initialization
	void Start () {
		laserBarObject = this.transform.Find ("LaserBar").gameObject;
		spine = (laserBarObject != null ? laserBarObject.transform.GetComponent<CommonSpine> () : null);

		if (spine != null) {
			spine.damage = damage;
			spine.knockbackDist = knockbackDist;
		}

		beforeTriggerFlag = isTriggered;

		if (isTriggered != false) {
			StartCoroutine (MainCoroutine ());
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (beforeTriggerFlag != isTriggered && isTriggered == true) {
			beforeTriggerFlag = isTriggered;
			
			StartCoroutine (MainCoroutine ());
			
		} else if (beforeTriggerFlag != isTriggered && isTriggered == false) {
			beforeTriggerFlag = isTriggered;

			StopAllCoroutines();
		}
	}

	void isShowLaserBar() {
		if (laserBarObject != null) {
			this.SoundTrigger();
			laserBarObject.SetActive(true);
		}
	}

	void isHideLaserBar() {

		if (laserBarObject != null) {
			laserBarObject.SetActive(false);
		}
	}

	IEnumerator MainCoroutine() {

		if (isShow) {
			isShowLaserBar();
		}
		yield return new WaitForSeconds (waitTime);
		
		if (maxLoop == 0) {
			while (true) {
				yield return StartCoroutine (PatternCoroutine ());
			}
		} else {
			for (int i = maxLoop; i > 0; i--) {
				maxLoop--;
				yield return StartCoroutine (PatternCoroutine ());
			}
		}

		yield return null;
	}

	IEnumerator PatternCoroutine() {

		if (isTriggered == true && GameRuleManager.instance.eGameStatus == E_GAME_STATUS.IN_PLAY) {
			isShowLaserBar ();
			yield return new WaitForSeconds (showTime);

			isHideLaserBar ();
			yield return new WaitForSeconds (hideTime);
		}
		else {
			yield return null;
		}
	}
}
