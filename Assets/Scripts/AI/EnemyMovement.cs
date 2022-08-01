using UnityEngine;
using System.Collections;

public enum E_AI_STATUS //현재 적의 상태를 3가지 상태로 나눕니다. 
{
	ATTACK, //공격중 
	CHASE, //추격중 
	PATROL, //순찰중 
	FLEE, //도망중 
	IDLE, //아무것도 하지 않음 
}

public enum E_MONSTER_TYPE //현재 적의 상태를 3가지 상태로 나눕니다. 
{
	UNIDENTIFIED = 0,
	MO = 1,
	BO,
	SPINE_MO,
	CANNON,
	REMAINS_MO,
	FACTORY_MO
}

public class EnemyMovement : MonoBehaviour {

	new public Transform transform;

	public E_MONSTER_TYPE eMonsterType = E_MONSTER_TYPE.UNIDENTIFIED;
	public string monsterID;
	public E_AI_STATUS eAIStatus; //적의 현재 스테이터스. 위 E_AI_STATUS 참조 

	public bool isImmortal = false;
	public float health = 1F;
	public bool isInvincible = false;
	public bool isPlayerOnTheHead = false;

	private EnemyVision vision;
	private PatrolRoute patrolRoute;

	private EnemyWeapon weapon;

	private CapsuleCollider myCollider;
//	private Rigidbody myRigidbody;

	private Animator animator;
	private AnimatorControllerParameter[] animParameter;

	private UnityEngine.AI.NavMeshAgent myNav;
	public float attackRange; //적이 무기를 달지 않았을시의 공격 범위. 
	public bool canMoveAttack = false; //적이 이동하면서 공격이 가능한가 

	private float modifiedSpeed = -999F;
	public float currentSpeed;

	public float chaseSpeed; //추격시의 속도 
	public float patrolSpeed; //순찰시의 속도 

	public float aggroWaitTime = 1F; //적을 찾았을때 다음 동작까지 기다리는 시
	public float chaseWaitTime; //추격이 끝나고 나서 다음 동작까지 기다려야 하는 시간 
	public float patrolWaitTime; //순찰이 끝나고 나서 다음 동작까지 기다려야 하는 시간 
	private float deathWaitTime = 0F;
	public float stunWaitTime; //기절하고 나서 기다리는 시간 
	public float stunInterval;
	public float invincibleTime = 1F;

	private float chaseTimer; //각종 타이머들 (수정하지 말것) 
	private float patrolTimer;
	[SerializeField]
	private float attackTimer;
	private float stunTimer;
	private float invincibleTimer;

	public float stuckCheckTime;
	private float stuckIntervalTimer;
	public int stuckTriesLimit; //끼임 상태 기다리는 시간 간
	private int stuckTries; // 이동중 장시간 끼었을때 작동하는 타이머

	private bool isStunned = false; //현재 기절중인지 아닌지 표시해주는 값. 

//	private float remainingDist;
	private float stoppingDist;
	private Vector3 destination;
	private float prevRemainingDist;

	public bool autoColliderSetting = true; //캐릭터 모델에 맞게 자동으로 컬라이더 값을 수정해주는 값입니다. 건드리지 말것. 
	public bool autoVariableSetting = true; //이 값이 켜져있으면 추격속도/순찰속도/각종 대기시간이 자동으로 세팅됩니다. 이 값을 꺼줘야 위의 값을 따로 수정이 가능 

	[SerializeField]
	private float aggroTimer;
	private bool isAggroed = false; 

	private bool isLost = false;

	public string dropObjStr;
//	public GameObject gemIndicator;

	public GameObject dropFxObj = null;
	public bool hasDroppable = true;
	public int dropCount = 3;

	private float objDropForce = 270F;

	public Animation foundAni;
	public Animation lostAni;

