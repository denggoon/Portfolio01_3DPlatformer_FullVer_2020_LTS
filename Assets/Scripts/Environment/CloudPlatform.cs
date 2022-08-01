using UnityEngine;
using System.Collections;

public class CloudPlatform : Trap {

	public float enduranceTime;
	public float fallSpeed;
	public float fallLength;

	private bool isfallDown = false;
	private float fallDownLength = 0.0f;
	private Collider _collider;
	private Vector3 defaultPosition;

	// Use this for initialization
	void Start () {
		_collider = transform.GetChild(0).GetComponentInChildren<Collider> ();
		defaultPosition = _collider.transform.position;
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerStay(Collider collider)
	{
		if (collider.gameObject.layer == LayerMask.NameToLayer ("Player")) { //플레이어인 경우

			if (isfallDown == true)
				return;
			
			if (isTriggered == false)
				return;

			GameRuleManager.instance.playerMove.groundName = this.gameObject.tag;
			isfallDown = true;
			StartCoroutine ("PatternCoroutine");
		}
	}

	void OnTriggerExit(Collider collider) 
	{
		if (collider.gameObject.layer == LayerMask.NameToLayer ("Player")) { //플레이어인 경우
			GameRuleManager.instance.playerMove.groundName = string.Empty;
			endAction ();
		}
	}

	void endAction()
	{
		isfallDown = false;
		fallDownLength = 0.0f;
		_collider.transform.position = defaultPosition;
		StopCoroutine ("PatternCoroutine");
	}

	IEnumerator PatternCoroutine() {

		yield return new WaitForSeconds (enduranceTime);
	
		while (true) {
			if (fallLength < fallDownLength){
				_collider.isTrigger = true;
				break;
			}

			fallDownLength += Time.deltaTime * 0.1f * fallSpeed;
			_collider.transform.Translate(new Vector3(0.0f, -0.1f, 0.0f) * Time.deltaTime * fallSpeed);
			yield return new WaitForEndOfFrame ();
		}

		if (isTriggered == false) {
			endAction();
			yield return null;
		}

		yield return null;
	}


}
