using UnityEngine;
using System.Collections;

public enum E_PLAYER_ATTACK_TYPE
{
	VULNERABLE,
	HOPNBOP,
	RUNNBUMP,
	RUNNGUN,
}

[RequireComponent (typeof (CharacterController))]
public class PlayerMoveCC : MonoBehaviour 
{
	private RecordGameplay replay;
	public CNJoystick cnJoystick;
	new public Transform transform;

	public E_PLAYER_ATTACK_TYPE ePlayerAtkType = E_PLAYER_ATTACK_TYPE.HOPNBOP;
	public float attackDamage = 1.0F;

	public float horz;
	public float vert;
	
	public bool isGrounded = false;

	private float speed_ = 4.5F;

	[SerializeField]
	private float currentSpeed;

	public float speed
	{
		get{
			currentSpeed = speed_;
//			if (PurchasedItemEffect.instance == null) {
//				currentSpeed = speed_;
//			} else {
//
//				currentSpeed = speed_ + (speed_ * PurchasedItemEffect.instance.GetItemEffectValue(PURCHASED_ITEM.SPEED_BOOST));
//			}
			return currentSpeed;
		}

		set{
			speed_ = value;
		}
	}

	private float _maxSpeed = 10F;

	public float maxSpeed
	{
		get {
			return _maxSpeed;
		}
	}

	public float jumpSpeed = 3F;
	public float jumpAccelRate = 2F;
	private float gravityReduceFactor = 0.3F;

	public bool canFly = false;
	public float doubleJumpSpeed = 2.5F;

	public float rotateSpeed = 0.9F;
	public float maxRotateSpeed = 1.0F;

	public float minRotateDegree = 10F;
	public float maxRotateDegree = 90F;

	public float idleLimitValue = 0.3F;
	public float maxIdleLimit = 1F;

	public float airIdleLimitValue = 0F;
	public float maxAirIdleLimit = 1F;

	public float moveSensitivity = 3F;

	public float maxSpeedReachPercentage = .6F;
	public float maxSpeedReachStandard = 1F;

	public float gravity = 9.87F;
	public float spdAtMaxHt = 1F;
	
	public Animator animator;
	public AnimatorMoveCaller moveCaller;
	public AnimatorEventCaller eventcaller;

	public Transform mainTransform;
	protected CharacterController controller;
	
	public bool useManualControlSettings = false;
	
	public Vector3 faceDirection = Vector3.zero;
	public Vector3 moveDirection = Vector3.zero;
	
	private SideScrollCamera sideCam;

	[SerializeField]
	private float currentY;

	public float savedJumpHeight;
	private float initialJumpY;
	public float fallDelta;

	public bool isAcceleRate = false;
	public float accelerationTime = 2.0f;

	public bool isKnockBack = false;
    public float knockBackTime = 0.5F;
	public bool blockMovement = false;

	public PlayerSkill playerSkill;
	public PlayerFX playerFX;

//	public float socialWaitTime = 10F;
//	public float socialTimer;
//	public bool socialPlaying = false;

	public float straightFactor = .8F; //수치가 낮을수록 더 빨리 직선 보정 

	public float accelWaitTime = 1F;
	public float accelInputTimer;

	public string groundName = string.Empty;

	void Awake()
	{
		transform = GetComponent<Transform>();

		if(animator == null)
			animator = GetComponent<Animator> ();

		if (animator == null)
			animator = this.transform.GetComponentInChildren<Animator> ();

		if (animator != null) 
		{
			moveCaller = animator.gameObject.GetComponent<AnimatorMoveCaller>();

			if(moveCaller == null)
				moveCaller = animator.gameObject.AddComponent<AnimatorMoveCaller> ();

			eventcaller = animator.gameObject.GetComponent<AnimatorEventCaller>();

			if(eventcaller == null)
				eventcaller = animator.gameObject.AddComponent<AnimatorEventCaller>();
		}
		
		controller = GetComponent<CharacterController> ();
		
		if (useManualControlSettings == false) 
		{
			controller.center = new Vector3 (0F, .25F, 0F); //33F
			controller.radius = .05F;
			controller.height = .5F;
		}

		playerFX = GetComponent<PlayerFX> ();

		if (playerFX == null)
			playerFX = transform.gameObject.AddComponent<PlayerFX> ();

		mainTransform = transform.parent.transform;
	}

