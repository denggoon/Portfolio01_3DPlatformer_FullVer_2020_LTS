using UnityEngine;
using System.Collections;

public class PatternBoard : Trap {

	public int maxLoop = 0;
	public float waitTime = 0.0f;
	public bool isShow = false;
	public float aTime = 2.0f;
	public float bTime = 2.0f;

	public float speed = 5.0f;
	private Transform posTrans;
	private bool beforeTriggerFlag;

	// Use this for initialization
	void Start () {
		posTrans = this.transform.Find ("axisPosition"); 

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

	IEnumerator MainCoroutine() {

		yield return new WaitForSeconds (waitTime);

		if (maxLoop == 0) {

			bool isReverse = isShow;
			if(isReverse == true) {
				yield return StartCoroutine (PatternCoroutine (false));
				yield return new WaitForSeconds (bTime);
			}

			while (true) {

				yield return StartCoroutine (PatternCoroutine (isReverse));

				if (isReverse == true) {
					isReverse = false;
					yield return new WaitForSeconds (aTime);
				} else {
					isReverse = true;
					yield return new WaitForSeconds (bTime);
				}
			}
		} else {

			bool isReverse = isShow;
			if(isReverse == true) {
				yield return StartCoroutine (PatternCoroutine (false));
				yield return new WaitForSeconds (bTime);
			}

			for (int i = maxLoop; i > 0; i--) {
				maxLoop--;

				yield return StartCoroutine (PatternCoroutine (isReverse));

				if (isReverse == true) {
					isReverse = false;
					yield return new WaitForSeconds (aTime);
				} else {
					isReverse = true;
					yield return new WaitForSeconds (bTime);
				}
			}
		}

		yield return null;
	}

	IEnumerator PatternCoroutine(bool isReverse) {

		if (isTriggered == true && (GameRuleManager.instance.eGameStatus == E_GAME_STATUS.IN_PLAY || GameRuleManager.instance.eGameStatus == E_GAME_STATUS.GAME_READY) ) {

			this.SoundTrigger();

			int angle = 0;
			while (angle < 180) {
				if ( isReverse == true ) {
					
					this.transform.RotateAround(posTrans.transform.position, this.transform.TransformDirection(Vector3.right), speed);
				} else {
					this.transform.RotateAround(posTrans.transform.position, this.transform.TransformDirection(Vector3.left), speed);
				}
				angle += (int)(speed);
				
				yield return new WaitForEndOfFrame();
			}

		}

		yield return null;
	}
}
