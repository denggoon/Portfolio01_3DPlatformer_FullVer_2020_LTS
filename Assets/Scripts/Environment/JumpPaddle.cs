using UnityEngine;
using System.Collections;

public class JumpPaddle : Trap {

	public Collider trigger;

	public float bounceHeight = 2F;
	public float forwardForce = 2F;

	public override void Awake()
	{
		base.Awake();
		trigger = GetComponent<Collider> ();
	}

	void OnTriggerEnter(Collider collider)
	{
		if (isTriggered == false)
			return;

		if(collider.gameObject.layer == LayerMask.NameToLayer("Player") == false) return;
		Bounce();
	}

	void Bounce()
	{
		if(GameRuleManager.instance.playerMove.isKnockBack == true) return;

		Vector3 direction = transform.TransformDirection (Vector3.forward * forwardForce);

		direction.y = bounceHeight;

		GameRuleManager.instance.playerMove.Jump (true, direction.y, direction.x, direction.z);

		if (SoundBoard.instance != null)
			SoundBoard.instance.PlayFromSoundBoard ("SND_GMK_009_PLAY", this.transform.position);
	}
}
