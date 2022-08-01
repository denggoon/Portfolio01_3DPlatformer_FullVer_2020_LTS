using UnityEngine;
using System.Collections;

public class EnemySafePlate : MonoBehaviour {
	
	private BoxCollider subCollider = null;
	private EnemyMovement movement = null;

	// Use this for initialization
	void Start () {
		movement = GetComponentInParent<EnemyMovement> ();
		subCollider = transform.Find ("childObject").GetComponent<BoxCollider>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.layer == LayerMask.NameToLayer ("Player")) { //플레이어인 경우

			subCollider.isTrigger = false;

			if (movement != null) {
				movement.isPlayerOnTheHead = true;
			}
		}
	}

	void OnTriggerExit(Collider collider)
	{
		if (collider.gameObject.layer == LayerMask.NameToLayer ("Player")) { //플레이어인 경우

			subCollider.isTrigger = true;

			if (movement != null) {
				movement.isPlayerOnTheHead = false;
			}
		}
	}

}
