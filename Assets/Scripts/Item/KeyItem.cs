using UnityEngine;
using System.Collections;

public class KeyItem : MonoBehaviour {

	public KeyItemSettings keyItemSettings;

	new public Transform transform;
	public KeyItemTrigger itemTrigger;
	public int amount = 1;

	public string getSoundStr;

	public string destroyFxStr;

	public Collider collectCollider;
	public float triggerEnableTime = .5F;

	public virtual void Awake()
	{
		transform = GetComponent<Transform>();
		collectCollider = GetComponent<Collider>();

		getSoundStr = "SND_ITM_CMN_GET";
	}

	public void SetKeyItemSettings(KeyItemSettings settings)
	{
		this.keyItemSettings = settings;

		collectCollider.enabled = false;
		settings.magnetCollider.enabled = false;
		StartCoroutine(EnableCollider(triggerEnableTime));
	}

	public virtual IEnumerator EnableCollider(float time)
	{
		yield return new WaitForSeconds(time);
		
		collectCollider.enabled = true;
		keyItemSettings.magnetCollider.enabled = true;
	}

	public virtual void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.layer == LayerMask.NameToLayer("Player") == false) return; //골드에 닿은 상대가 플레이어일 경우 

		if(itemTrigger != null)
		{
			itemTrigger.UpdateItemCollect(this);
		}

		if(SoundBoard.instance != null)
			SoundBoard.instance.PlayFromSoundBoard(getSoundStr, this.transform.position);


		if(!string.IsNullOrEmpty(destroyFxStr))
		{
			ResourcesManager.instance.PopEffect(destroyFxStr, this.transform.position);
		}

		if(keyItemSettings != null)
		{
			Destroy (keyItemSettings.transform.parent.gameObject);
		} else {
			Destroy (this.gameObject.gameObject);
		}

	}
}
