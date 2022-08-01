using UnityEngine;
using System.Collections;

public class EnemyWeapon : MonoBehaviour {

	new public Transform transform;

	public Transform fireTrans;

	public EnemyMovement movement;

	public float attackRate;
//	public float attackSpeedRate = 1F;
//	public float attackInvincibleRatio = .8F;

	public bool useVisionRange = true;
	public float attackRange; //Nav Mesh의 Stopping Distance보다 작을수는 없습니다. 

	public float damage;

	public string projObjStr;
	public float projVelo;

	public float knockbackDistForRay;

	private Vector3 shootingDir = Vector3.zero;

	public string atkFxStr;
	public bool isCannonBall = false;
	public float cannonShotAngle = 0F;

	void Awake () {
		transform = GetComponent<Transform>();

		movement = GetComponent<EnemyMovement>();

		movement.SetAttackTimer(attackRate);

		if (fireTrans == null)
			fireTrans = transform.Find ("objPosition");

		if (isCannonBall == true) {
			fireTrans = fireTrans.Find ("objPosition");
			fireTrans.parent.transform.localEulerAngles = new Vector3(360.0F - cannonShotAngle, 0, 0);
		}

	}

	void Start()
	{
		if(string.IsNullOrEmpty(projObjStr))
		{
			attackRate = 0F;
			
			if(attackRange <= 0F)
			{
				attackRange = 1F;
			}
			
		} else {
			
			if(useVisionRange)
				attackRange = movement.GetVision().visionRadius;
		}
	}

	public void Attack(Vector3 shootDir)
	{
		this.shootingDir = shootDir; //나중에 애니메이션의 이벤트를 통해서 함수가 호출되므로 this.shootingDir에 변수를 이관해 놓습니다. 

//		attackTimer -= Time.deltaTime; //공격 명령을 받으면 공격 타이머 작동 => EnemyMovement쪽으로 이관 함 

		if(movement.GetAttackTimer() >= 0F) //타이머가 0이하가 되기 전까지는 아무것도 하지 않음 
			return;

		if (string.IsNullOrEmpty (projObjStr)) { //발사체가 설정 되있지 않으면
			movement.GetAnimator ().SetTrigger ("MeleeAtkTgr");
		} else if (movement.GetAnimator () != null) {
			movement.GetAnimator ().SetTrigger ("RangeAtkTgr");
		}

		if(attackRate <= 0F)
			attackRate = movement.GetAnimator().GetNextAnimatorStateInfo(0).length;

		movement.SetAttackTimer(attackRate); //타이머를 다시 공격 속도로 설정 
	}

	public void RaycastAttack() //Vector3 shootDir)
	{
		RaycastHit hit;

		Vector3 firePos = Vector3.zero;
		
		if (fireTrans == null) {
			firePos = new Vector3 (transform.position.x, transform.position.y + movement.GetMyCollider ().height * .25F, transform.position.z);
			//Raycast를 그냥 진행하면 transform.position의 특성상 발바닥에서 레이를 발사하기 때문에 collider 크기의 반값 높이에서 시작 
		} else {
			firePos = fireTrans.position;
		}

		Ray shootRay = new Ray(firePos , shootingDir); //조정된 위치에서 플레이어 방향으로 레이 생성 

		Debug.DrawRay(firePos, shootingDir, Color.red);

		if (!string.IsNullOrEmpty (atkFxStr))
			ObjectPooler.instance.ObjPop (atkFxStr, firePos);

		if(Physics.Raycast(shootRay, out hit, attackRange)) //시야 범위내에서 레이를 플레이어 방향으로 발사 했을때 
		{
			Debug.DrawLine(firePos, hit.point, Color.magenta);
			if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Player")) //레이가 플레이어에 적중하는경우 
			{
				if(GameRuleManager.instance.playerHealth.isInvincible == false)
				{
					GameRuleManager.instance.playerMove.KnockBack(shootingDir.normalized, knockbackDistForRay); //넉백 효과도 줌 
				}

				GameRuleManager.instance.playerHealth.TakeDamage(damage); //데미지 줌 
			}
		}
	}

	void ProjectileAttack() //Vector3 shootDir)
	{
		if(string.IsNullOrEmpty(projObjStr)) return; //발사체가 설정되어 있지 않으면 아무것도 하지 않음 

		Vector3 firePos = Vector3.zero;

		if (fireTrans == null) {
			firePos = new Vector3 (transform.position.x, transform.position.y + movement.GetMyCollider ().height * .25F, transform.position.z);
		} else {
			firePos = fireTrans.position;
		}
		//Raycast를 그냥 진행하면 transform.position의 특성상 발바닥에서 레이를 발사하기 때문에 collider 크기의 반값 높이에서 시작 

		GameObject rsrc = ResourcesManager.instance.LoadGameObject (projObjStr);
		GameObject proj = GameObject.Instantiate(rsrc, firePos, rsrc.transform.rotation) as GameObject;
		//발사체 생성 

		if (isCannonBall == false) {
			EnemyProj enemProj = proj.GetComponent<EnemyProj> (); //발사체 스크립트 접근 
			if(enemProj != null)
			{
				enemProj.SetDamage(this.damage); //데미지 설정 
				enemProj.SetShooter(this.gameObject); //발사한 장본인 설정 
				enemProj.Launch(this.shootingDir , projVelo); //희망 방향으로 발사체 날림 
			}

		} else {
			EnemyCannonBall enemProj = proj.GetComponent<EnemyCannonBall>(); // cannon 발사체 스크립트 접근 
			if(enemProj != null)
			{
				enemProj.SetDamage(this.damage); //데미지 설정 
				enemProj.SetShooter(this.gameObject); //발사한 장본인 설정 
				enemProj.Launch(this.shootingDir , projVelo); //희망 방향으로 발사체 날림 
			}
		}

		if (SoundBoard.instance != null)
			SoundBoard.instance.PlayFromSoundBoard ("SND_MON_" + movement.monsterID + "_SHOOT", this.transform.position);
	}
}
