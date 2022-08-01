using UnityEngine;
using System.Collections;

[System.Serializable]
public class CamRouteInfo : RouteInfo
{
	[SerializeField]
	public bool applyOnGoingAngles = false;

//	[HideUnless("applyOnGoingAngles")]
//	public Vector3 onGoingAngles;

	[SerializeField]
	public bool applyWaitTimeAngles = false;

	[HideUnless("applyWaitTimeAngles")]
	public Vector3 waitTimeAngles;

	public CamRouteInfo(Transform trans, float speed = 0F): base(trans, speed)
	{
		routeTrans = trans;
		routeSpeed = speed;
	}
}

public class CameraRoute : CameraEvent 
{
	public GameObject goPatrolPattern;
	//	public Transform[] routeTrans;
	
	public CamRouteInfo[] routes;
	
	public int routeIndex = 0;
	
	public float speed = 1F; //스파이크의 이동속도 
	
	[SerializeField]
	private Transform destination; //현재 스파이크가 이동하려고 하는 위치 
	
	public bool isPingPong = false; //이동방식이 핑퐁 스타일인가? -> 1/2/3/4/3/2/1 
	
	private bool isReverse = false; //핑퐁방식으로 움직일때 역순으로 움직이는지 아닌지 표시용 

	public float stoppingDist = .1F; //이정도 거리에 들어오면 최종 목적지에 도달한걸로 침 

	[SerializeField]
	private float moveWaitTime = 3F; //목적지에 도달했을때 대기하는 시간 

	[SerializeField]
	private float remainingDist; //목적지까지 남은거리 
	
	private float waitTimer; //목적지 도착시 기다리는 시간을 재는 타이머 
	
	public Vector3 movingVector; //스파이크의 이동방향 
	public string moveEventStr;
	
	private FMOD.Studio.EventInstance movingSoundEvent;
	private FMOD.ATTRIBUTES_3D attrib3D;
	private Animator animator;
	private int idleHash;

	void Start()
	{
		remainingDist = 999F;
	}

	public override void SwitchTrigger(bool flag)
	{
		base.SwitchTrigger (flag);

		GameRuleManager.instance.sideCam.SetToDefaultCamera ();
		GameRuleManager.instance.sideCam.HoldCameraFunction (flag);
	}
	
	public override void Awake() //프리팹 routeTrans로 바꾸는 함수 
	{
		base.Awake ();

		if (goPatrolPattern == null) 
		{
			Transform routeBox = transform.parent.Find ("route_Box");
			
			if(routeBox != null)
				goPatrolPattern = routeBox.gameObject;
			
		}
		
		IntepretePatternFromPrefab ();
		RemoveUnnecessaryColliders ();
	}
	
	public void IntepretePatternFromPrefab()
	{
		if (goPatrolPattern == null)
			return;
		
		if (goPatrolPattern.transform.childCount <= 0)
			return;
				
		if(routes == null || routes.Length == 0)
			routes = new CamRouteInfo[goPatrolPattern.transform.childCount];
		
		for (int j=0; j<routes.Length; j++) 
		{
			Transform transformInfo = goPatrolPattern.transform.GetChild(j);
			if(routes[j] == null)
			{
				CamRouteInfo route = new CamRouteInfo(transformInfo);
				routes[j] = route;
			} else {
				routes[j].routeTrans = transformInfo;
			}
		}
		
		System.Array.Sort(routes, delegate(CamRouteInfo a, CamRouteInfo b) { return a.routeTrans.name.CompareTo(b.routeTrans.name); });
	}
	
	void RemoveUnnecessaryColliders()
	{
		for (int j=0; j<routes.Length; j++) 
		{
			Collider routeCollider = routes [j].routeTrans.GetComponent<Collider> ();
			if (routeCollider != null) {
				Destroy (routeCollider);
			}
		}
	}

