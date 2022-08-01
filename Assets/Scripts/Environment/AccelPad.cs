using UnityEngine;
using System.Collections;

public class AccelPad : MonoBehaviour {
	
	private Animator animator;
	new public Transform transform;

	public float speed = 5.0F;
	public float duration = 2.5F;

	void Awake()
	{
		transform = GetComponent<Transform>();
		animator = this.transform.GetComponentInChildren<Animator> ();
	}
	
	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.layer == LayerMask.NameToLayer("Player") == false) 
			return;

		Acceleration();
	}

	void AnimationOn () {
		if(animator != null)
		{
			float length = animator.GetCurrentAnimatorStateInfo(0).length;
			Invoke("AnimationOff", length);
		}
	}

	void AnimationOff () {
		if(animator != null)
		{
			animator.SetTrigger("OffTrigger");
		}
	}

	void Acceleration()
	{
		if(GameRuleManager.instance.playerMove.isKnockBack == true) return;

		animator.SetTrigger("OnTrigger");
		Invoke("AnimationOn", 0.05f);
//		Vector3 direction = transform.TransformDirection (Vector3.forward);
		GameRuleManager.instance.playerMove.Accelerate(speed ,duration);

	}
}
