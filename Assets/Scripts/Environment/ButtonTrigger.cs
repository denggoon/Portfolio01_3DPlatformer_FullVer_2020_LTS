using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonTrigger : Trap {

	public bool isDisable = false;
	public float wakeTime = 0F;

	public Transform[] turnOnTransforms;
	public Transform[] turnOffTransforms;

	private List<Trap> turnOnTraps = new List<Trap>();
	private List<Trap> turnOffTraps = new List<Trap>();

	private bool isStay = false;
	private bool beforeTriggerFlag;

	private BoxCollider buttonCollider = null;
	private Animator animator;

	void Start () {

		if (animator == null)
			animator =  this.transform.GetComponentInChildren<Animator> ();

		buttonCollider = this.GetComponent<BoxCollider>();

		if (isDisable == true) {
			buttonCollider.enabled = false;
			animator.SetBool ("isDisable", true);
			return;
		}
		beforeTriggerFlag = isTriggered;

		initFindTraps (true);
		initFindTraps (false);

//		for (int i=0; i<turnOnTraps.Count; i++) {
//			turnOnTraps[i].SwitchTrigger(false);
//		}

		if (isTriggered == false) {
			buttonCollider.enabled = false;
			disableButton ();
		}
	}

	void Update () {
		if (beforeTriggerFlag != isTriggered && isTriggered == true) {
			beforeTriggerFlag = isTriggered;
			buttonCollider.enabled = true;
			enableButton ();
		} else if (beforeTriggerFlag != isTriggered && isTriggered == false) {
			beforeTriggerFlag = isTriggered;
			buttonCollider.enabled = false;
			disableButton ();
		}
	}

	void initFindTraps(bool isOn) {
		Trap[] traps;
		int idx = 0;

		if (isOn == true) {
			for (int i=0; i<turnOnTransforms.Length; i++) {
			
				traps = turnOnTransforms[i].GetComponents<Trap> ();
				for (int j=0; j <traps.Length; j++) {
				
					turnOnTraps.Add(traps [j]);
					idx++;
				}
			}
		} else {
			for (int i=0; i<turnOffTransforms.Length; i++) {
				
				traps = turnOffTransforms[i].GetComponents<Trap> ();
				for (int j=0; j <traps.Length; j++) {

					turnOffTraps.Add(traps [j]);
					idx++;
				}
			}
		}
	}

	void OnTriggerEnter(Collider collider)
	{
		if (GameRuleManager.instance.eGameStatus != E_GAME_STATUS.IN_PLAY) 
			return;
		
		if (collider.gameObject.layer == LayerMask.NameToLayer ("Player")) { //플레이어인 경우
			bool jumpingDown = (GameRuleManager.instance.playerMove.fallDelta < 0 ? true : false);
			if (jumpingDown == true && isStay == false) {
//				Debug.Log("Jump");

				disableButton();
				isStay = true;

				// traps turn On
				for(int i=0; i<turnOnTraps.Count; i++)
				{
					turnOnTraps[i].SwitchTrigger(true);
				}

				// traps turn Off
				for(int i=0; i<turnOffTraps.Count; i++)
				{
					turnOffTraps[i].SwitchTrigger(false);
				}
			}
		}
	}

	void OnTriggerStay(Collider collider)
	{
		if (collider.gameObject.layer == LayerMask.NameToLayer ("Player")) { //플레이어인 경우
			isStay = true;
		}
	}

	void OnTriggerExit(Collider collider)
	{
		if (collider.gameObject.layer == LayerMask.NameToLayer ("Player")) { //플레이어인 경우
			isStay = false;
			if (wakeTime > 0) {
				Invoke("enableButton", wakeTime);
			}
		}
	}

	void enableButton() {
		if (isStay == false) {
			animator.SetBool ("isDefault", true);
		}
	}

	void disableButton(){
		animator.SetBool ("isDefault", false);
		animator.SetTrigger("isButtonClick");
	}
	
}