	void Awake()
	{
		transform = GetComponent<Transform>();
		myCollider = GetComponent<CapsuleCollider>(); //AI의 모델 충돌 박스 

//		myRigidbody = GetComponent<Rigidbody>(); //충돌을 돕기 위한 물리 강체 

		if(autoColliderSetting == true)
		{
			myCollider.center = new Vector3 (0F, .25F, 0F); //컬라이더 크기 설정 
			myCollider.radius = .15F;
			myCollider.height = .5F;
		}

		vision = transform.Find("VisionCollider").GetComponent<EnemyVision>();

		patrolRoute = GetComponent<PatrolRoute>(); //순찰 루트 설정 

		weapon = GetComponent<EnemyWeapon>(); //무기 설정 

		myNav = GetComponent<UnityEngine.AI.NavMeshAgent>(); //AI 네비게이션 설정 

		if(animator == null)
			animator = GetComponent<Animator>(); //AI의 애니메이션을 플레이 하기 위한 애니메이터 설정 

		if (animator == null)
			animator = this.transform.GetComponentInChildren<Animator> ();

		if (animator != null) 
		{
			animParameter = animator.parameters;

			AnimatorEventCaller animEventCaller = animator.gameObject.GetComponent<AnimatorEventCaller>();

			if(animEventCaller == null)
				animator.gameObject.AddComponent<AnimatorEventCaller> ();

			stunnedBody = animator.GetComponentsInChildren<SkinnedMeshRenderer>();

			string artRsrcName = animator.gameObject.name; //프리팹에 부착된 아트 프리팹의 명칭속 아이디 번호로 몬스터 종류를 판단합니다. 

			int[] monsterEnums = System.Enum.GetValues(typeof(E_MONSTER_TYPE)) as int[];

			if (eMonsterType.CompareTo(E_MONSTER_TYPE.UNIDENTIFIED) == 0) {
				for(int i=0; i<monsterEnums.Length; i++)
				{
					monsterID = monsterEnums[i].ToString("000");

					if(artRsrcName.Contains(monsterID))
					{
						eMonsterType = (E_MONSTER_TYPE)monsterEnums[i];
						break;
					}
				}
			}

		}

		if(attackRange == 0)
			attackRange = 2F; //기본 공격범위는 2 (무기가 없을시엔) 
	}

	// Use this for initialization
	void Start () 
	{

		if(autoVariableSetting == true)
		{
			//이 부분은 기획자가 값을 손대야 할경우 아래 값을 주석처리하고 인스펙터에서 원하는 값을 입력하면 됩니다 
			chaseWaitTime = patrolWaitTime = stunWaitTime = 3F; //추적시와 순찰시의 대기 시간은 3초로 동일 
			deathWaitTime = 1F;
			chaseSpeed = patrolSpeed = 1F; //추적시와 순찰시의 이동속도는 1로 동일 

			stuckTriesLimit = 1;
			stuckCheckTime = 1F;
			stunInterval = .05F;
		}

		patrolTimer = patrolWaitTime; //최초에는 바로 패트롤 모드로 돌입해야 하므로 
		chaseTimer = chaseWaitTime;
		invincibleTimer = invincibleTime;

		AssignAttackRange ();

	}

	void AssignAttackRange()
	{
		if(weapon != null) //무기가 있으면 공격 범위를 무기의 공격범위로 치환 
			attackRange = weapon.attackRange;
		
		if(attackRange > vision.visionCollider.radius || attackRange <= 0F) //공격 범위는 시야 범위를 넘을수 없다. 
		{
			attackRange = vision.visionCollider.radius;
		}
	}

