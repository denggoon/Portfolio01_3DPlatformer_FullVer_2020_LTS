using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public enum PURCHASED_ITEM
{
	HEALTH_BOOST = 1001,
	SPEED_BOOST = 1002,
	MAGNETIC = 1003,
	EXTRA_TIME = 1004
}

[System.Serializable]
public class ItemEffect
{
	public int item_id;
	public string item_desc;
	public float effect_value;
	public float effect_duration;
}

public class PurchasedItemEffect : MonoBehaviour {

	private static PurchasedItemEffect instance_;

	public static PurchasedItemEffect instance
	{
		get
		{
			if(instance_ == null)
			{
				GameObject itemFxObj = new GameObject("_PurchasedItemEffect");
//				Transform itemFxObjTrans = itemFxObj.GetComponent<Transform>();
//				
//				if(GameRuleManager.instance != null)
//				{
//									
//					itemFxObjTrans.SetParent(GameRuleManager.instance.transform);
//				}
				
				instance_ = itemFxObj.AddComponent<PurchasedItemEffect>();
			}
			
			return instance_;
		}
	}

	public TextAsset balanceFile;
	public List<ItemEffect> itemBalance;
	public List<ItemEffect> currentItemFxs = new List<ItemEffect>();
	public List<int> purchasedItems = new List<int>();

	void Awake() 
	{
		instance_ = this;

		balanceFile = ResourcesManager.instance.ResourcesLoadCached ("PurchasedItemBalance") as TextAsset;
		
		JsonMapper.RegisterImporter<double, float>(DoubleToFloat);
		itemBalance = JsonMapper.ToObject<List<ItemEffect>>(balanceFile.text);

		Initialize ();
		
	}
	
	float DoubleToFloat(double val)
	{
		return (float)val;
	}
	
	double FloatToDouble(float val)
	{
		return (double)val;
	}

	void OnDestroy()
	{
		instance_ = null;
	}

	public float GetItemEffectValue(PURCHASED_ITEM enumVal)
	{
		for(int i=0; i<currentItemFxs.Count; i++)
		{
			Debug.Log("ID: " + currentItemFxs[i].item_id + " / " + (int)enumVal);
			if(currentItemFxs[i].item_id == (int)enumVal)
			{
				Debug.Log("Value!: " + currentItemFxs[i].effect_value);
				return currentItemFxs[i].effect_value;
			}
		}

		return 0F;
	}

	public void Initialize()
	{
		currentItemFxs.Clear ();

//		purchasedItems.Add (1002);

		for(int i=0; i<purchasedItems.Count; i++)
		{
			for(int j=0; j<itemBalance.Count; j++)
			{
				ItemEffect item =  itemBalance[j];
				if(purchasedItems[i] == item.item_id)
				{
					currentItemFxs.Add(item);
				}
		   }
		}
	}

	public void StartItemEffectDuration()
	{
		for(int k=0; k<currentItemFxs.Count; k++)
		{
			ItemEffect itemFx = currentItemFxs[k];
			if(itemFx.effect_duration > 0F)
			{
				StartCoroutine(SetEffectDuration(itemFx));
			}
		}
	}

	public IEnumerator SetEffectDuration(ItemEffect itemFx)
	{
		yield return new WaitForSeconds(itemFx.effect_duration);

		itemFx.effect_value = 0F;
	}
}
