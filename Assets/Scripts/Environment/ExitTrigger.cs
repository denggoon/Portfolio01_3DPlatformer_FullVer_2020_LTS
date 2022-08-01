using UnityEngine;
using System.Collections;

public class ExitTrigger : MonoBehaviour 
{
	new public Transform transform;
	public string fxObjStr;

	void Awake()
	{
		transform = GetComponent<Transform>();
	}

	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			if(SoundBoard.instance != null)
				SoundBoard.instance.PlayFromSoundBoard("SND_FX_PORTAL_DESPAWN", this.transform.position);

			GameRuleManager.instance.StageClear();

			Destroy(GameRuleManager.instance.playerMove.mainTransform.gameObject);

			GameRuleManager.instance.player = null;

			ResourcesManager.instance.PopEffect(fxObjStr, this.transform.position);

			Destroy(this.gameObject);
		}
	}
}
