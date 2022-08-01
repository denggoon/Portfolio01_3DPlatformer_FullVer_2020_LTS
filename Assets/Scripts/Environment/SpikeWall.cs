using UnityEngine;
using System.Collections;

public class SpikeWall : Trap {

	public float damage = 1F; //스파이크가 주는 데미지 
	public float knockbackDist; //스파이크와 부딪혔을때 넉백 크기 

	public MovingRoute movingRoute;

	private Transform destination; //현재 스파이크가 이동하려고 하는 위치 

	public int maxLoop = 0; //스파이크가 설정된 행동을 반복하는 횟수 - 0: 무한 
	private int loopCount = 0; //그 횟수를 카운트하는 카운터 

	public float speed = 1F; //스파이크의 이동속도 

	public bool isRotateDir = false; //이동방향으로 스파이크의 정면이 향하도록 회전을 합니다. 
	public bool isPingPong = false; //이동방식이 핑퐁 스타일인가? -> 1/2/3/4/3/2/1 

	private bool isReverse = false; //핑퐁방식으로 움직일때 역순으로 움직이는지 아닌지 표시용 

	public float stoppingDist = .1F; //이정도 거리에 들어오면 최종 목적지에 도달한걸로 침 
	public float moveWaitTime = 3F; //목적지에 도달했을때 대기하는 시간 

	private float remainingDist; //목적지까지 남은거리 

	private float moveTimer; //목적지 도착시 기다리는 시간을 재는 타이머 

	private Vector3 movingVector; //스파이크의 이동방향 

