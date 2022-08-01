using UnityEngine;
using System.Collections;

public class Pendulum : Trap {

	public bool gravityMode = false;

	public Rigidbody pivotRigidbody;
	public Rigidbody pendulumRigidbody;

	public float angle = 45F;
	public float speed = 1.5F;

	Quaternion qStart, qEnd;

	void Start()
	{
		pivotRigidbody = GetComponent<Rigidbody>();

		qStart = Quaternion.AngleAxis(angle, Vector3.right);
		qEnd = Quaternion.AngleAxis(-angle, Vector3.right);

		if(gravityMode)
		{
			this.transform.eulerAngles = new Vector3(angle, 0F, 0F);
		}
	}

	void FixedUpdate () 
	{
		if(GameRuleManager.instance.eGameStatus == E_GAME_STATUS.GAME_READY) return;
		if(GameRuleManager.instance.eGameStatus != E_GAME_STATUS.IN_PLAY) return;

		if(pivotRigidbody == null) return;
		if(pendulumRigidbody == null) return;
		if(isTriggered == false) return; 
		//트리거가 발동되지 않았으면 작동하지 않음. 

		if(gravityMode)
		{
			pivotRigidbody.isKinematic = false;
			pendulumRigidbody.useGravity = true;
			pendulumRigidbody.isKinematic = true;

			return;
		}

		pivotRigidbody.isKinematic = true;
		pendulumRigidbody.useGravity = false;
		pendulumRigidbody.isKinematic = true;

		transform.rotation = Quaternion.Lerp(qStart, qEnd, (Mathf.Sin (Time.time * speed) + 1.0F) * .5F);


	}
}