	public override void Update ()
	{
		base.Update ();

		if(GameRuleManager.instance.eGameStatus != E_GAME_STATUS.IN_PLAY) return;

		if (cameraTrans == null)
			return;
		
		if(isTriggered == false) return; 

		if(routes == null) return;
		if(routes.Length == 0) return;

		if(loopCount < maxLoop || maxLoop == 0)
		{
			if(MoveTo(routeIndex)) //목적지 이동이 성공적일때까지 이동 
			{
				if (SoundBoard.instance != null) 
				{
					movingSoundEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
					
					movingSoundEvent = SoundBoard.instance.PlayLoopSoundFromBoard (moveEventStr, this.gameObject);
				}
				
				if(isPingPong) //핑퐁 스타일인경우 
				{
					if(isReverse) //역순으로 움직일 차례면 
					{
						routeIndex --; //인덱스를 감소 시킴 
						
						if(routeIndex < 0) //감소시킨 인덱스가 0보다 작다면 
						{
							isReverse = false; //이제는 다시 정순으로 움직여야함 
							routeIndex ++; //인덱스를 2 늘려서 -1에서 다음 인덱스를 1로 만듬 
							routeIndex ++;
							
							if(maxLoop > 0)
							{
								loopCount ++;
							}
						}
						
					} else { //정순으로 움직일 차례면 
						
						routeIndex ++; //인덱스를 증가 시킴 
						
						if(routeIndex == routes.Length) //증가시킨 인덱스가 루트 최대 크기와 동일한경우 
						{
							isReverse = true; //이제는 역순으로 움직여야 
							routeIndex --; //인덱스를 2 감소시켜서 방금 전 지나쳐온 목적지로 다시 백 
							routeIndex --;
							
							if(maxLoop > 0)
							{
								loopCount ++;
							}
						}
					}
				} 
				else  //핑퐁스타일이 아닌 순환형인경우 
				{
					routeIndex ++; //단순히 인덱스 늘리고 
					
					if(routeIndex == routes.Length) //인덱스가 초과하면 
					{
						routeIndex = 0; //0의 위치로. 
						
						if(maxLoop > 0)
						{
							loopCount ++;
						}
					}
				}
			}
		}
		
		if(maxLoop >=1 && loopCount >= maxLoop) //반복 횟수가 설정되어 있고 그 횟수에 도달했을경우 
		{
			SwitchTrigger(false); //트리거 발동 장치를 다시 리셋 
			loopCount = 0; //반복 횟수도 리셋 
		}
	}

	
	private float prevRemainDist = 999F;

	[SerializeField]
	private float diffDist;
	public bool hasGonePastDestination = false;

	private float estimatedDistance;

	public Quaternion curRot;
	public Quaternion nextRot;