	// Use this for initialization
	void Start () {

		if(movingRoute == null)
			movingRoute = GetComponent<MovingRoute> ();

		if(movingRoute == null) return; //이동경로 루트가 없는경우 시작하지 않음 
		if(movingRoute.routes.Length <= 1) return; //이동경로 루트가 2개 이상 설정되어있지 않은경우 시작하지 않음 

		movingVector = Vector3.zero; //기본적으로 이동방향은 없음, 
		remainingDist = 999F; //남은거리는 최대한으로 설정하여 다음 목적지로 바로 이동하는것을 방지 
		this.transform.position = movingRoute.routes[0].routeTrans.position; //스파이크가 어디있든 최초 위치로 강제 설정 
		this.transform.localRotation = movingRoute.routes[0].routeTrans.localRotation;

		movingRoute.routeIndex = 1; //다음에 움직일 이동경로로 인덱스 번호 설정. 
//		this.transform.rotation = Quaternion.LookRotation(GetMovingVector(movingRoute[movingRoute.routeIndex].transform.position), Vector3.up);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(movingRoute == null) return;
		if(movingRoute.routes.Length == 0) return;

		if(GameRuleManager.instance.eGameStatus == E_GAME_STATUS.GAME_READY) return;
		if(GameRuleManager.instance.eGameStatus != E_GAME_STATUS.IN_PLAY) return;

		if(isTriggered == false) return; 
		//트리거가 발동되지 않았으면 작동하지 않음. 

		if(loopCount < maxLoop || maxLoop == 0)
		{
			if(MoveTo(movingRoute.routeIndex)) //목적지 이동이 성공적일때까지 이동 
			{
				if(isPingPong) //핑퐁 스타일인경우 
				{
					if(isReverse) //역순으로 움직일 차례면 
					{
						movingRoute.routeIndex --; //인덱스를 감소 시킴 

						if(movingRoute.routeIndex < 0) //감소시킨 인덱스가 0보다 작다면 
						{
							isReverse = false; //이제는 다시 정순으로 움직여야함 
							movingRoute.routeIndex ++; //인덱스를 2 늘려서 -1에서 다음 인덱스를 1로 만듬 
							movingRoute.routeIndex ++;

							if(maxLoop > 0)
							{
								loopCount ++;
							}
						}

					} else { //정순으로 움직일 차례면 

						movingRoute.routeIndex ++; //인덱스를 증가 시킴 

						if(movingRoute.routeIndex == movingRoute.routes.Length) //증가시킨 인덱스가 루트 최대 크기와 동일한경우 
						{
							isReverse = true; //이제는 역순으로 움직여야 
							movingRoute.routeIndex --; //인덱스를 2 감소시켜서 방금 전 지나쳐온 목적지로 다시 백 
							movingRoute.routeIndex --;

							if(maxLoop > 0)
							{
								loopCount ++;
							}
						}
					}
				} 
				else  //핑퐁스타일이 아닌 순환형인경우 
				{
					movingRoute.routeIndex ++; //단순히 인덱스 늘리고 

					if(movingRoute.routeIndex == movingRoute.routes.Length) //인덱스가 초과하면 
					{
						movingRoute.routeIndex = 0; //0의 위치로. 

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

	private float prevRemainDist;
	private float diffDist;
	public bool hasGonePastDestination = false;
	bool MoveTo(int index) //부여된 인덱스로 이동 
	{
		if(movingVector == Vector3.zero) //최초엔 이동방향 벡터가 설정되어 있지 않으므로 
		{
			destination = movingRoute.routes[index].routeTrans; //제공된 위치로 이동하도록 설정 
			movingVector = GetMovingVector(destination.transform.position);

			if(movingRoute.routes[index].routeSpeed > 0F)
				speed = movingRoute.routes[index].routeSpeed;

			moveTimer = moveWaitTime; //목적지로 이동이 완료되었을때 카운트다운을 하기위한 무브 타이머 미리 설정 
		}

		if(movingVector != Vector3.zero) //방향이 설정되었으면 
		{
			prevRemainDist = remainingDist;

			remainingDist = Vector3.Distance(destination.position, transform.position); //목적지와 현 위치 거리 계산 

			diffDist = prevRemainDist - remainingDist;
			if(diffDist < 0F)
			{
				hasGonePastDestination = true;
			} else {
				hasGonePastDestination = false;
			}

			if(remainingDist >= stoppingDist && !hasGonePastDestination) //남은 거리가 아직 최소도달 허용거리보다 큰경우 
			{
				this.transform.Translate(movingVector * speed * Time.deltaTime, Space.World); //계속 그 방향으로 이동 

			} else { //대충 다 도달했다면 

				moveTimer -= Time.deltaTime; //카운트다운시작 

				if(isRotateDir)
				{
					int nextIndex = FindNextIndex(movingRoute.routeIndex); //다음 이동 인덱스를 미리 계산하는 함수 

					Vector3 nextMovingVector = GetMovingVector(movingRoute.routes[nextIndex].routeTrans.position); //다음에 움직일 방향 벡터를 미리 구합니다. 

					Debug.DrawRay(this.transform.position, nextMovingVector, Color.red);

					this.transform.rotation 
						= Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(nextMovingVector), (moveTimer/ moveWaitTime) * -1F + 1F);

					//moveTimer는 moveWaitTime에서 0으로 감소하기 때문에, moveTimer / moveWaitTime을 하면 1에서 0으로 감소합니다. 따라서 -1을 곱하고 1을 더하면 
					// 0에서 1로 증감하도록 유도할수 있습니다. 
						
					//타이머 카운트 동안 다음 방향으로 회전 
				}

				if(moveTimer <= 0) //대기시간만큼 충분히 대기했다면 
				{
					movingVector = Vector3.zero; //이동방향 리셋 
					remainingDist = 999F; //남은거리 리셋 

					return true; //성공적으로 이동하였습니다. 
				}
			}
		}

		return false; //그전까지는 계속 아직 다 이동 못한것으로 간주하며, 위의 이동 시퀀스를 계속 진행 
	}

	Vector3 GetMovingVector (Vector3 targetVector) //이동 방향 벡터를 리턴해주는 함수 
	{
		return (targetVector - this.transform.position).normalized;
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
				
				if(index == movingRoute.routes.Length) //증가시킨 인덱스가 루트 최대 크기와 동일한경우 
				{
					index --; //인덱스를 2 감소시켜서 방금 전 지나쳐온 목적지로 다시 백 
					index --;
				}
			}
		} 
		else  //핑퐁스타일이 아닌 순환형인경우 
		{
			index ++; //단순히 인덱스 늘리고 
			
			if(index == movingRoute.routes.Length) //인덱스가 초과하면 
			{
				index = 0; //0의 위치로. 
			}
		}

		return index;
	}

	void OnTriggerStay(Collider collider)
	{
		if (isTriggered == false)
			return;

		if(collider.gameObject.layer == LayerMask.NameToLayer("Player")) //플레이어인 경우 
		{
			if(GameRuleManager.instance.playerHealth.isInvincible == true) return;
			
			GameRuleManager.instance.playerMove.KnockBack((GameRuleManager.instance.playerMove.transform.position - transform.position).normalized, knockbackDist); //넉백 효과도 줌 
			
			GameRuleManager.instance.playerHealth.TakeDamage(damage); //데미지 줌 
		}

		//적인경우 
		if(collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
		{
			EnemyMovement enemyMove = collider.gameObject.GetComponent<EnemyMovement>();
			
			if(enemyMove != null)
			{
				enemyMove.Stun(damage); //적 스턴 
			}
		}
	}


}
