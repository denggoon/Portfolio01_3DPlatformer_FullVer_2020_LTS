using UnityEngine;
using System.Collections;
using System;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(CommonSpine))]
public class CommonSpineEditor : Editor
{
	string[] _choices = new [] {"Normal", "Pattern"};

	public override void OnInspectorGUI()
	{
		CommonSpine spine = target as CommonSpine;
		
		spine.type = EditorGUILayout.Popup("type", spine.type, _choices);
		spine.isTriggered = EditorGUILayout.Toggle("IsTriggered", spine.isTriggered);
		spine.damage = EditorGUILayout.FloatField("damage", spine.damage);
		spine.knockbackDist = EditorGUILayout.FloatField("knockbackDist", spine.knockbackDist);
//		spine.forceDir = EditorGUILayout.Vector3Field("forceDir", spine.forceDir);

		switch (spine.type) {
		case 0:
			break;
			
		case 1:
			spine.maxLoop = EditorGUILayout.IntField("maxLoop", spine.maxLoop);
			spine.waitTime = EditorGUILayout.FloatField("waitTime", spine.waitTime);
			spine.showTime = EditorGUILayout.FloatField("showTime", spine.showTime);
			spine.startRate = EditorGUILayout.FloatField("startRate", spine.startRate);
			spine.hideTime = EditorGUILayout.FloatField("hideTime", spine.hideTime);
			spine.isShow = EditorGUILayout.Toggle("isShow", spine.isShow);

			break;
			
		default:
			break;
		}

		if (GUI.changed)
			EditorUtility.SetDirty(spine);
	}
}
#endif

public enum SPINE_TYPE
{
	NORMAL = 0,
	PATTERN = 1,
}

public class CommonSpine : Trap {

	public int type = (int)SPINE_TYPE.NORMAL;
	public float damage = 1.0f; 			//스파이크가 주는 데미지 
	public float knockbackDist = 3.0f; 	//스파이크와 부딪혔을때 넉백 크기 
	public int maxLoop = 0;				//스파이크가 설정된 행동을 반복하는 횟수 - 0: 무한 

	public float waitTime = 2.0f;
	public float showTime = 3.0f;
	public float startRate = 3.0f;
	public float hideTime = 5.0f;

	public bool isShow = false;
	private Vector3 forceDir = Vector3.zero;

	private BoxCollider damageCollider = null;
	private float elapsedTime = 0.0f;
	private bool beforeTriggerFlag;

	// Use this for initialization
	void Start () {
		damageCollider = GetComponent<BoxCollider>();
		beforeTriggerFlag = isTriggered;
		if (isTriggered == false) {
			//트리거가 발동되지 않았으면 작동하지 않음. 
			if (type == (int)SPINE_TYPE.PATTERN && isShow != true) {
				this.transform.localPosition = Vector3.down;
			}

			return;
		}

		if (type == (int)SPINE_TYPE.PATTERN) {

//			long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
//			Debug.Log(">> " + this.name + " " + milliseconds);
			StartCoroutine (MainCoroutine ());
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (beforeTriggerFlag != isTriggered && isTriggered == true) {
			beforeTriggerFlag = isTriggered;
			if (type == (int)SPINE_TYPE.PATTERN) {
				StartCoroutine (MainCoroutine ());
			}
		} 
	}


	IEnumerator MainCoroutine() {

		if (isShow == false) {
			this.transform.localPosition = Vector3.down;
		}

		yield return new WaitForSeconds (waitTime);

		if (maxLoop == 0) {
			while (true)
				yield return StartCoroutine (PatternCoroutine ());
		} else {
			for (int i = maxLoop; i > 0; i--){
				maxLoop--;
				yield return StartCoroutine (PatternCoroutine ());
			}

			this.transform.localPosition = new Vector3 (0.0f, -(this.transform.localScale.y), 0.0f);
			yield return null;
		}
	}

	IEnumerator PatternCoroutine() {

//		this.transform.localPosition = new Vector3 (0.0f, -(this.transform.localScale.y), 0.0f);
//		yield return null;
		if (this.transform.localPosition.y != -(this.transform.localScale.y)) {

			while (elapsedTime < 0.4f) {
				Vector3 vector = Vector3.Lerp (this.transform.localPosition, new Vector3 (0.0f, -(this.transform.localScale.y), 0.0f), Time.fixedDeltaTime * 10.0f);
				elapsedTime += Time.fixedDeltaTime;
				this.transform.localPosition = vector;
				yield return null;
			}
			elapsedTime = 0.0f;


			while (elapsedTime < hideTime) {
				elapsedTime += Time.fixedDeltaTime;
				yield return null;
			}
			elapsedTime = 0.0f;
//			yield return new WaitForSeconds (hideTime);
		}

		if (isTriggered == false) {
			StopAllCoroutines();
			yield return null;
		}

		this.transform.localPosition = new Vector3 (0.0f, -0.3f, 0.0f);
		yield return null;


		while (elapsedTime < startRate) {
			elapsedTime += Time.fixedDeltaTime;
			yield return null;
		}
		elapsedTime = 0.0f;
//		yield return new WaitForSeconds (startRate);

//		this.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
//		yield return null;

		while (elapsedTime < 0.4f)
		{
			Vector3 vector = Vector3.Lerp(this.transform.localPosition,  new Vector3 (0.0f, 0.0f, 0.0f), Time.fixedDeltaTime * 10.0f);
			elapsedTime += Time.fixedDeltaTime;
			this.transform.localPosition = vector;
			yield return null;
		}
		elapsedTime = 0.0f;


		while (elapsedTime < showTime) {
			elapsedTime += Time.fixedDeltaTime;
			yield return null;
		}
		elapsedTime = 0.0f;
//		yield return new WaitForSeconds (showTime);

	}

	void OnTriggerEnter(Collider collider)
	{
//		if (isTriggered == false)
//			return;

		if(GameRuleManager.instance.eGameStatus != E_GAME_STATUS.IN_PLAY) 
			return;
		
		if(collider.gameObject.layer == LayerMask.NameToLayer("Player")) //플레이어인 경우 
		{
			if(GameRuleManager.instance.playerHealth.isInvincible == true) return;

			if (forceDir != Vector3.zero) {
				GameRuleManager.instance.playerMove.KnockBack(forceDir, knockbackDist);
			} else {
				GameRuleManager.instance.playerMove.KnockBack((GameRuleManager.instance.playerMove.transform.position - transform.position).normalized, knockbackDist); //넉백 효과도 줌 
			}

			GameRuleManager.instance.playerHealth.TakeDamage(damage); //데미지 줌 
		}
		
		//적인경우 
		if(collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
		{
//			EnemyMovement enemyMove = collider.gameObject.GetComponent<EnemyMovement>();
//			if(enemyMove != null)
//			{
//				enemyMove.Stun(damage); //적 스턴 
//			}
		}
	}
}