	void OnDestroy()
	{
		GameRuleManager.instance.player = null;
		GameRuleManager.instance.playerMove = null;
		GameRuleManager.instance.playerHealth = null;
	}

	void Start () 
	{
//		ePlayerAtkType = E_PLAYER_ATTACK_TYPE.RUNNBUMP;
		GameRuleManager.instance.isPlayerScriptSuccess = false;

		cnJoystick = FindObjectOfType<CNJoystick>();

//		socialTimer = socialWaitTime;

		if (GameRuleManager.instance.playerMove == null)
			GameRuleManager.instance.playerMove = this;

		RegisterDefaultValues();
		LoadSavedValues();
	}

	public void RegisterDefaultValues()
	{
		if(PlayerPrefs.HasKey("DefaultSpeed"))
		{
			this.speed = PlayerPrefs.GetFloat("DefaultSpeed");
		} else {
			PlayerPrefs.SetFloat("DefaultSpeed", this.speed);
		}

		if(PlayerPrefs.HasKey("DefaultRotateSpeed"))
		{
			this.rotateSpeed = PlayerPrefs.GetFloat("DefaultRotateSpeed");
		} else {
			PlayerPrefs.SetFloat("DefaultRotateSpeed", this.rotateSpeed);
		}

		if(PlayerPrefs.HasKey("DefaultRotateAngle"))
		{
			this.minRotateDegree = PlayerPrefs.GetFloat("DefaultRotateAngle");
		} else {
			PlayerPrefs.SetFloat("DefaultRotateAngle", this.minRotateDegree);
		}

		if(PlayerPrefs.HasKey("DefaultIdleLimit"))
		{
			this.idleLimitValue = PlayerPrefs.GetFloat("DefaultIdleLimit");
		} else {
			PlayerPrefs.SetFloat("DefaultIdleLimit", this.idleLimitValue);
		}

		if(PlayerPrefs.HasKey("DefaultAirIdleLimit"))
		{
			this.airIdleLimitValue = PlayerPrefs.GetFloat("DefaultAirIdleLimit");
		} else {
			PlayerPrefs.SetFloat("DefaultAirIdleLimit", this.airIdleLimitValue);
		}

		if(PlayerPrefs.HasKey("DefaultMaxSpeedReach"))
		{
			this.maxSpeedReachPercentage = PlayerPrefs.GetFloat("DefaultMaxSpeedReach");
		} else {
			PlayerPrefs.SetFloat("DefaultMaxSpeedReach", this.maxSpeedReachPercentage);
		}

		if(PlayerPrefs.HasKey("DefaultInputMode"))
		{
			int boolInt = PlayerPrefs.GetInt("DefaultInputMode");

			UIManager.instance.optionPanel.eConfirmedInputMode = (INPUT_MODE)boolInt;

			switch((INPUT_MODE)boolInt)
			{
				case INPUT_MODE.FREE_JOYSTICK:
					cnJoystick.SnapsToFinger = true;
					cnJoystick.TouchZoneSize = new Vector2(14F, 16F);

					UIManager.instance.optionPanel.gamePadOverlay = false;
					UIManager.instance.optionPanel.gamePadSys.ToggleGamePadSprites(false);
				break;

				case INPUT_MODE.STATIC_JOYSTICK:
					cnJoystick.SnapsToFinger = false;
					cnJoystick.TouchZoneSize = new Vector2(5F, 5F);

					UIManager.instance.optionPanel.gamePadOverlay = false;
					UIManager.instance.optionPanel.gamePadSys.ToggleGamePadSprites(false);
				break;

				case INPUT_MODE.GAMEPAD:
					cnJoystick.SnapsToFinger = false;
					cnJoystick.TouchZoneSize = new Vector2(5F, 5F);

					UIManager.instance.optionPanel.gamePadOverlay = true;
					UIManager.instance.optionPanel.gamePadSys.ToggleGamePadSprites(true);
				break;
			} 
		} else {

			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				cnJoystick.SnapsToFinger = false;						// 아이폰에서 기본 pad는 static joystick
				UIManager.instance.optionPanel.gamePadOverlay = false;
			}

			if(cnJoystick.SnapsToFinger)
			{
				UIManager.instance.optionPanel.gamePadOverlay = false;
				UIManager.instance.optionPanel.gamePadSys.ToggleGamePadSprites(false);
				cnJoystick.TouchZoneSize = new Vector2(14F, 16F);

				PlayerPrefs.SetInt("DefaultInputMode", (int)INPUT_MODE.FREE_JOYSTICK);
				UIManager.instance.optionPanel.eConfirmedInputMode = INPUT_MODE.FREE_JOYSTICK;
			} else {

				if(UIManager.instance.optionPanel.gamePadOverlay)
				{
					UIManager.instance.optionPanel.gamePadSys.ToggleGamePadSprites(true);
					cnJoystick.TouchZoneSize = new Vector2(5F, 5F);

					PlayerPrefs.SetInt("DefaultInputMode", (int)INPUT_MODE.GAMEPAD);
					UIManager.instance.optionPanel.eConfirmedInputMode = INPUT_MODE.GAMEPAD;

				} else {

					UIManager.instance.optionPanel.gamePadSys.ToggleGamePadSprites(false);
					cnJoystick.TouchZoneSize = new Vector2(5F, 5F);

					PlayerPrefs.SetInt("DefaultInputMode", (int)INPUT_MODE.STATIC_JOYSTICK);
					UIManager.instance.optionPanel.eConfirmedInputMode = INPUT_MODE.STATIC_JOYSTICK;
				}
			}
		}
	}

	void LoadSavedValues()
	{
		if(PlayerPrefs.HasKey("Speed"))
		{
			this.speed = PlayerPrefs.GetFloat("Speed");
		}

		if(PlayerPrefs.HasKey("RotateSpeed"))
		{
			this.rotateSpeed = PlayerPrefs.GetFloat("RotateSpeed");
		}

		if(PlayerPrefs.HasKey("RotateAngle"))
		{
			this.minRotateDegree = PlayerPrefs.GetFloat("RotateAngle");
		}

		if(PlayerPrefs.HasKey("IdleLimit"))
		{
			this.idleLimitValue = PlayerPrefs.GetFloat("IdleLimit");
		}

		if(PlayerPrefs.HasKey("AirIdleLimit"))
		{
			this.airIdleLimitValue = PlayerPrefs.GetFloat("AirIdleLimit");
		}

		if(PlayerPrefs.HasKey("InputMode"))
		{
			int boolInt = PlayerPrefs.GetInt("InputMode");
			
			switch((INPUT_MODE)boolInt)
			{
			case INPUT_MODE.FREE_JOYSTICK:
				cnJoystick.SnapsToFinger = true;
				cnJoystick.TouchZoneSize = new Vector2(14F, 16F);

				UIManager.instance.optionPanel.gamePadOverlay = false;
				UIManager.instance.optionPanel.gamePadSys.ToggleGamePadSprites(false);
				break;
				
			case INPUT_MODE.STATIC_JOYSTICK:
				cnJoystick.SnapsToFinger = false;
				cnJoystick.TouchZoneSize = new Vector2(5F, 5F);

				UIManager.instance.optionPanel.gamePadOverlay = false;
				UIManager.instance.optionPanel.gamePadSys.ToggleGamePadSprites(false);
				break;
				
			case INPUT_MODE.GAMEPAD:
				cnJoystick.SnapsToFinger = false;
				cnJoystick.TouchZoneSize = new Vector2(5F, 5F);

				UIManager.instance.optionPanel.gamePadOverlay = true;
				UIManager.instance.optionPanel.gamePadSys.ToggleGamePadSprites(true);
				break;
			} 

			UIManager.instance.optionPanel.eConfirmedInputMode = (INPUT_MODE)boolInt;
		}
	}

	float timer;

	public Vector2 joystickVec;
	public float joystickMag;
	public bool onFreezeEvent = false;

	void Update()
	{
		if (animator == null)
			animator = GetComponent<Animator> ();
		
		if (animator == null)
			return;

		if(GameRuleManager.instance.eGameStatus == E_GAME_STATUS.IN_PLAY && !onFreezeEvent)
		{
			if(cnJoystick == null)
				cnJoystick = FindObjectOfType<CNJoystick>();
			
			if(cnJoystick == null) return;

			if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) 
			{
				if(UIManager.instance != null)
				{
					horz = Mathf.Clamp(cnJoystick.GetAxis("Horizontal") / cnJoystick.DragRadius, -1F, 1F);

					vert = Mathf.Clamp(cnJoystick.GetAxis("Vertical") / cnJoystick.DragRadius, -1F, 1F);
//					horz = UIManager.instance.posX;
//					vert = UIManager.instance.posY;
				}
			}
			else 
			{
				if(UIManager.instance != null)
				{
					horz = Mathf.Clamp(cnJoystick.GetAxis("Horizontal") / cnJoystick.DragRadius, -1F, 1F);

					if(horz == 0)
						horz = Input.GetAxis ("Horizontal");

					vert = Mathf.Clamp(cnJoystick.GetAxis("Vertical") / cnJoystick.DragRadius, -1F, 1F);

					if(vert == 0)
						vert = Input.GetAxis ("Vertical");
				}
			}

		} else {
			horz = 0F;
			vert = 0F;
		}

		if (Mathf.Abs(horz) > 0 || Mathf.Abs(vert) > 0) 
		{
			accelInputTimer += Time.deltaTime;

			if(accelInputTimer > accelWaitTime)
				accelInputTimer = accelWaitTime;

		} else {
			accelInputTimer = 0F;
		}

		if(isKnockBack == false && blockMovement == false)
		{
			joystickVec = new Vector2 (horz, vert);
			joystickMag = joystickVec.magnitude;
			//		AbsoluteFourDirectionMovement();

			joystickMag -= idleLimitValue;
			if (joystickMag > 0) {
				joystickMag += maxSpeedReachPercentage;
			}

			float tempIdleLimitValue = 0.0F;
			if(!controller.isGrounded) //is not Grounded
			{
				tempIdleLimitValue = airIdleLimitValue;
			}

			float calcRatio = joystickMag - tempIdleLimitValue;

			if(calcRatio < 0)
				calcRatio = 0F;

			float playerSpeed = Mathf.Clamp01(calcRatio / maxSpeedReachStandard);
//			Debug.Log (">>>>>>" + playerSpeed.ToString()  + "|" + calcRatio.ToString()+ "|" + joystickMag.ToString());

			animator.SetFloat("Speed", playerSpeed);
		}

		GroundCheck ();
		PlayerInput ();

		currentY = controller.transform.position.y;