	public EnemyWeapon GetWeapon()
	{
		return this.weapon;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (GameRuleManager.instance == null)
			return;

		if(GameRuleManager.instance.eGameStatus == E_GAME_STATUS.GAME_READY) return;

		if(weapon != null)
		{
			if(attackTimer >= 0F)
			{
				attackTimer -= Time.deltaTime;
			}
		}

		if(isStunned)
		{
			myNav.speed = 0F; //스턴시 이동속도 0 
			return;
		}

		if(vision.playerInSight) //적이 시야에 보인다 
		{
			Chase(); //추적 

			AssignAttackRange();

			if(vision.distance <= attackRange) //공격 범위 내에 플레이어가 들어왔을경우 
			{
				if (eMonsterType.CompareTo(E_MONSTER_TYPE.CANNON) == 0) {

					Vector3 direction = weapon.fireTrans.TransformDirection (Vector3.forward);

					Attack(direction); // 공격 
				} else {
					Attack(vision.normVisionVector); // 공격 
				}
			}

		} else {

			if(vision.lastPlayerSighted != vision.unreachablePos) //적이 보이지 않더라도 일단 마지막 발견된 위치까지는 가본다 
			{
				Chase();
			} else { //발견된 위치 자체가 없으면

				if(eAIStatus != E_AI_STATUS.IDLE)
				{
					Patrol(); //패트롤 
				}
			}
		}

		if(weapon != null)
		{
			if(canMoveAttack == false) //이동하면서 공격이 불가능한경우, 적의 도망 방향을 추적하지만 이동하지는 않습니다. 
			{
				if(attackTimer >= 0F)
				{
					SetCurrentSpeed(0F);
				}
			}
		}

		if(modifiedSpeed == -999F)
		{
			myNav.speed = currentSpeed;
		} else {
			myNav.speed = modifiedSpeed;
		}

		if (ContainsParam ("Speed"))
			animator.SetFloat ("Speed", myNav.speed); //Mathf.Sqrt(Mathf.Abs(myNav.velocity.x) + Mathf.Abs(myNav.velocity.z)));
		//애니메이터에 속도를 반영, 속도에 맞는 애니메이션 재생 

//		remainingDist = myNav.remainingDistance;
		stoppingDist = myNav.stoppingDistance;
		destination = myNav.destination;
	}

	public bool ContainsParam(string _ParamName)
	{
		if (animParameter == null) {
			return false;
		}

		for(int i=0; i< animParameter.Length; i++)
		{
			if(animParameter[i].name.Equals(_ParamName))
				return true;
		}

		return false;
	}

	void Chase()
	{
		eAIStatus = E_AI_STATUS.CHASE;

		vision.ChangeVisionRadius(eAIStatus);

		SetCurrentSpeed(chaseSpeed); //일단은 추격이니까 설정된 추격속도에 맞추지만, 아래 상황에 따라 추격모드시의 이동 속도가 바뀌게 됩니다. 

		if(vision.playerInSight) //적이 시야에 보이면 
		{
			isLost = false; //적이 보이면 적을 찾았기 때문에 더이상 두리번 거리지 않습니다. 

			if(ContainsParam("LostTime"))
				animator.SetFloat("LostTime", 0F); //본래 chaseTimer가 다 감소할때까지 두리번 애니메이션을 반복하지만, 강제로 종료 시킵니다. 

//			StopLostAnimation(); //? 애니메이션도 급종료. 

			if (eMonsterType.CompareTo(E_MONSTER_TYPE.REMAINS_MO) == 0 && isPlayerOnTheHead == true) {
				SetCurrentSpeed(patrolSpeed);
//				vision.visionCollider.enabled = false;
//				myNav.enabled = false;
//
//				Vector3 moveDirection = this.transform.position + vision.normVisionVector * 0.5f; 	// 0.5m 앞방향 ray
////				Ray testRay = new Ray(nextVector, Vector3.up * 2);
////				Debug.DrawRay (testRay.origin, testRay.direction * 3, Color.magenta, 5);
//
//				RaycastHit groundHit;
//				bool isHit = Physics.Raycast (moveDirection, Vector3.down, out groundHit, 1F, LayerMask.NameToLayer("collision"));
//                if (isHit) {
//
////					Ray testRay = new Ray(groundHit.point, Vector3.up * 2);
////					Debug.DrawRay (testRay.origin, testRay.direction * 3, Color.magenta, 5);
//
//					this.transform.Translate(vision.normVisionVector * Time.deltaTime * patrolSpeed, Space.World);
//				}
//
//				return;

			} else {
				vision.visionCollider.enabled = true;
				myNav.enabled = true;
			}

			if(myNav.destination != vision.lastPlayerSighted)
				myNav.SetDestination(vision.lastPlayerSighted); //네비게이션의 최종 목적지는 일단 마지막으로 플레이어를 목격한 위치 (Vision을 통해 매 프레임 마다 업데이트 됨) 

			if(isAggroed == false) //최초 적 발견 시에는 
			{
				if(SoundBoard.instance != null)
				{
					SoundBoard.instance.PlayFromSoundBoard("SND_MON_FIND", GameRuleManager.instance.playerMove.transform.position);
				}

				if (animator != null) {
					animator.SetTrigger("FoundEnemyTgr"); //어그로 애니메이션 플레이 
				}
//				aggroTimer = animator.getstate(0).length; //어그로 애니메이션의 길이 만큼 대기해야함 
				aggroTimer = 0F;
				

//				StartCoroutine("PlayFoundAnimation");		// Mentis 0000010 Issue

				isAggroed = true; //최초 발견이 아니므로 이제 여기로 다시 들어오지 않음 
//				attackTimer = aggroTimer; // 여기서 attackTimer를 0이하로 두면 바로 attackRate가 대입되면서 그만큼 기다려야하는 문제가 생김 
			}

			


		} else { 
			//적이 시야에 보이지 않는경우라도 최초에 SetCurrentSpeed(chaseSpeed)로 해놓았고 Destination도 lastPlayerSighted로 설정해놓았기 때문에 
			//일단 마지막으로 적이 보인 지점까지 이동합니다. 

			if(myNav.remainingDistance <= myNav.stoppingDistance)  //지점 도착 
			{
				LostSequence(); //플레이어 잃어버림 시퀀스 

				return;
			} else {

				//적이 안보이는 상태에서 남은 거리가 좁혀지지 않는경우를 대비해 끼임 방지 시퀀스 작동 
				StuckFreeSequence();

			}
		}

		if(aggroTimer > 0F) //적을 찾는 애니메이션을 할때는 움직이지 않습니다. 
		{
			aggroTimer -= Time.deltaTime;
			SetCurrentSpeed(0F);
		}

		if(isLost) //적을 놓쳐서 두리번 애니메이션을 할때는 움직이지 않습니다. 
		{
			SetCurrentSpeed(0F);
		}

	}

