using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerPassive_Magnet : PlayerPassive 
{
	new public Transform transform;

	public float magnetForce = 5F;

//	public float magneticInterval = 0F;
//	private float setInterval;

	private int coinLayer;

	private Transform magnetTrail = null;

	void Awake()
	{
		transform = GetComponent<Transform>();
	}

	void Start()
	{
		coinLayer += (1 << LayerMask.NameToLayer("Magnet"));

//		if(magneticInterval >= 0)
//		{
//			setInterval = magneticInterval;
//
//			if(magneticInterval == 0)
//			{
//				setInterval = Time.deltaTime;
//			}
//			StartCoroutine(Attract());
//		}
	}

	void Update()
	{
		if (amount <= 0) {
			return;
		}

		Collider[] coins;
		
		coins = Physics.OverlapSphere(this.transform.position, amount, coinLayer);
		
		for(int i=0; i<coins.Length; i++)
		{
			if(coins[i] == null) continue;
		
			float dist = Vector3.Distance(coins[i].transform.position, this.transform.position);
			
			if(dist <= amount)
			{
				float attractVelocity = amount / dist;
				coins[i].transform.position = Vector3.MoveTowards(coins[i].transform.position, this.transform.position, magnetForce * attractVelocity * Time.deltaTime);

				if (magnetTrail == null) {

					magnetTrail = coins[i].transform.Find("Fx_Trail_P_01");
					if (magnetTrail != null) 
					{
						if(!magnetTrail.gameObject.activeSelf)
							magnetTrail.gameObject.SetActive(true);
					}

					MovingPlatform movingPlatform = coins[i].transform.GetComponent<MovingPlatform>();
					if (movingPlatform != null) {
						movingPlatform.isTriggered = false;
					}
				}

			}
				
		}
	}

//	IEnumerator Attract()
//	{
//		while(true)
//		{
//			Collider[] coins;
//
//			coins = Physics.OverlapSphere(this.transform.position, magnetRadius, coinLayer);
//
//			for(int i=0; i<coins.Length; i++)
//			{
//				if(coins[i] == null) continue;
//
//				if(coins[i].CompareTag("Gold"))
//				{
//					float dist = Vector3.Distance(coins[i].transform.position, this.transform.position);
//
//					if(dist <= magnetRadius)
//					{
//						float attractVelocity = magnetRadius / dist;
//
//						coins[i].transform.position = Vector3.MoveTowards(coins[i].transform.position, this.transform.position, magnetForce * attractVelocity * Time.deltaTime);
//					}
//
//				}
//			}
//
//			yield return new WaitForSeconds(setInterval);
//		}
//	}
}
