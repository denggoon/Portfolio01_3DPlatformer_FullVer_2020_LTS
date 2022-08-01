using UnityEngine;
using System.Collections;

public class PatrolRoute : MovingRoute {

	public EnemyMovement movement;

	public override void Awake()
	{
		base.Awake(); //상위클래스의 route 변환 함수를 먼저 시작합니다. 

		movement = GetComponent<EnemyMovement>();
	}

	void OnDrawGizmos()
	{
		if(movement == null) return;

		if(movement.eAIStatus == E_AI_STATUS.PATROL)
		{
			Gizmos.DrawIcon(movement.GetDestination(), "patrol.png"); //씬뷰에 적이 바라보는 대상을 표시할 기즈모를 그립니다. 
		}
	}

}