	void LostSequence() //플레이어를 잃어버림 시퀀스 
	{
		//attackRange //myNav.remainingDistance -그러다가 네비게이션상으로 최종목적지에 도달하기까지의 거리가 성공적인 공격범위 내에 들어와 있으면 
		chaseTimer -= Time.deltaTime; //추적종료 후 대기 타이머 작동 
		
		if(isLost == false) //처음 놓친 시점에 애니메이션 재생 
		{
			if(ContainsParam("LostEnemyTgr"))
				animator.SetTrigger("LostEnemyTgr");

//			StartCoroutine("PlayLostAnimation");	// Mentis 0000010 Issue
			isLost = true;
		}
		
		if(isLost) //놓친 상태에서 대기시간 애니메이션 트리로 전송 
		{
			if(ContainsParam("Lost"))
				animator.SetFloat("LostTime", chaseTimer);
		}
		
		if(chaseTimer <= 0F) //추적 대기 시간 만큼 기다린 다음 
		{
			vision.lastPlayerSighted = vision.unreachablePos; //적을 찾을수 없다로 판단 
			chaseTimer = chaseWaitTime; //추적 타이머 리셋 
			isLost = false; //다시 적을 최초로 놓칠수 있도록 설정. 그래야 다시 이동할수 있으므로. 
			
			if(aggroTimer <= 0F) //이 시점에 어그로 타이머도 0이하인경우 
			{
				isAggroed = false; //다시 최초 발견으로 설정합니다. 
			}

			stuckTries = 0;
		}
		
		SetCurrentSpeed(0F); //놓친 상황상태에서는 아무데도 이동하지 않습니다.  


	}

	void Flee()
	{

	}

	IEnumerator PlayFoundAnimation()
	{
		foundAni.gameObject.SetActive(true);
		foundAni.Play();

//		gemIndicator.SetActive(false);

		yield return new WaitForSeconds(foundAni.clip.length);

		foundAni.Stop();
		foundAni.gameObject.SetActive(false);

//		if(hasGem)
//			gemIndicator.SetActive(true);

	}

