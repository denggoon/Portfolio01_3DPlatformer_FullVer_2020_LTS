using UnityEngine;
using System.Collections;

//적의 시야를 구현하는 스크립트 
public class EnemyVision : MonoBehaviour {

	new public Transform transform;

	public EnemyMovement enemyMovement; //적의 움직임을 담당하는 스크립트가 필요 

	public SphereCollider visionCollider; //적의 시야 범위 감지용 컬라이더 //Vision Layer를 가지고 있으며 Collision, Projectile과 충돌하지 않습니다. 

	public float patrolVision = 3F;
	public float chaseVision = 6F;

	public float visionRadius = 3F; //적의 시야 거리 (반지름) 

	public float viewAngle = 360F; //적의 시야각 (정면 기준) 
	public bool sameLevelDetection = true; //적의 눈과 같은 높이에 있을때만 플레이어를 감지합니다.

	public bool playerInSight; //플레이어가 시야에 들어왔는가? 
	public Vector3 normVisionVector;

	public Vector3 lastPlayerSighted; //마지막으로 플레이어를 감지한 위치 
	public Vector3 unreachablePos = new Vector3(-9999F, -9999F, -9999F); //접근 불가 포지션 

	public float distance; //플레이어와 나와의 거리 

	public Texture2D eyeTexture; //기즈모 표시용 눈 텍스쳐 

	public int playerCollisionLayer;
	public int playerLayerOnly;

	public bool canSeeThroughWalls = false;

	void Awake()
	{
		transform = GetComponent<Transform>();
		visionCollider = GetComponent<SphereCollider>(); //transform.Find("VisionCollider").GetComponent<SphereCollider>();

		visionRadius = patrolVision;

		VisionSetting();

		enemyMovement = transform.parent.GetComponent<EnemyMovement>();

		//Start
		int playerLayer = LayerMask.NameToLayer("Player");
		int collisionLayer = LayerMask.NameToLayer("Collision");

		playerLayerOnly += (1 << playerLayer);

		playerCollisionLayer += (1 << playerLayer) + (1 << collisionLayer);

		lastPlayerSighted = unreachablePos; //게임 처음 시작시엔 플레이어를 못본 상태 
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (GameRuleManager.instance == null)
			return;

		if(GameRuleManager.instance.eGameStatus != E_GAME_STATUS.IN_PLAY) return;

		VisionSetting(); //기본 시야 설정을 합니다. 
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawIcon(lastPlayerSighted, "eye.png"); //씬뷰에 적이 바라보는 대상을 표시할 기즈모를 그립니다. 
	}

	void VisionSetting()
	{
		if(visionCollider.radius != visionRadius) //시야 컬라이더의 반지름이 설정값과 다른경우 
			visionCollider.radius = visionRadius; //맞춥니다. 
	}

