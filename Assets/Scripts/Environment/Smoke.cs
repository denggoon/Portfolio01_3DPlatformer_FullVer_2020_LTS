using UnityEngine;
using System.Collections;

public class Smoke : Trap {

	public int maxLoop = 0;
	public float waitTime = 0.0f;
	public float showTime = 3.0f;
	public float hideTime = 5.0f;
	public bool isShow = false;
	
	private bool beforeTriggerFlag;
	private GameObject smokeObject;
	// Use this for initialization
	void Start () {
		smokeObject = this.transform.Find ("WhiteSmoke").gameObject;
		
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
	
	void isShowSmoke() {
		if (smokeObject != null) {
			smokeObject.SetActive(true);
		}
	}
	
	void isHideSmoke() {
		
		if (smokeObject != null) {
			smokeObject.SetActive(false);
		}
	}
	
	IEnumerator MainCoroutine() {
		
		if (isShow) {
			isShowSmoke();
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
			isShowSmoke ();
			yield return new WaitForSeconds (showTime);
			
			isHideSmoke ();
			yield return new WaitForSeconds (hideTime);
		}
		else {
			yield return null;
		}
	}
}
