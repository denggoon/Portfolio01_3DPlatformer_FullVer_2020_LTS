using UnityEngine;
using System.Collections;

public class WeightDropPlatform : Trap 
{
	public Transform platformCollider;
	public Transform startPoint;
	public Transform dropDestination;

	public bool hasShake = false;
	public bool hasDoneShaking = false;
	public float shakeTime;
	public float shakeTimer;

	public bool hasDoneWaiting = false;
	public float dropSpeed;
	public float dropWaitTime = 1F;
	public float dropWaitTimer;
	public float travelDistance;
	public float remainingDistance;
	public float distanceTolerance = 0.1F;
	public Vector3 moveDirection;

	public bool hasDoneRespawning = false;
	public float respawnTime;
	public float respawnTimer;

	public override void Awake()
	{
		base.Awake();
		platformCollider = this.transform.Find("Col_WeightDrop");

	}

	void Start()
	{
		platformCollider.position = startPoint.position;
	}

	void Update()
	{
		if(isTriggered == false) return;
		if(startPoint == null)
		{
			Debug.Log("No StartPoint!");
			return;
		}

		if(dropDestination == null)
		{
			Debug.Log("No Destination!");
			return;
		}

		if(hasShake && hasDoneShaking == false)
		{
			if(shakeTimer < shakeTime)
			{
				shakeTimer += Time.deltaTime;
				Shake();
				return;
			} else {
				hasDoneShaking = true;
				shakeTimer = 0F;
			}
		}

		if(hasDoneWaiting == false)
		{
			if(dropWaitTimer < dropWaitTime)
			{
				dropWaitTimer += Time.deltaTime;
				return;
			} else {

				hasDoneWaiting = true;
				dropWaitTimer = 0F;
				moveDirection = (dropDestination.position - platformCollider.position).normalized;
				travelDistance = Vector3.Distance(dropDestination.position, platformCollider.position);
			}
		}

		remainingDistance = Vector3.Distance(dropDestination.position, platformCollider.position);
		if(remainingDistance >= distanceTolerance)
		{
			platformCollider.Translate(moveDirection * dropSpeed * Time.deltaTime);
			return;
		}

		if(hasDoneRespawning == false)
		{
			if(respawnTimer < respawnTime)
			{
				respawnTimer += Time.deltaTime;
				return;
			} else {
				
				hasDoneRespawning = true;
				hasDoneWaiting = false;
				hasDoneShaking = false;
				isTriggered = false;
				respawnTimer = 0F;
				platformCollider.position = startPoint.position;
			}
		}
	}

	void Shake()
	{

	}


}