//		SocialUpdate();

		SetRotation();
		SetDirection();
		JumpGravityControl ();

		throwingVector = Vector3.Lerp (throwingVector, Vector3.zero, Time.deltaTime);
	}

	void AbsoluteFourDirectionMovement() //운전자 미숙 직선 방지 코드 
	{
		float absHorz = Mathf.Abs(horz);
		float absVert = Mathf.Abs(vert);

		if((absHorz - absVert) >= straightFactor)
		{
			vert = 0F; //Mathf.Lerp(vert, 0F, Time.deltaTime);
		}

		if((absVert - absHorz) >= straightFactor)
		{
			horz = 0F; //Mathf.Lerp(horz, 0F, Time.deltaTime);
		}
	}

	void FixedUpdate()
	{
		if (!isGrounded && controller.isGrounded) //이전 프레임이 그라운드 상태가 아니었으나 이번 프레임에 그라운드가 되는경우 
		{
            if (currentY > transform.position.y)
            {
				if (groundName.Contains("Cloud")) {
					playerFX.PopEffect("Fx_Landing_02"); //Cloud 착지 효과 표시 
				} else {
					playerFX.PopEffect("Fx_Landing_01"); //착지 효과 표시 
				}
                
                playerFX.ToggleOffMoveFX();

                if (SoundBoard.instance != null)
                    SoundBoard.instance.PlayFromSoundBoard("SND_PC_LANDING", this.transform.position);
            }
		}

		isGrounded = controller.isGrounded;

		if (isGrounded) {
			throwingVector = Vector3.zero;
			jumpCount = 0;
		}
	}

