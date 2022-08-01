using UnityEngine;
using System.Collections;

public class RadarCamera : MonoBehaviour {

	GameObject player;
	// Update is called once per frame
	void Update () {

		if(player == null)
		{
			player = GameRuleManager.instance.player;
		}

		if(player == null) return;

		this.transform.position = new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z);
	
	}
}
