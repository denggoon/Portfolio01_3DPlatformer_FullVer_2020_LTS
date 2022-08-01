using UnityEngine;
using System.Collections;

public class PlayerSpawn : MonoBehaviour {

	new public Transform transform;

	public string playerObjStr;
	public string spawnFxObjStr;
	public string closeFxObjStr;

	[SerializeField]
	private GameObject playerObj;

	public float spawnDelayTime;
	public float destroyDelayTime;
	// Use this for initialization
	void Awake()
	{
		if (playerObj == null) 
		{
			playerObj = ResourcesManager.instance.LoadGameObject (playerObjStr);
		}

		transform = GetComponent<Transform>();
	}

	IEnumerator Start () 
	{
		yield return new WaitForSeconds(spawnDelayTime);

		if(SoundBoard.instance != null)
			SoundBoard.instance.PlayFromSoundBoard("SND_FX_PORTAL_SPAWN", this.transform.position);

		ResourcesManager.instance.PopEffect (spawnFxObjStr, this.transform.position);

		GameObject.Instantiate(playerObj, this.transform.position, this.transform.rotation);

		playerObj = null;

		float timer = GameRuleManager.instance.gameReadyTimer;

		if(timer >= 1F)
		{
			yield return new WaitForSeconds(GameRuleManager.instance.gameReadyCount -1F);
		} else {
			yield return new WaitForSeconds(1F);
		}

		WarpClose();
	}

	public void WarpClose()
	{
		if(SoundBoard.instance != null)
			SoundBoard.instance.PlayFromSoundBoard("SND_FX_PORTAL_DESPAWN", this.transform.position);

		ResourcesManager.instance.PopEffect (closeFxObjStr, this.transform.position);

		Destroy(this.gameObject, destroyDelayTime);

	}
}
