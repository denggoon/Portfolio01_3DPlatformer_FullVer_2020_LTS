using UnityEngine;
using System.Collections;

public class MonsterAnimEventCaller : AnimatorEventCaller {

	#region PlayerSound Functions
	
	public void FootSound()
	{
		parentObj.SendMessage ("FootSound", SendMessageOptions.DontRequireReceiver);
	}
	
	#endregion

	#region Monster Functions

	public void ProjectileAttack()
	{
		parentObj.SendMessage ("ProjectileAttack", SendMessageOptions.DontRequireReceiver);
	}

	public void RaycastAttack()
	{
		parentObj.SendMessage ("RaycastAttack", SendMessageOptions.DontRequireReceiver);
	}

	public void MonsterDeath()
	{
		parentObj.SendMessage ("MonsterDeath", SendMessageOptions.DontRequireReceiver);
	}

	#endregion
}
