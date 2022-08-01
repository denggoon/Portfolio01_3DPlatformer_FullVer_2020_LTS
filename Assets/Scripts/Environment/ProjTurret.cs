using UnityEngine;
using System.Collections;

public class ProjTurret : Trap {
	
	public float damage;
	
	public GameObject projectileObj;
	public float projVelo;
	
	public float knockbackDist = 20F;

	public Transform shootingBarrel;

	public override void SwitchTrigger (bool flag)
	{
		if(flag == true)
		{
			ProjectileAttack((this.transform.forward).normalized);
		}
	}

	void ProjectileAttack(Vector3 shootDir)
	{
		if(projectileObj == null) return; //발사체가 설정되어 있지 않으면 아무것도 하지 않음 
		if(shootingBarrel == null) return; //발사 위치가 설정되지 않은경우 아무것도 하지 않음. 

		GameObject proj = GameObject.Instantiate(projectileObj, shootingBarrel.transform.position, projectileObj.transform.rotation) as GameObject;
		//발사체 생성 
		
		EnemyProj enemProj = proj.GetComponent<EnemyProj>(); //발사체 스크립트 접근 
		
		if(enemProj != null)
		{
			enemProj.SetDamage(this.damage); //데미지 설정 
			enemProj.SetShooter(this.gameObject); //발사한 장본인 설정 
			enemProj.Launch(shootDir, projVelo);
		}
	}
}
