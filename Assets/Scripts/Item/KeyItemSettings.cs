using UnityEngine;
using System.Collections;

public enum KEY_ITEMS
{
	GOLD,
	HP,
	MAGNET,
	INVINCIBLE
}

public class KeyItemSettings : MonoBehaviour {

//	private Animator animator;
	private Collider triggerCollider;
	public Collider magnetCollider;
	private KeyItem keyItem;

	public KEY_ITEMS eKeyItems = KEY_ITEMS.GOLD;
	public int amount;
	public float colliderEnableTime = .5F;
	public bool isDroppable = false;
	public string[] collectSoundPaths;
	public string destroyFxStr;
//	private BoxCollider dropCollider;

	public MovingPlatform followingPlatform;
	public float initDist;
	public Vector3 offsetVector;

	public new Transform transform;

	void Awake()
	{
//		animator = transform.GetComponentInChildren<Animator> ();

		transform = GetComponent<Transform> ();
		
		Collider[] collArr = transform.GetComponentsInChildren<Collider> ();
		
		for (int i=0; i<collArr.Length; i++) 
		{
			if(magnetCollider == null)
			{
				if(collArr[i].gameObject.layer == LayerMask.NameToLayer("Magnet"))
				{
					magnetCollider = collArr[i];
					magnetCollider.enabled = false;
				}
			}

			if(string.Equals(collArr[i].name, "trigger"))
			{
				triggerCollider = collArr[i];
				break;
			}
		}
		
		if (triggerCollider != null) 
		{
			AttachKeyItemScript();
			triggerCollider.gameObject.layer = LayerMask.NameToLayer("Item");
			triggerCollider.isTrigger = true;
		}
		
		if (isDroppable) 
		{
			Rigidbody rigidbody = this.transform.parent.gameObject.AddComponent<Rigidbody>();
			rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
//			dropCollider = this.transform.parent.gameObject.AddComponent<BoxCollider>();
			this.transform.parent.gameObject.AddComponent<BoxCollider>();
			this.transform.parent.gameObject.layer = LayerMask.NameToLayer("WallDetectOnly");
		}
	}
	
	void Start()
	{
		if (followingPlatform != null) 
		{
			initDist = Vector3.Distance(followingPlatform.transform.position, this.transform.parent.position);
			offsetVector = this.transform.position - followingPlatform.transform.position;
		}

		// 15.12.07 infiniteMode..... from YoungHoon
		if (isDroppable == false) {  
			Collider[] collArr = transform.GetComponentsInChildren<Collider> ();
		
			for (int i=0; i<collArr.Length; i++) {
				collArr [i].enabled = true;
			}
		}
	}

	void AttachKeyItemScript()
	{
		switch (eKeyItems) 
		{
		case KEY_ITEMS.GOLD:
			keyItem = triggerCollider.gameObject.AddComponent<ItemGold>();
			break;

		case KEY_ITEMS.HP:
			keyItem = triggerCollider.gameObject.AddComponent<ItemHP>();
			break;

		case KEY_ITEMS.INVINCIBLE:
			keyItem = triggerCollider.gameObject.AddComponent<ItemInvincible>();
			break;

		case KEY_ITEMS.MAGNET:
			keyItem = triggerCollider.gameObject.AddComponent<ItemMagnet>();
			break;
		}

		keyItem.amount = amount;
		keyItem.destroyFxStr = destroyFxStr;
		keyItem.triggerEnableTime = this.colliderEnableTime;

		keyItem.SetKeyItemSettings (this);

	}

	public float distance;

	void Update()
	{
		if (followingPlatform == null)
			return;

		distance = Vector3.Distance(this.transform.parent.position, followingPlatform.transform.position);
		
		if(distance != initDist) 
		{
			transform.parent.position = followingPlatform.transform.position + offsetVector;
		}
	}
}
