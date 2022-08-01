using UnityEngine;
using System.Collections;

public class KnockBackWall : MonoBehaviour {

	new public Transform transform;
	public float knockbackDist = 2.0F;

	void Awake()
	{
		transform = GetComponent<Transform>();
	}

	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.layer == LayerMask.NameToLayer("Player") == false) return;

		StartCoroutine(KnockBack(0.5F));
	}

	void Update()
	{
		Debug.DrawRay (this.transform.position, transform.TransformDirection (Vector3.forward), Color.magenta);
	}

	IEnumerator KnockBack(float time)
	{
		float velo = 0F;
		Vector3	dir = (GameRuleManager.instance.playerMove.transform.position - transform.position).normalized;

//		dir += Vector3.up * 2F;

		GameRuleManager.instance.playerMove.moveDirection = Vector3.zero;

		while (time > 0) 
		{
			float knockBackAmount = Mathf.SmoothDamp(knockbackDist, 0, ref velo, time);

			GameRuleManager.instance.playerMove.moveDirection += dir * knockBackAmount * Time.deltaTime;
			GameRuleManager.instance.playerMove.GetController ().Move(GameRuleManager.instance.playerMove.moveDirection * Time.deltaTime * GameRuleManager.instance.playerMove.speed);
			time -= Time.deltaTime;

			yield return null;
		}
	}
}