	IEnumerator PlayLostAnimation()
	{
		lostAni.gameObject.SetActive(true);
		lostAni.Play();

//		gemIndicator.SetActive(false);
		
		yield return new WaitForSeconds(lostAni.clip.length);
		
		lostAni.Stop();
		lostAni.gameObject.SetActive(false);

//		if(hasGem)
//			gemIndicator.SetActive(true);
	}

	public void StopLostAnimation() //헤멤 애니메이션 즉시 중지 함수 
	{
		StopCoroutine("PlayLostAnimation");
		lostAni.Stop();
		lostAni.gameObject.SetActive(false);
	}

	void Attack(Vector3 shootDir)
	{
		if(weapon == null) return; //무기가 없으면 공격 못합니다. 

		eAIStatus = E_AI_STATUS.ATTACK;

		weapon.Attack(shootDir); //가지고 있는 무기로 설정된 방향으로 공격 감행 
	}

	void Patrol()
	{
		eAIStatus = E_AI_STATUS.PATROL;

		vision.ChangeVisionRadius(eAIStatus);

		if(patrolRoute == null) return;
		if(patrolRoute.routes == null) return;
		if(patrolRoute.routes.Length == 0) return; //순찰 루트가 설정되어 있지 않으면 순찰하지 않습니다. 
		if(patrolRoute.routes[0] == null) return;
		if(patrolRoute.routes[0].routeTrans == null) return;

		SetCurrentSpeed(patrolSpeed);

		if(patrolRoute.routes[patrolRoute.routeIndex].routeSpeed > 0F)
			SetCurrentSpeed(patrolRoute.routes[patrolRoute.routeIndex].routeSpeed);

//		myNav.speed = patrolSpeed;

		//네비게이션상으로 설정된 다음 종착지까지 남은 거리가 멈춤 거리보다 작은 경우 (즉 사실상 거의 다 도착한경우) 
		if(myNav.remainingDistance <= myNav.stoppingDistance || myNav.destination == vision.unreachablePos)
		{
			//순찰 타이머가 대기시간을 초과하는경우 
			if(patrolTimer >= patrolWaitTime)
			{
				ProceedToNextPatrolPoint(); //다음 패트롤 포인트로 이동 

			} else {
				// 순찰 타이머 작동 
				patrolTimer += Time.deltaTime;
				SetCurrentSpeed(0F);
			}
		}
		else {
			// 아직 다음 순찰 포인트에 가까워지지 않은경우 타이머는 작동하지 않는다 
			patrolTimer = 0;

			StuckFreeSequence(); //혹시모를 끼임 방지 시퀀스를 가동한다 
		}
		
		// 네비게이션을 다음 순찰 포인트로 설정한다 
		if(myNav.destination != patrolRoute.routes[patrolRoute.routeIndex].routeTrans.position)
			myNav.SetDestination(patrolRoute.routes[patrolRoute.routeIndex].routeTrans.position);
	}

	void ProceedToNextPatrolPoint()
	{
		if(patrolRoute.routeIndex == patrolRoute.routes.Length - 1) //그냥 다음 패트롤 포인트로 이동 
		{
			//배열상의 인덱스를 초과하는경우 0번의 순찰포인트로 설정 
			patrolRoute.routeIndex = 0;
		} else {
			//다음 순서 
			patrolRoute.routeIndex++;
		}

		// 타이머 리셋 
		patrolTimer = 0F;
		stuckTries = 0; //정상적으로 이동중일때는 누적중인 끼임 방지 타이머를 언제나 리셋 시킨다. 
	}