//	void SocialUpdate()
//	{
//
//		if(horz != 0F || vert != 0F || UIManager.instance.buttonPressed)
//		{
//			socialTimer = socialWaitTime;
//			socialPlaying = false;
//		}
//
//		if(socialPlaying) return;
//
//		if(socialPlaying == false)
//		{
//			if(socialTimer > 0F)
//				socialTimer -= Time.deltaTime;
//		}
//
//		if(socialTimer < 0F)
//		{
//			int rand = Random.Range(0,2);
//
//			if(rand == 1)
//			{
//				animator.SetTrigger("Social2Tgr");
//			} else {
//				animator.SetTrigger("Social1Tgr");
//			}
//
//			socialTimer = socialWaitTime;
//			socialPlaying = true;
//		}
//	}

//	public void SocialFinished()
//	{
////		Debug.Log("SocialFinished");
//		socialPlaying = false;
//	}

	public CharacterController GetController()
	{
		return this.controller;
	}

	private RaycastHit hit;

	public RaycastHit GetCurrentRaycastInfo()
	{
		return this.hit;
	}

	public float groundDist;
	private bool isOnPlatform;

//	[SerializeField]
	private float platformDetectFactor = 0.3F;
	void GroundCheck()
	{
		fallDelta = controller.transform.position.y - currentY; //플레이어가 위아래로 움직이는 속도 

		Vector3 playersDownVector = controller.transform.TransformDirection(-Vector3.up);

		if (Physics.SphereCast (this.controller.transform.position + this.controller.center, this.controller.radius, playersDownVector, out hit)) 
		{
			groundDist = Vector3.Distance (this.controller.transform.position, hit.point);
			if (hit.collider.CompareTag ("MovingPlatform")) 
			{
				if (groundDist <= platformDetectFactor)
				{
					//SetDirection에서 isOnPlatform은 controller.isGrounded와 같은 취급을 해준다. 
					//따라서 isOnPlatform이 되었을때는 moveDirection.y가 유지되는것이 아니라 0으로 초기화 된다. 
					//그래서 isGrounded에서는 moveDirection.y가 -gravity * Time.deltaTime을 유지한다. 
					//그러나 controller.isGrounded와는 별개로 platformDetectFactor에 따라 이동 플랫폼을 미리 감지하므로 
					//아래 코드처럼 한프레임 가량의 중력을 가해주지 않으면 이동 플랫폼 착지시 슬로우 모션이 생겨 버린다. 
					if(!isOnPlatform) 
					{
						Vector3 platformStickVec = Vector3.zero;
						platformStickVec.y = -gravity * Time.deltaTime;
						controller.Move(platformStickVec);
					}

					isOnPlatform = true;

					//무빙 플랫폼에 착지했을때는 parenting을 
					this.transform.SetParent(hit.collider.gameObject.transform);
					this.transform.eulerAngles = new Vector3 (0F, this.transform.eulerAngles.y, 0F);


				} else {
					isOnPlatform = false;
					this.transform.SetParent(mainTransform);
				}



			} else {
				isOnPlatform = false;
				this.transform.SetParent(mainTransform);
			}
		} else {
			isOnPlatform = false;
			this.transform.SetParent(mainTransform);

		}

		animator.SetBool ("isGrounded", controller.isGrounded || isOnPlatform);
	}

	public int jumpCount = 0;
	void PlayerInput()
	{
		if (onFreezeEvent)
			return;

		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) 
		{
			if(UIManager.instance != null)
			{
				if (UIManager.instance.buttonPressed) 
				{
					Jump(); 
				}

//				if(UIManager.instance.specButtonPressed)
//				{
//					if(playerSkill != null)
//					{
//						playerSkill.Activate();
//					}
//				}
			}

		} else {

			if(UIManager.instance.GetRRect().Contains(Input.mousePosition))
			{
				if (Input.GetButtonDown ("Fire1") || Input.GetButtonDown ("Jump")) 
				{
					Jump();
				}

			} else
			if(Input.GetKeyDown(KeyCode.Space))
			{
				Jump();
			}

//			if(UIManager.instance.specButtonPressed)
//			{
//				if(playerSkill != null)
//				{
//					playerSkill.Activate();
//				}
//			}
		}
	}

	public float accelBonusRate = .2F;
	public void OnPlayerMove()
	{
		float accelBonus = 0F;
		if (accelInputTimer >= accelWaitTime) {
			accelBonus = speed * accelBonusRate;
		}

		controller.Move (moveDirection * Time.deltaTime * (speed + accelBonus));
	}

	void SetRotation()
	{
		int frameSkip = (int)((maxRotateSpeed - rotateSpeed) * 10F) + 1;

//		Debug.Log(Time.frameCount + " / "  + frameSkip + " / " + (Time.frameCount % frameSkip != 0));
		if(Time.frameCount % frameSkip != 0) return;

		faceDirection = new Vector3 (horz, 0, vert);
		
		if(GameRuleManager.instance.sideCam.moveStandardObj != null)
		{
			faceDirection = GameRuleManager.instance.sideCam.moveStandardObj.transform.TransformDirection(faceDirection);
		} else {
			faceDirection = GameRuleManager.instance.sideCam.transform.TransformDirection(faceDirection);
		}

		faceDirection = new Vector3(faceDirection.x, 0F, faceDirection.z).normalized;
		
		if(faceDirection != Vector3.zero)
			transform.rotation = Quaternion.LookRotation (faceDirection);

		ForceFaceRotationToDegreeInterval(minRotateDegree);
	}

	void ForceFaceRotationToDegreeInterval(float degreeInterval)
	{
		if(degreeInterval == 0F) return;

		Vector3 playerEuler = transform.eulerAngles;

		float newY = Mathf.Round(playerEuler.y / degreeInterval) * degreeInterval;

		transform.eulerAngles = new Vector3(transform.eulerAngles.x, newY, transform.eulerAngles.z);
	}

	void SetDirection()
	{
		Vector3 normMoveDirection = controller.transform.forward.normalized;

		//isOnPlatform (무빙 플랫폼에 올라섬)을 controller.isGrounded와 동급 취급을 해준다. 
		//이는 플레이어가 이동 플랫폼에 올라선 상태에서 플레이어의 입력과는 상관없이 벽등을 통해 낭떠러지로 밀려날경우, 
		//controller.isGrounded는 이미 isGrounded가 false라 판정하여 중력이 누적된다. 
		//이렇게되면 이동 플랫폼이 비로소 플레이어의 추락을 가로막지 않게 되었을때 급작스럽게 하강하는 문제가 발생하므로 
		//동급취급을 해주었다. 
		if((controller.isGrounded || isOnPlatform) && savedJumpHeight <= 0F) //isGrounded
		{
			moveDirection = normMoveDirection * (Mathf.Clamp01(animator.GetFloat ("Speed") * moveSensitivity));

			if(moveDirection != Vector3.zero) //지면일때 움직임이 존재하면 걷는 효과 넣음 
			{
				playerFX.ToggleOnMoveFX(this.groundName);
			} else {
				playerFX.ToggleOffMoveFX(); // 지면이더라도 움직임이 없으면 걷는 효과 끔 
			}

		} else {

			moveDirection = new Vector3 (normMoveDirection.x * animator.GetFloat ("Speed"),
			                             moveDirection.y,
			                             normMoveDirection.z * animator.GetFloat ("Speed"));

			playerFX.ToggleOffMoveFX(); //공중일땐 움직임과 상관없이 걷는 효과 없음 
		}
	}

	void JumpGravityControl()
	{
		if (savedJumpHeight > 0F) 
		{
			if (currentY >= (savedJumpHeight + initialJumpY) 
			    || controller.collisionFlags == CollisionFlags.Above
			    || controller.collisionFlags == CollisionFlags.CollidedAbove
			    || (int)controller.collisionFlags == 3) //왜 3인지는 모르겠다 ㅡㅡ;;; 
			{
				moveDirection.y = spdAtMaxHt;
				savedJumpHeight = 0F;
			}

		} else {
			
			if(antiGravityHeightLimit > 0F)
			{
				moveDirection.y = Mathf.Lerp(appliedGravity, 0F, currentY / antiGravityHeightLimit);
				
			} else {

				moveDirection.y -= gravity * Time.deltaTime; 
			}
		}
		
		moveDirection += throwingVector;
	}

	[SerializeField]
	private float antiGravityHeightLimit;
	private float appliedGravity;
	private bool isAntiGravity;

	public void ToggleAntiGravity(bool flag, float heightLimit =0F, float gravityAmount =0F)
	{
		if (isAntiGravity == flag)
			return;

		if (flag) 
		{
			appliedGravity = gravity + gravityAmount;
			antiGravityHeightLimit = heightLimit;

		} else {

			antiGravityHeightLimit = 0F;
			appliedGravity = 0F;
		}

		isAntiGravity = flag;
	}

	public void Accelerate(float AccelSpeed, float AccelDuration)
	{
		if (isAcceleRate == false) {

			accelerationTime = AccelDuration;
			StartCoroutine(AccelerateTimer(AccelSpeed));
		}
	}

	public IEnumerator AccelerateTimer(float AccelSpeed)
	{
		isAcceleRate = true;

		while(accelerationTime > 0F)
		{
			yield return null;
			accelerationTime -= Time.deltaTime;
			Vector3 direction = transform.TransformDirection (Vector3.forward);
			controller.Move(direction * Time.deltaTime * AccelSpeed);
			animator.SetFloat("Speed", 1F);
		}
		
		isAcceleRate = false;
		animator.SetFloat("Speed", 0F);
	}

	public void Jump(bool isForced = false, float jumpHeight = -1F, float forceX = 0F, float forceZ = 0F)
	{
		if(isKnockBack) return;

		isOnPlatform = false;

		bool isSpin = false;

		if (jumpCount > 0) 
		{
			isSpin = true;
		}

		if (!isForced) 
		{
			if (jumpCount > 1) {
				if (canFly) {
					jumpCount = 1;
				} else {
					jumpCount = 2;
					return;
				}
			}

			jumpCount ++;
		} else {
			jumpCount = 1;
		}

		if (isSpin) 
		{
			if(SoundBoard.instance != null)
				SoundBoard.instance.PlayFromSoundBoard("SND_PC_DOUBLE_JUMP", this.transform.position);

			animator.SetTrigger("SpinTgr"); //켜지고 트랜지션이 일어나면 자동으로 꺼지는 것.  //왜 이걸 몰랐을까! 타이밍 맞춰서 bool false할 필요가 없다 
		} else {

			if(SoundBoard.instance != null)
				SoundBoard.instance.PlayFromSoundBoard("SND_PC_JUMP", this.transform.position);

			animator.SetTrigger("JumpTgr");
		}

		if (jumpHeight == -1F) 
		{ //default
			if(isSpin)
			{
				jumpHeight = doubleJumpSpeed;
			} else {
				jumpHeight = jumpSpeed;
			}
		}

		jumpHeight = jumpHeight - gravityReduceFactor;

		if (jumpHeight < 0F)
			jumpHeight = 0F;

		savedJumpHeight = jumpHeight;
		initialJumpY = currentY;


		moveDirection.y = jumpHeight * jumpAccelRate;

		if (forceX != 0F || forceZ != 0F) 
		{

			throwingVector = new Vector3(forceX, 0F, forceZ);

//			Debug.DrawRay(this.transform.position, throwingVector, Color.magenta);

//			initThrowingVec = throwingVector;
//			moveDirection.x = throwingVector.x;
//			moveDirection.z = throwingVector.z;
		}

		controller.Move(moveDirection * Time.deltaTime); // * speed);
	}

