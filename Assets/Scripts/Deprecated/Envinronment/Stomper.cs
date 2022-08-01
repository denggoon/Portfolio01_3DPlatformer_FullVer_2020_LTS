using UnityEngine;
using System.Collections;

public class Stomper : Trap {

	public int maxLoop = 0; //스파이크가 설정된 행동을 반복하는 횟수 - 0: 무한 
	public int loopCount = 0; //그 횟수를 카운트하는 카운터 

	public float dropSpeed = 1F; //스파이크의 이동속도 
	public float dropWaitTime = 1F;

	public float liftSpeed = 1F;
	public float liftWaitTime = 1F;

	public float moveTimer;

	public Transform landingPoint; //착지 위치 
	public Transform peakPoint; //최고 높이 

	public float stoppingDist = .1F;
	public float remainingDist = 0F;

	public bool isLifting = false;

	public Vector3 movingVector = Vector3.zero;
	public Vector3 destination;
	public float movingSpeed = 0F;

	void Start()
	{
		if(isLifting)
		{
			this.transform.position = landingPoint.transform.position;
		} else {
			this.transform.position = peakPoint.transform.position;
		}
	}

	void FixedUpdate () 
	{
		if(landingPoint == null) return;
		if(peakPoint == null) return;
		
		if(GameRuleManager.instance.eGameStatus == E_GAME_STATUS.GAME_READY) return;
		if(GameRuleManager.instance.eGameStatus != E_GAME_STATUS.IN_PLAY) return;
		
		if(isTriggered == false) return; 
		//트리거가 발동되지 않았으면 작동하지 않음. 
		
		if(loopCount < maxLoop || maxLoop == 0)
		{
			if(MoveTo()) //목적지 이동이 성공적일때까지 이동 
			{
				if(maxLoop > 0)
				{
					loopCount ++;
				}
			}
		}
		
		if(maxLoop >=1 && loopCount >= maxLoop) //반복 횟수가 설정되어 있고 그 횟수에 도달했을경우 
		{
			SwitchTrigger(false); //트리거 발동 장치를 다시 리셋 
			loopCount = 0; //반복 횟수도 리셋 
		}
	}
	
	bool MoveTo() //부여된 인덱스로 이동 
	{
		if(movingVector == Vector3.zero) //최초엔 이동방향 벡터가 설정되어 있지 않으므로 
		{
			if(isLifting)
			{
				destination = peakPoint.transform.position;
				movingSpeed = liftSpeed;
				moveTimer = dropWaitTime; //목적지로 이동이 완료되었을때 카운트다운을 하기위한 무브 타이머 미리 설정 

			} else {
				destination = landingPoint.transform.position;
				movingSpeed = dropSpeed;
				moveTimer = liftWaitTime; //목적지로 이동이 완료되었을때 카운트다운을 하기위한 무브 타이머 미리 설정 
			}

			movingVector = GetMovingVector(destination);
			
		}
		
		if(movingVector != Vector3.zero) //방향이 설정되었으면 
		{
			remainingDist = Vector3.Distance(destination, transform.position); //목적지와 현 위치 거리 계산 
			
			if(remainingDist >= stoppingDist) //남은 거리가 아직 최소도달 허용거리보다 큰경우 
			{
				this.transform.Translate(movingVector * movingSpeed * Time.fixedDeltaTime, Space.World); //계속 그 방향으로 이동 
				
			} else { //대충 다 도달했다면 
				
				moveTimer -= Time.deltaTime; //카운트다운시작 

				if(moveTimer <= 0) //대기시간만큼 충분히 대기했다면 
				{
					movingVector = Vector3.zero; //이동방향 리셋 
					remainingDist = 999F; //남은거리 리셋 
					isLifting = !isLifting;
					
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
}