	void StuckFreeSequence() //끼임 방지 시퀀스 
	{
		if(stuckTries > stuckTriesLimit) //끼여있는 시간이 3초이상 누적되면 
		{
			switch(eAIStatus)
			{
				case E_AI_STATUS.PATROL: //패트롤 상태에서 끼임 방지 시퀀스가 작동했다면 

					ProceedToNextPatrolPoint(); //다음 패트롤 포인트로 이동 

				break;

				case E_AI_STATUS.CHASE: //추격 상태에서 끼임 방지 시퀀스가 작동했다면 
					LostSequence(); //이제 잃어버렸다. 
				break;

				default:
				break;
			} 
		} else {

			stuckIntervalTimer += Time.deltaTime;

			if(stuckIntervalTimer > stuckCheckTime)
			{
				stuckIntervalTimer = 0F;
				
				if(Mathf.Abs((myNav.remainingDistance - prevRemainingDist)) <= stoppingDist) //한프레임 사이간의 남아있는 거리가 모종의 이유로 더이상 나아가지 않을때 
				{
					stuckTries++; //끼임 방지 타이머 시간 누적 
				}
				
				prevRemainingDist = myNav.remainingDistance;
			}
		}

	}


	public void Stun(float damage = 0F, bool stomped = false) //외부에서 본 AI를 스턴시키는 함수 
	{
		if(isStunned == false && isInvincible == false) //이미 스턴중이면 누적 스턴 시키지 않는다. 
		{
			if(isImmortal == false && health > 0F) //캐릭터가 불사 캐릭터가 아니라면, 데미지를 입습니다. 
			{
				health -= damage;
			}

			if (eMonsterType.CompareTo(E_MONSTER_TYPE.FACTORY_MO) == 0) {
				vision.transform.gameObject.SetActive(false);
				GameObject contactCollider = transform.Find("MeleeContactCollider").gameObject;
				if (contactCollider != null) {
					contactCollider.SetActive(false);
				}
				vision.playerInSight = false;
				LostSequence(); //플레이어 잃어버림 시퀀스 

				Invoke("activeVision", stunWaitTime);

			} else {
				StartCoroutine("StartStun", stomped);
			}

			vision.lastPlayerSighted = vision.unreachablePos;

//			if(hasDroppable)
//			{
//				if((isImmortal == false && health <= 0F) || isImmortal == true) //불사이면 그냥 첫 데미지 때 코인 떨굼, 아닌경우 사망시 떨굼 
//				{
//					for(int i=0; i<dropCount; i++)
//					{
//						Vector3 droppingDirection = Random.insideUnitSphere; //this.transform.forward + this.transform.up;
//						StartCoroutine("DelayDropItem",droppingDirection);
//					}
//				}
//			}

			if(SoundBoard.instance != null)
				SoundBoard.instance.PlayFromSoundBoard("SND_MON_HIT", this.transform.position);
		}
	} 

	public void MonsterDeath() //데미지 입는 애니메이션에서 함수를 호출합니다. 
	{
		if(isImmortal == false && health <= 0F) //불사캐릭터가 아니고, 체력이 0이하가 되었다면 
		{
			if(SoundBoard.instance != null)
				SoundBoard.instance.PlayFromSoundBoard("SND_MON_DESPAWN", this.transform.position);

			if(hasDroppable)
			{
				if((isImmortal == false && health <= 0F) || isImmortal == true) //불사이면 그냥 첫 데미지 때 코인 떨굼, 아닌경우 사망시 떨굼 
				{
					this.gameObject.SetActive(false);
					Invoke("makeDropItems", 0.02F);
				}
			} else {

				Destroy (this.transform.gameObject); //캐릭터 삭제 
			}

		}
	}

	void activeVision()
	{
		vision.transform.gameObject.SetActive(true);
		GameObject contactCollider = transform.Find("MeleeContactCollider").gameObject;
		if (contactCollider != null) {
			contactCollider.SetActive(true);
		}
	}

	void makeDropItems() 
	{
		if (dropFxObj != null) {
			ResourcesManager.instance.PopEffect (dropFxObj, this.transform.position);
		} else {
			ResourcesManager.instance.PopEffect ("Fx_M_Die_01", this.transform.position);
		}
		
		for(int i=0; i<dropCount; i++)
		{
			Vector3 droppingDirection = Random.insideUnitSphere; //this.transform.forward + this.transform.up;
			DropItem(droppingDirection);
		}
	}

