using UnityEngine;
using System.Collections;

public class DeathBall : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider collider) //무언가와 충돌 
	{
		if (collider.gameObject.layer == LayerMask.NameToLayer ("Player")) { //플레이어인 경우
//			Debug.LogWarning (">>>>>>>> Death");
			GameRuleManager.instance.GameOver ();
		} else 
		{
			Debug.LogWarning (">>> " + collider.gameObject.tag + "|" + collider.gameObject.layer );

			if (collider.transform.parent.gameObject != null) {
				Destroy(collider.transform.parent.gameObject);
//				collider.transform.parent.gameObject.SetActive(false);
			}
		}


	}
}
