using UnityEngine;
using System.Collections;

public class SpinBox : Trap {
	
	public float waitTime = 3.0f;
	public bool isClockWise = true;
	public float rotateSpeed = 1f;
	public float spinAngle = 30.0f;
	public int maxLoop = 0;
	public float stoppingDist = 1.0f;
	public int pingPongPoint = 0;
		
	private bool isReversPoint = false;
	private int reversCnt = 0;
	private float movingAngle = 0.0f;
	private float rotateMaxSpeed = 100.0f;
	private Transform parentTransform;
	private bool beforeTriggerFlag;

	// Use this for initialization
	void Start () {
		parentTransform = this.transform.parent;
		beforeTriggerFlag = isTriggered;
		reversCnt = pingPongPoint;

//		int numChildren = parentTransform.childCount;
//		for (int i = 0; i < numChildren; ++i) {
//			Collider collider = parentTransform.GetChild(i).GetComponentInChildren<Collider> ();
//			if (collider != null) 
//			{
//				collider.gameObject.tag = "MovingPlatform";
//			}
//		}

		if (isTriggered == false) {
			//트리거가 발동되지 않았으면 작동하지 않음. 
			return;
		}

		StartCoroutine (MainCoroutine ());
	}
	
	// Update is called once per frame
	void Update () {

		if (beforeTriggerFlag != isTriggered && isTriggered == true) {
			beforeTriggerFlag = isTriggered;

			StartCoroutine (MainCoroutine ());

		} else if (beforeTriggerFlag != isTriggered && isTriggered == false) {
			beforeTriggerFlag = isTriggered;

		}
	}

	bool isReversCheck() {

		bool isReturnFlag = false;
		reversCnt--;

		if (pingPongPoint > 0) {
			
			if (isReversPoint) {
				isReturnFlag = true;
			} else {
				isReturnFlag = false;
			}
			
			if (reversCnt == 0) {
				reversCnt = pingPongPoint;
				isReversPoint = !isReversPoint;
			}
			
		} else {
			isReturnFlag = false;
		}

		return isReturnFlag;
	}
	
	IEnumerator MainCoroutine() {
		
		yield return new WaitForSeconds (waitTime);

		if (maxLoop == 0) {
			while (true) {
				bool isRevers = isReversCheck();
				if (isRevers) {
					yield return StartCoroutine (PatternCoroutine (!isClockWise));
				} else {
					yield return StartCoroutine (PatternCoroutine (isClockWise));
				}
			}
		} else {

			for (int i = maxLoop; i > 0; i--) {
				maxLoop--;
				bool isRevers = isReversCheck();
				if (isRevers) {
					yield return StartCoroutine (PatternCoroutine (!isClockWise));
				} else {
					yield return StartCoroutine (PatternCoroutine (isClockWise));
				}

			}

			yield return new WaitForEndOfFrame ();

		}
	}

	IEnumerator PatternCoroutine(bool isReverse) {

		Vector3 newAngle = parentTransform.localEulerAngles + new Vector3 ( 0.0f, (isReverse == true ? spinAngle : -spinAngle), 0.0f );

		movingAngle = 0.0f;
		float overTime = 0.0f;
		float speed = (Time.deltaTime * (rotateSpeed > rotateMaxSpeed ? rotateMaxSpeed : rotateSpeed) * 10);

		while (true) {
			overTime += Time.deltaTime;
			movingAngle += speed;

			if (isReverse == true) {
				parentTransform.Rotate (0, (speed), 0);
			} else {
				parentTransform.Rotate (0, -(speed), 0);
			}

			if (spinAngle - movingAngle < 1.0f || overTime > 60.0f) {
				parentTransform.rotation = Quaternion.Euler (newAngle);
				movingAngle = spinAngle;
				break;
			}

			yield return new WaitForEndOfFrame();
		}

		if (isTriggered == false) {
			StopAllCoroutines();
			yield return null;
		}

		yield return new WaitForSeconds (stoppingDist);
	}
}