	void DropItem(Vector3 droppingDirection) //아이템 드롭 함수 
	{
//		gemIndicator.SetActive(false);

		hasDroppable = false;

		GameObject dropItemObj = ResourcesManager.instance.LoadGameObject (dropObjStr) as GameObject;

		if (dropItemObj != null) 
		{

			GameObject dropItem = GameObject.Instantiate (dropItemObj, this.transform.position + this.transform.up, Quaternion.identity) as GameObject;
		
			Rigidbody dropRigid = dropItem.GetComponent<Rigidbody> ();

			if (dropRigid != null) {
				dropRigid.AddForce (droppingDirection * objDropForce);
			}

			Destroy (this.transform.gameObject); //캐릭터 삭제 
		}
	}

	public SkinnedMeshRenderer[] stunnedBody; //스턴될때 깜박일 캐릭터의 스킨 렌더러 
	IEnumerator StartStun(bool stomped)
	{
		isStunned = true; //스턴 시작 
		isInvincible = true;

		if (stomped) 
		{
			animator.SetTrigger("JumpDamagedTgr");
		} else {
			animator.SetTrigger("DamagedTgr");
		}
		
		stunTimer = stunWaitTime;

		if (health < 1) //적에게 치명타 였을 경우 
		{
			stunTimer = 0F;	//deathWaitTime; //설정된 스턴 시간을 설정하지 않고 죽음 대기 시간으로 설정 
		}

		float stunIntervalChecker = 0F;
		
		while(stunTimer > 0F)
		{
			stunTimer -= Time.deltaTime;
			stunIntervalChecker += Time.deltaTime;

			animator.SetFloat ("StunTime", stunTimer);

			if(stunIntervalChecker > stunInterval)
			{
				for(int i=0; i<stunnedBody.Length; i++)
				{
					stunnedBody[i].enabled = !stunnedBody[i].enabled; //매 프레임마다 렌더러를 껐다 켰다 해준다. 
				}

				stunIntervalChecker = 0F;
			}
			
			yield return null;
		}

		if (health > 0) //치명타가 아니었을경우 스턴후 무적시간도 존재 
		{
            isStunned = false; //스턴이 풀렸음 
			invincibleTimer = invincibleTime; //스턴 이후 1초가량의 무적시간이 존재 소위 적이 트랩에 계속 "갈리는것"을 막아준다 

			while (invincibleTimer > 0F) {
				invincibleTimer -= Time.deltaTime;
				stunIntervalChecker += Time.deltaTime;

				if (stunIntervalChecker > stunInterval) {
					for (int i=0; i<stunnedBody.Length; i++) {
						stunnedBody [i].enabled = !stunnedBody [i].enabled; //매 프레임마다 렌더러를 껐다 켰다 해준다. 
					}

					stunIntervalChecker = 0F;
				}
			
				yield return null;
			}

			for (int i=0; i<stunnedBody.Length; i++) {
				stunnedBody [i].enabled = true; //다 끝나면 다 켜줌 
			}
		}

		isInvincible = false;
	}

	public void FootSound()
	{
//		if (SoundBoard.instance != null)
//			SoundBoard.instance.PlayFromSoundBoard ("SND_MON_" + monsterID + "_MOVE", this.transform.position);
	}

	#region GetFunctions
	public EnemyVision GetVision()
	{
		return this.vision;
	}

	public bool IsStunned()
	{
		return this.isStunned;
	}

	public CapsuleCollider GetMyCollider()
	{
		return this.myCollider;
	}

	public UnityEngine.AI.NavMeshAgent GetMyNav()
	{
		return this.myNav;
	}

	public float GetAttackTimer()
	{
		return this.attackTimer;
	}

	public Animator GetAnimator()
	{
		return this.animator;
	}

	public Vector3 GetDestination()
	{
		return this.destination;
	}
	#endregion

	#region SetFunctions
	public void SetAttackTimer(float timer)
	{
		this.attackTimer = timer;
	}

	public void SetCurrentSpeed(float speedValue)
	{
		this.currentSpeed = speedValue;
	}

	public void SetModifiedSpeed(float speed)
	{
		this.modifiedSpeed = speed;
	}

	public void SetAggroTimer(float aggroTimeValue)
	{
		this.aggroTimer = aggroTimeValue;
	}
	#endregion
}
