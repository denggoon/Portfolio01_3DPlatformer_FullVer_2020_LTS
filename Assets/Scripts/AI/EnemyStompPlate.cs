using UnityEngine;
using System.Collections;

public class EnemyStompPlate : MonoBehaviour {

	public EnemyMovement movement;
	public EnemyCannonBall cannonBall;
	public BoxCollider stompCollider;

//	private int playerLayer;
	private new Transform transform;
	private float stompableHeight;

	void Awake()
	{
		transform = GetComponent<Transform> ();

		movement = transform.GetComponentInParent<EnemyMovement> ();
		cannonBall = transform.GetComponentInParent<EnemyCannonBall> ();
//		playerLayer = LayerMask.NameToLayer("Player");
	}

	void Start()
	{
		stompCollider = GetComponent<BoxCollider>();
		stompableHeight = .2F;
	}

	void OnTriggerEnter(Collider collider)
	{
		if(stompCollider == null) return;
		if(GameRuleManager.instance.playerMove == null) return;
		if(GameRuleManager.instance.playerMove.ePlayerAtkType != E_PLAYER_ATTACK_TYPE.HOPNBOP) return; //적을 밟아서 죽이는 형태의 공격을 할수 없는 플레이어면 스킵 
		if (movement != null) {
			if (movement.IsStunned ())
				return; //적이 이미 스턴 상태일때는 플레이어가 추가적으로 데미지를 줄수 없습니다. 
		}

		if(collider.gameObject.layer == LayerMask.NameToLayer("Player")) //감지한 컬라이더가 플레이어인 경우 
		{
			if (movement == null) {

				bool jumpingDown = (GameRuleManager.instance.playerMove.fallDelta < 0 ? true : false);
				if (jumpingDown == true) {

					if (cannonBall != null) {
						cannonBall.onJumping();
					}
					GameRuleManager.instance.playerMove.Jump (true);
				}

			} else {
	//			Debug.Log(GameRuleManager.instance.playerMove.transform.position.y + " / " + (movement.transform.position.y + stompableHeight));
				bool jumpingDown = (GameRuleManager.instance.playerMove.fallDelta < 0 ? true : false);
				if(GameRuleManager.instance.playerMove.transform.position.y > movement.transform.position.y + stompableHeight && jumpingDown == true)
				{
					ResourcesManager.instance.PopEffect ("FX_JumpAttack_01", this.transform.position);
					GameRuleManager.instance.playerMove.Jump (true);
					movement.Stun(GameRuleManager.instance.playerMove.attackDamage, true); //설정된 데미지 만큼 줍니다. 
				}
			}
		}
	}
}
