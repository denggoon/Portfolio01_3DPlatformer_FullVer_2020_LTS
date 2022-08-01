using UnityEngine;
using System.Collections;

public class SandTomb : Trap {

	public float speedDown;
	public float jumpDown;

	private float defaultSpeed = 0.0f;
	private float defaultJumpDown = 0.0f;

	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay(Collider collider)
	{
		if (collider.gameObject.layer == LayerMask.NameToLayer ("Player")) { //플레이어인 경우

			if(GameRuleManager.instance.eGameStatus != E_GAME_STATUS.IN_PLAY) return;
			
			if (isTriggered == false)
				return;

			if ( defaultSpeed == 0.0f) {
			    defaultSpeed = GameRuleManager.instance.playerMove.speed;
			    defaultJumpDown = GameRuleManager.instance.playerMove.jumpSpeed;
			}

			GameRuleManager.instance.playerMove.speed = defaultSpeed - speedDown;
			GameRuleManager.instance.playerMove.jumpSpeed = defaultJumpDown - jumpDown;
		}
	}

	void OnTriggerExit(Collider collider) 
	{
		
		if (collider.gameObject.layer == LayerMask.NameToLayer ("Player")) { //플레이어인 경우

			GameRuleManager.instance.playerMove.speed = defaultSpeed;
			GameRuleManager.instance.playerMove.jumpSpeed = defaultJumpDown;
		}

	}
}