	//정지완) 매 프레임마다 VisionCollider 컴포넌트에 충돌체가 들어왔는지를 체크하는것이 퍼포먼스 적으로 부담이 고려되는경우, 
	//아래 OnTriggerStay 이벤트 함수를 Physics.SphereOverlap으로 대체하고, SphereOverlap 함수를 호출하는 빈도수를 interval로 조절하는 방식을 고려해 볼 수 있습니다. 
	void OnTriggerStay(Collider col) //시야 범위내에 충돌체가 들어온 경우 
	{
		if(GameRuleManager.instance == null) return;
		if(enemyMovement.IsStunned()) return; //몬스터가 스턴(데미지 입은) 상태이면 시야 검색을 하지 않습니다. 

		if (col.gameObject == GameRuleManager.instance.player) //감지한 충돌체가 플레이어인경우 
		{
			playerInSight = false; //기본 설정은 플레이어를 감지하지 못한 상태 

			if(GameRuleManager.instance.eGameStatus == E_GAME_STATUS.GAME_OVER) return; //게임 오버면 플레이어를 감지 하지 않습니다. 
			if(GameRuleManager.instance.playerHealth.currHealth <= 0F) return; //플레이어가 체력이 0이하 면 본체 만체 합니다. 
			if(GameRuleManager.instance.playerMove.playerSkill != null &&
			   GameRuleManager.instance.playerMove.playerSkill is PlayerSkill_Invisible && 
			   GameRuleManager.instance.playerMove.playerSkill.isActivated == true) return; //은신 스킬을 쓰고 있으면 플레이어를 감지 하지 않습니다. 
			if (enemyMovement.isPlayerOnTheHead == true) return;

			Vector3 playerPos = GameRuleManager.instance.player.transform.position;

			if(sameLevelDetection)
			{
				playerPos = new Vector3(GameRuleManager.instance.player.transform.position.x, this.transform.position.y, GameRuleManager.instance.player.transform.position.z);
				//플레이어의 높이가 적과 동등한 높이내에서 감지 되어야하만 플레이어를 감지 하도록 합니다. 
			}

			Vector3 eyeLevelPos = new Vector3(transform.position.x, transform.position.y + enemyMovement.GetMyCollider().height * .25F , transform.position.z); //
			//Raycast를 그냥 진행하면 transform.position의 특성상 적의 발바닥에서 레이를 발사하기 때문에 collider 크기의 25% 높이에서 레이를 발사하도록 합니다.  
			
			Vector3 dir = playerPos - this.transform.position; //플레이어 위치에서 현위치를 빼면 현위치에서 플레이어 위치까지의 방향 벡터가 나옴 

			normVisionVector = dir.normalized; //시야의 방향 

			if(enemyMovement.eAIStatus == E_AI_STATUS.PATROL || enemyMovement.eAIStatus == E_AI_STATUS.IDLE) //패트롤 중인경우, 시야 각 안에 들어오는지를 체크합니다. 
			{
				VisionAngleCalculation(dir, eyeLevelPos);
			}

			if(enemyMovement.eAIStatus == E_AI_STATUS.CHASE) //추격중인경우, 시야 각 안에 들어오는지 체크하지 않고, 
			{
				if(enemyMovement.GetWeapon() != null && !string.IsNullOrEmpty(enemyMovement.GetWeapon().projObjStr))
				{
					//원거리 공격을 할 수 있는 경우 플레이어를 바로 감지한 것으로 판정하고, 
					playerInSight = true;

				} else {
					//아닌경우엔 길찾기 알고리즘을 한후 감지를 판단합니다. 
					NavMeshCalculation();

				}
			}
		
			if(playerInSight) //이러한 판단으로 플레이어 감지가 완료되면, 
			{
				if (enemyMovement.eMonsterType.CompareTo(E_MONSTER_TYPE.CANNON) != 0) {

					distance = Vector3.Distance(GameRuleManager.instance.player.transform.position, eyeLevelPos); //플레이어와의 거리계산 
					
					if(dir != Vector3.zero) //플레이어로의 방향 벡터가 0이 아닌경우 해당 방향으로 적의 몸을 회전함 
					{
						enemyMovement.transform.rotation = Quaternion.LookRotation (dir); //적을 부드럽게 돌리는것은 이 라인을 수정하면 될것으로 보입니다. 
					}
					
					lastPlayerSighted = col.transform.position; //마지막으로 플레이어를 본 위치 업데이트 
					
					Debug.DrawLine(transform.position, GameRuleManager.instance.playerMove.transform.position, Color.green);
				} 
			}
		}
	}

	void VisionAngleCalculation(Vector3 dir, Vector3 eyeLevelPos) //시야 감지 
	{
		float angle = Vector3.Angle(dir, transform.forward);	//적기준 정면 방향과 플레이어 위치를 가르키는 방향 벡터 사이의 각을 구하면 

		if(angle < viewAngle * 0.5f) //정면 기준으로 딱 반값의 각이 나오기 때문에 구해진 값이 설정한 시야각의 반보다 작은 경우 
		{
			RaycastHit hit; //적의 정면 시야에 들었다고 할 수 있다. 
			Ray rayOfSight = new Ray(eyeLevelPos , normVisionVector); //조정된 위치에서 플레이어 방향으로 레이 생성 

			int detectLayerUsed = playerCollisionLayer; //레이캐스트에 사용되는 기본 레이어는 플레이어 레이어 + 컬리젼 (지형) 
			
			if(canSeeThroughWalls) //벽 너머로도 플레이어를 감지 할수 있는 경우, 
			{
				detectLayerUsed = playerLayerOnly; //레이캐스트에 사용하는 레이어를 플레이어 레이어로만 국한합니다. 
			}
			
			if(Physics.Raycast(rayOfSight, out hit, visionCollider.radius, detectLayerUsed)) //시야 범위내에서 레이를 플레이어 방향으로 발사 했을때 
			{
				if(hit.collider.gameObject == GameRuleManager.instance.player) //플레이어가 적중하면 
				{
					playerInSight = true; //플레이어를 감지하였다. 
				}
			}
		}
	}

	void NavMeshCalculation() //길찾기 감지 
	{
		UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath(); //길찾기를 합니다. 
		enemyMovement.GetMyNav().CalculatePath(GameRuleManager.instance.playerMove.transform.position, path);
		
		if (path.status == UnityEngine.AI.NavMeshPathStatus.PathComplete) //적이 NavMesh상으로 도달 할수 있는 위치인가 체크 
		{
			playerInSight = true; //참이면, 플레이어를 감지하였다. 
			
		} else {
			Debug.DrawLine(transform.position, GameRuleManager.instance.playerMove.transform.position, Color.red);
		}
	}

	void OnTriggerExit(Collider col) //무언가가 시야에서 벗어남 
	{
		if (col.gameObject == GameRuleManager.instance.player) //그것이 플레이어인경우 
		{
			//플레이어가 시야에서 없어졌다. 
			playerInSight = false;
		}
	}

	public void ChangeVisionRadius(E_AI_STATUS eStatus)
	{
		switch(eStatus)
		{
			case E_AI_STATUS.CHASE:
				visionRadius = chaseVision;
			break;

			case E_AI_STATUS.PATROL:
				visionRadius = patrolVision;
			break;
		}
	}
}