//	Vector3 initThrowingVec;
	public Vector3 throwingVector;

	public void KnockBack(Vector3 knockBackVector, float knockBackValue)
	{
		if(GameRuleManager.instance.eGameStatus != E_GAME_STATUS.IN_PLAY) return;
		if(knockBackValue == 0F) return;

		savedJumpHeight = 0F;

		if(isKnockBack == false)
		{
            StartCoroutine(KnockBackCoroutine(knockBackVector, knockBackValue, knockBackTime));

			if(playerFX != null)
			{
				playerFX.PlayKnockFX();
			}
		}

	}

	private float knockBackMeterFactor = 80F;
	IEnumerator KnockBackCoroutine(Vector3 knockBackVector, float knockBackValue = 1F, float time= 1F)
	{
		isKnockBack = false;
		knockBackValue = knockBackValue * knockBackMeterFactor;

		knockBackVector = new Vector3(knockBackVector.x, 0F, knockBackVector.z); //Y값을 없애서 공중에 넉백 당할경우 슈퍼 점프하는 것 방지 

		moveDirection = Vector3.zero;

		float timer = 0F;
		float knockBackAmount = 0F;
		
		while (timer < time) // && isGrounded) 
		{
			timer += Time.deltaTime;

			isKnockBack = true;

			knockBackAmount = Mathf.Lerp(knockBackValue, 0F, timer / time);
			
			moveDirection += knockBackVector * knockBackAmount * Time.fixedDeltaTime;
			controller.Move(moveDirection * Time.deltaTime);
//			transform.Translate(knockBackVector * knockBackAmount * Time.deltaTime, Space.World);

			horz = 0F;
			vert = 0F;

			animator.SetFloat("Speed", 0F);
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}

		isKnockBack = false;
	}

	public void FootSound()
	{
//		if(SoundBoard.instance != null)
//			SoundBoard.instance.PlayFromSoundBoard("SND_PC_MOVE", this.transform.position);
	}

	public void AddPassive(int passiveid = 0, float value = 0)
	{
		string passiveSkillName = System.Enum.GetName (typeof(PLAYER_PASSIVE), passiveid);
		PlayerPassive passive = GetComponent (System.Type.GetType(passiveSkillName)) as PlayerPassive;

		if(passive == null)
		{
			passive = transform.gameObject.AddComponent(System.Type.GetType(passiveSkillName)) as PlayerPassive;
		} else {
			GameRuleManager.instance.AddGold(10);
		}

		// magnet effect On
		if (passiveid == System.Convert.ToInt32(PLAYER_PASSIVE.PlayerPassive_Magnet)) {
			playerFX.ToggleOnMagnetFX();
		}

		if (passive != null) {
			passive.amount = value;
		}
	}

	public void RemovePassive(int passiveid = 0)
	{
		PlayerPassive passive = GetComponent (System.Enum.GetName (typeof(PLAYER_PASSIVE), passiveid)) as PlayerPassive;

		if(passive != null)
		{
			if (passiveid == System.Convert.ToInt32(PLAYER_PASSIVE.PlayerPassive_Magnet)) {
				playerFX.ToggleOffMagnetFX();
			}

			Destroy(passive);
		}
	}

	void OnColliderEnter(Collider collision)
	{
		Debug.Log (collision.gameObject.name);
	}
} 
