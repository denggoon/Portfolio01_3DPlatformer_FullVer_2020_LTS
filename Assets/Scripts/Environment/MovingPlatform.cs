using UnityEngine;
using System.Collections;

public class MovingPlatform : Trap {
	
	public float initialWaitTime;
	public float waitTimer;

	public int maxLoop = 0; //platform의 설정된 행동을 반복하는 횟수 - 0: 무한 
	private int loopCount = 0; //그 횟수를 카운트하는 카운터 
	public float speed = 1F; //platform의 이동속도 
	
//	public Vector3 movingVector = Vector3.zero;
	private MovingRoute movingRoute;
	
	[SerializeField]
	private Transform destination; //현재 platform이 이동하려고 하는 위치 
	
	public bool isRotateDir = false; //이동방향으로 platform이 정면이 향하도록 회전을 합니다. 
	public bool isPingPong = false; //이동방식이 핑퐁 스타일인가? -> 1/2/3/4/3/2/1 
	
	private bool isReverse = false; //핑퐁방식으로 움직일때 역순으로 움직이는지 아닌지 표시용 
	private float moveWaitTime = 3F; //목적지에 도달했을때 대기하는 시간 
	
	private float moveTimer; //목적지 도착시 기다리는 시간을 재는 타이머 
	public string moveEventStr;
	
	private FMOD.Studio.EventInstance movingSoundEvent;
	private FMOD.ATTRIBUTES_3D attrib3D;
	private Animator animator;
	private int idleHash;

//	private float updateInterval;
	// Use this for initialization
	void Start () {
		
		animator = GetComponentInChildren<Animator> ();
		
		if(animator != null)
			idleHash = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
		
		Collider collider = transform.GetChild(0).GetComponentInChildren<Collider> ();
		
		if (collider != null) 
		{
			collider.gameObject.tag = "MovingPlatform";
		}
		
		if(movingRoute == null)
			movingRoute = GetComponent<MovingRoute> ();
		
		if(movingRoute == null) return; //이동경로 루트가 없는경우 시작하지 않음 
		if(movingRoute.routes.Length <= 1) return; //이동경로 루트가 2개 이상 설정되어있지 않은경우 시작하지 않음 
		
		if (movingRoute.routes.Length > 0) 
		{
			this.transform.position = movingRoute.routes[0].routeTrans.position; // platform 최초 위치로 강제 설정 
			this.transform.localRotation = movingRoute.routes[0].routeTrans.localRotation;
		}
		
		movingRoute.routeIndex = 1; //다음에 움직일 이동경로로 인덱스 번호 설정. 
	}
	