	bool MoveTo(int index) //부여된 인덱스로 이동 
	{
		if(!destination) //최초엔 이동방향 벡터가 설정되어 있지 않으므로 
		{
			destination = routes[index].routeTrans; //제공된 위치로 이동하도록 설정 

			curRot = cameraTrans.localRotation;

			movingVector = GetMovingVector(destination.position);
			
			if(routes[index].routeSpeed > 0F)
				speed = routes[index].routeSpeed;

			moveWaitTime = routes[index].routeEndWaitTime;

			estimatedDistance = Vector3.Distance(destination.position, cameraTrans.position);
			waitTimer = 0F; //목적지로 이동이 완료되었을때 카운트다운을 하기위한 무브 타이머 미리 설정 
		}
		
		if(destination) //방향이 설정되었으면 
		{
			prevRemainDist = remainingDist; //이전 프레임의 남은 거리 값을 저장합니다. 

			remainingDist = Vector3.Distance(destination.position, cameraTrans.position); //목적지와 현 위치 거리 계산 
			
			diffDist = prevRemainDist - remainingDist; //이전 프레임의 남은거리와 현 프레임의 남은거리의 오차를 계산하여 
			
			if(diffDist <= 0F) //양수이면 목적지를 아직 지나치지 않은것이며, 음수이면 지나친것이다. 
			{
				hasGonePastDestination = true;
			}
			
			if(remainingDist >= stoppingDist && !hasGonePastDestination) //남은 거리가 아직 최소도달 허용거리보다 큰경우 
			{
				GameRuleManager.instance.sideCam.transform.Translate(movingVector * speed * Time.deltaTime, Space.World); //계속 그 방향으로 이동 

				if(routes[index].applyOnGoingAngles)
				{
					nextRot = routes[index].routeTrans.localRotation;
					 
					GameRuleManager.instance.sideCam.SetCameraQuaternionAngle(Quaternion.Slerp(curRot, nextRot, (estimatedDistance - remainingDist) / estimatedDistance));
//					cameraTrans.localRotation = Quaternion.Slerp(curRot, nextRot, (estimatedDistance - remainingDist) / estimatedDistance);
				}
				
			} else { //대충 다 도달했다면, 또는 목적지를 지나친것이 확인된경우 
				
				if(waitTimer == 0F)
				{
					//해당 루트에 도달시 실행해야 할 음원이나 이펙트가 있다면? 
					if(!string.IsNullOrEmpty(routes[index].routeEndSound))
					{
						if(SoundBoard.instance != null)
							SoundBoard.instance.PlayFromSoundBoard(routes[index].routeEndSound, cameraTrans.position);
					}
					
					if(!string.IsNullOrEmpty(routes[index].routeEndFx))
					{
						if(ResourcesManager.instance != null)
							ResourcesManager.instance.PopEffect(routes[index].routeEndFx, cameraTrans.position);
					}
					
					if(!string.IsNullOrEmpty(routes[index].routeEndAni))
					{
						if(animator != null)
							animator.Play(routes[index].routeEndAni);
					}
				}
				
				waitTimer += Time.deltaTime; //카운트다운시작 
			
				if(routes[index].applyWaitTimeAngles)
				{
					if(routes[index].applyOnGoingAngles)
					{
						curRot = routes[index].routeTrans.localRotation;
					}

					nextRot = Quaternion.Euler(routes[index].waitTimeAngles);

//					cameraTrans.localRotation = 

					GameRuleManager.instance.sideCam.SetCameraQuaternionAngle(Quaternion.Slerp(curRot, nextRot, waitTimer/ moveWaitTime));
				}
				
				if(waitTimer >= moveWaitTime) //대기시간만큼 충분히 대기했다면 
				{
					if(animator != null)
						animator.Play(idleHash);
					
					movingVector = Vector3.zero; //이동방향 리셋 
					remainingDist = 999F; //남은거리 리셋
					hasGonePastDestination = false;
					destination = null;

//					Debug.LogError("Route " + index + " Complete");
					return true; //성공적으로 이동하였습니다. 
				}
			}
		}
		
		return false; //그전까지는 계속 아직 다 이동 못한것으로 간주하며, 위의 이동 시퀀스를 계속 진행 
	}
	
	Vector3 GetMovingVector (Vector3 targetVector) //이동 방향 벡터를 리턴해주는 함수 
	{
		return (targetVector - cameraTrans.position).normalized;
	}
	
	int FindNextIndex(int index)
	{
		if(isPingPong) //핑퐁 스타일인경우 
		{
			if(isReverse) //역순으로 움직일 차례면 
			{
				index --; //인덱스를 감소 시킴 
				
				if(index < 0) //감소시킨 인덱스가 0보다 작다면 
				{
					index ++; //인덱스를 2 늘려서 -1에서 다음 인덱스를 1로 만듬 
					index ++;
				}
				
			} else { //정순으로 움직일 차례면 
				
				index ++; //인덱스를 증가 시킴 
				
				if(index == routes.Length) //증가시킨 인덱스가 루트 최대 크기와 동일한경우 
				{
					index --; //인덱스를 2 감소시켜서 방금 전 지나쳐온 목적지로 다시 백 
					index --;
				}
			}
		} 
		else  //핑퐁스타일이 아닌 순환형인경우 
		{
			index ++; //단순히 인덱스 늘리고 
			
			if(index == routes.Length) //인덱스가 초과하면 
			{
				index = 0; //0의 위치로. 
			}
		}
		
		return index;
	}
}
