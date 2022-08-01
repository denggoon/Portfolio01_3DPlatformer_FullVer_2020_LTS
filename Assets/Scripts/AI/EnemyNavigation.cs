using UnityEngine;
using System.Collections;

public class EnemyNavigation : MonoBehaviour {

	new Transform transform;
	new Rigidbody rigidbody;

	public Vector3 destination;

	public EnemyMovement movement;

	public float speed;
	public float stoppingDistance;
	public float remainingDistance;
	void Awake()
	{
		transform = GetComponent<Transform> ();
		rigidbody = GetComponent<Rigidbody> ();
		rigidbody.isKinematic = false;
//		rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ;

		movement = GetComponent<EnemyMovement> ();

		stoppingDistance = 0.5F;
	}

	public void SetDestination(Vector3 pos) //should be called every frame for constant movement
	{
		destination = pos;

		//설정된 목적지가 공중의 위치일경우, 몬스터가 갈 수 없으므로, 지면상에서 갈 수 있는 위치로 변환합니다. 
		RaycastHit groundHit;
		if (Physics.Raycast (pos, Vector3.down, out groundHit, 10F, LayerMask.NameToLayer("collision"))) 
		{
			Debug.DrawLine(pos, groundHit.point, Color.red);
			destination = groundHit.point;
		}

		//가려는 목적지에 벽이 있는경우, 벽까지만 갑니다. 

		if (destination.y > transform.position.y) 
		{
			RaycastHit rampTest;
			if (Physics.Raycast (this.transform.position, transform.forward, out rampTest, movement.GetVision ().visionRadius, LayerMask.NameToLayer ("colliion"))) {
				//몬스터 기준으로 정면으로 레이저를 쏘았을때, 반사된 각이 플레이어가 바라보는 방향과 서로 바라볼 경우에는 벽이라고 인지합니다. 
				Debug.DrawLine (this.transform.position, rampTest.point, Color.blue);

				float dotResult = Vector3.Dot (rampTest.normal, transform.forward);
				if (dotResult <= -1F) {
					destination = rampTest.point;
				}
			}
		}

		Vector3 travelDirection = (destination - transform.position).normalized;

		remainingDistance = Vector3.Distance (destination , transform.position);

		if (remainingDistance > stoppingDistance) 
		{
			if(travelDirection != Vector3.zero)
			{
				transform.rotation = Quaternion.LookRotation (travelDirection); //적을 부드럽게 돌리는것은 이 라인을 수정하면 될것으로 보입니다. 
			}

			rigidbody.velocity = (travelDirection * speed) + Mathf.Clamp01(transform.position.y - destination.y) * Physics.gravity;

		} else {

//			transform.localEulerAngles = Vector3.zero;
			rigidbody.velocity = Vector3.zero;
		}
	}
}
