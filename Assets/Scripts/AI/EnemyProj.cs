using UnityEngine;
using System.Collections;

public class EnemyProj : MonoBehaviour {

	public GameObject shooter;

	public float damage = 10F;
	public float knockbackDist;

	public float lifeTime = 5F;
	public float lifeTimer;

	private Rigidbody thisRigidBody;

	private PooledObj poolObj;
	private SoundEventCaller movingSound;

	public Rigidbody GetRigidBody()
	{
		if(thisRigidBody == null)
		{
			thisRigidBody = GetComponent<Rigidbody>();
		}

		return thisRigidBody;
	}

	void Awake()
	{
		thisRigidBody = GetComponent<Rigidbody>();
		poolObj = GetComponent<PooledObj>();
		movingSound = GetComponent<SoundEventCaller> ();
		lifeTimer = lifeTime;

		if (movingSound != null) {
			movingSound.PlaySound(this.transform.position);
		}
	}

	public void SetDamage(float damage) //외부에서 본 발사체 스크립트에 데미지를 설정할 수 있도록 해주는 함수 
	{
		if (damage > 0) {
			this.damage = damage;
		}
	}

	public void SetShooter(GameObject shooter) //본 발사체를 발사한 사람이 누군지 설정하는 함수 
	{
		this.shooter = shooter;
		this.GetComponent<Collider>().enabled = true;
	}

	private FMOD.Studio.EventInstance flyingSoundEvent;
	public void Launch(Vector3 dir, float velocity)
	{
		thisRigidBody.AddForce(dir * velocity, ForceMode.Impulse);
	}

	void Update()
	{
		if(GetComponent<Rigidbody>().velocity != Vector3.zero) //강체의 속도 벡터가 0이 아니면 
			transform.rotation  = Quaternion.LookRotation(GetComponent<Rigidbody>().velocity, transform.up); //벡터 방향으로 발사체가 바라보도록 설정 

		lifeTimer -= Time.deltaTime; //라이프 타이머 작동 

		if (lifeTimer <= 0F) { //0이 되면 적을 적중시켰는지와는 관계없이 그냥 소멸
			PushOrDestroy ();
		} else {

		}

	}

	void OnTriggerEnter(Collider collider) //무언가와 충돌 
	{
		if(collider.gameObject.layer == LayerMask.NameToLayer("Player")) //플레이어인 경우 
		{
			if(GameRuleManager.instance.playerHealth.isInvincible == false)
			{
				GameRuleManager.instance.playerMove.KnockBack(GetComponent<Rigidbody>().velocity.normalized, knockbackDist); //넉백 효과도 줌 

			} else {

				if(GameRuleManager.instance.playerMove.playerSkill != null)
				{
					if(GameRuleManager.instance.playerMove.playerSkill is PlayerSkill_Harden && GameRuleManager.instance.playerMove.playerSkill.isActivated == true)
					{
						thisRigidBody.velocity = Vector3.Reflect(thisRigidBody.velocity, (Vector3.right + Vector3.forward).normalized);
						return;
					}
				}
			}

			GameRuleManager.instance.playerHealth.TakeDamage(damage); //데미지 줌 
		}

		//lifeTime -.1F는 임시방편, 자신이 발사한 발사체가 스스로에게 맞지 않으려면 어떻게 해야하는지 고민할 필요가 있음... 
		if(collider.gameObject.layer == LayerMask.NameToLayer("Enemy") && shooter != null && collider.gameObject != shooter && lifeTimer < lifeTime - Time.deltaTime) //적인경우 
		{
			EnemyMovement enemyMove = collider.gameObject.GetComponent<EnemyMovement>();

			if(enemyMove != null)
			{
				Debug.Log("EnemyHit!");
				enemyMove.Stun(damage); //적 스턴 
				PushOrDestroy();
				return;

			}
		}

		if (collider.gameObject.layer != LayerMask.NameToLayer("Projectile") 
		    && collider.gameObject.tag != "Enemy" 
		    && collider.gameObject.layer != LayerMask.NameToLayer("Item")
		    && collider.gameObject.layer != LayerMask.NameToLayer("Magnet")) 
		{ //같은 발사체가 아닌경우에는 - Enemy만 태그 테스트인 이유는 StompPlate, VisionCollider 등이 Enemy 소속 이기 때문 
			PushOrDestroy();
		}
	}

	void PushOrDestroy()
	{
		if( poolObj == null)
		{
			Destroy (this.gameObject);
		} else {
			
			string name = this.gameObject.name;
			name = name.Replace("(Clone)", string.Empty);
			
			ObjectPooler.instance.ObjPush(name, this.gameObject);
		}
	}
}