	void Update() 
	{
		if(GameRuleManager.instance.eGameStatus != E_GAME_STATUS.IN_PLAY) return;
		
		if (!isTriggered) 
		{
			if(waitTimer > 0)
				waitTimer = 0F;

			return;
		}

		if (initialWaitTime > 0 && waitTimer < initialWaitTime) 
		{
			waitTimer += Time.deltaTime;

			if(waitTimer < initialWaitTime)
				return;
		}

		movingSoundEvent.set3DAttributes (FMODUnity.RuntimeUtils.To3DAttributes(this.gameObject));


		if (loopCount < maxLoop || maxLoop == 0) {
			if (MoveTo (movingRoute.routeIndex)) { //목적지 이동이 성공적일때까지 이동
				if (SoundBoard.instance != null) {
					
					movingSoundEvent.stop (FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
					
					movingSoundEvent = SoundBoard.instance.PlayLoopSoundFromBoard (moveEventStr, this.gameObject);
				}
				
				if (isPingPong) { //핑퐁 스타일인경우
					if (isReverse) { //역순으로 움직일 차례면
						movingRoute.routeIndex --; //인덱스를 감소 시킴 
						
						if (movingRoute.routeIndex < 0) { //감소시킨 인덱스가 0보다 작다면
							isReverse = false; //이제는 다시 정순으로 움직여야함 
							movingRoute.routeIndex ++; //인덱스를 2 늘려서 -1에서 다음 인덱스를 1로 만듬 
							movingRoute.routeIndex ++;
							
							if (maxLoop > 0) {
								loopCount ++;
							}
						}
						
					} else { //정순으로 움직일 차례면 
						
						movingRoute.routeIndex ++; //인덱스를 증가 시킴 
						
						if (movingRoute.routeIndex == movingRoute.routes.Length) { //증가시킨 인덱스가 루트 최대 크기와 동일한경우
							isReverse = true; //이제는 역순으로 움직여야 
							movingRoute.routeIndex --; //인덱스를 2 감소시켜서 방금 전 지나쳐온 목적지로 다시 백 
							movingRoute.routeIndex --;
							
							if (maxLoop > 0) {
								loopCount ++;
							}
						}
					}
				} else {  //핑퐁스타일이 아닌 순환형인경우
					movingRoute.routeIndex ++; //단순히 인덱스 늘리고 
					
					if (movingRoute.routeIndex == movingRoute.routes.Length) { //인덱스가 초과하면
						movingRoute.routeIndex = 0; //0의 위치로. 
						
						if (maxLoop > 0) {
							loopCount ++;
						}
					}
				}
			}
		}
		
		if (maxLoop >= 1 && loopCount >= maxLoop) { //반복 횟수가 설정되어 있고 그 횟수에 도달했을경우
			SwitchTrigger (false); //트리거 발동 장치를 다시 리셋 
			loopCount = 0; //반복 횟수도 리셋 
		}
	}
	
	private float prevRemainDist;
	private float diffDist;
	public bool hasGonePastDestination = false;
	
	public Vector3 startPos;
	public float estimatedTravel;
	public float estimatedMoveRange;
	public float moveRangeAddup;

	bool MoveTo(int index) //부여된 인덱스로 이동 
	{
		if (movingRoute.routes == null || movingRoute.routes.Length == 0)
			return true;

		if(!destination) //최초엔 이동방향 벡터가 설정되어 있지 않으므로 
		{
			destination = movingRoute.routes[index].routeTrans; //제공된 위치로 이동하도록 설정 
//			movingVector = GetMovingVector(destination.position);
			
			if(movingRoute.routes[index].routeSpeed > 0F)
				speed = movingRoute.routes[index].routeSpeed;
			
			//			if(movingRoute.routes[index].routeEndWaitTime > 0F)
			//
			startPos = transform.position;
			moveWaitTime = movingRoute.routes[index].routeEndWaitTime;
			moveTimer = moveWaitTime; //목적지로 이동이 완료되었을때 카운트다운을 하기위한 무브 타이머 미리 설정 
			
			moveRangeAddup = 0F;
			estimatedTravel = Vector3.Distance(destination.position, transform.position);
			estimatedMoveRange = Time.deltaTime * speed;	//updateInterval * speed; //목적지와 현 위치 거리 계산 // ;
			
		} 
		
		if (destination)
		{ //방향이 설정되었으면 
			
			moveRangeAddup += estimatedMoveRange;
			
			//			if(remainingDist >= stoppingDist && !hasGonePastDestination) //남은 거리가 아직 최소도달 허용거리보다 큰경우 
			
			if(moveRangeAddup < estimatedTravel)
			{
				this.transform.position = Vector3.Lerp(startPos, destination.position, moveRangeAddup / estimatedTravel);
//				this.transform.Translate(movingVector * speed * updateInterval, Space.World); //계속 그 방향으로 이동 
				
			} else { //대충 다 도달했다면, 또는 목적지를 지나친것이 확인된경우 

				if(moveTimer == moveWaitTime)
				{
					this.transform.position = destination.position;
					//해당 루트에 도달시 실행해야 할 음원이나 이펙트가 있다면? 
					if(!string.IsNullOrEmpty(movingRoute.routes[index].routeEndSound))
					{
						if(SoundBoard.instance != null)
							SoundBoard.instance.PlayFromSoundBoard(movingRoute.routes[index].routeEndSound, this.transform.position);
					}
					
					if(!string.IsNullOrEmpty(movingRoute.routes[index].routeEndFx))
					{
						if(ResourcesManager.instance != null)
							ResourcesManager.instance.PopEffect(movingRoute.routes[index].routeEndFx, this.transform.position);
					}
					
					if(!string.IsNullOrEmpty(movingRoute.routes[index].routeEndAni))
					{
						if(animator != null)
							animator.Play(movingRoute.routes[index].routeEndAni);
					}
				}

//				moveTimer -= updateInterval; //카운트다운시작 
				moveTimer -= Time.deltaTime; //카운트다운시작 
				
				if(isRotateDir)
				{
					int nextIndex = FindNextIndex(movingRoute.routeIndex); //다음 이동 인덱스를 미리 계산하는 함수 
					
					Vector3 nextMovingVector = GetMovingVector(movingRoute.routes[nextIndex].routeTrans.position); //다음에 움직일 방향 벡터를 미리 구합니다. 
					
					//					Debug.DrawRay(this.transform.position, nextMovingVector, Color.red);
					
					this.transform.rotation 
						= Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(nextMovingVector), (moveTimer/ moveWaitTime) * -1F + 1F);
					
					//moveTimer는 moveWaitTime에서 0으로 감소하기 때문에, moveTimer / moveWaitTime을 하면 1에서 0으로 감소합니다. 따라서 -1을 곱하고 1을 더하면 
					// 0에서 1로 증감하도록 유도할수 있습니다. 
					
					//타이머 카운트 동안 다음 방향으로 회전 
				}
				
				if(moveTimer <= 0) //대기시간만큼 충분히 대기했다면 
				{
					if(animator != null)
						animator.Play(idleHash);
				
					moveTimer = moveWaitTime;
//					movingVector = Vector3.zero; //이동방향 리셋 
					//					remainingDist = 999F; //남은거리 리셋
					hasGonePastDestination = false;
					destination = null;
					
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
}
