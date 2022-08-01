using UnityEngine;
using System.Collections;

public class PlayerFollower : MonoBehaviour {

	// Update is called once per frame
	public new Transform transform;
	void Awake()
	{
		transform = GetComponent<Transform> ();
	}

	void Update () 
	{
		if(GameRuleManager.instance.playerMove == null) return;

		this.transform.position = GameRuleManager.instance.playerMove.transform.position;
	}
}
