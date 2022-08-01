using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimatorMoveCaller : MonoBehaviour {

	private PlayerMoveCC player;
	void Start()
	{
		player = GameRuleManager.instance.playerMove;

		if (player == null) 
			this.enabled = false;
	}

	void OnAnimatorMove()
	{
		player.OnPlayerMove();
	}
}
