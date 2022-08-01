using UnityEngine;
using System.Collections;

public class PooledObj : MonoBehaviour 
{
	new Transform transform;
	public Transform parentTrans;

	public virtual void Awake()
	{
		transform = GetComponent<Transform> ();

		parentTrans = transform.parent;
	}

	public virtual void WhenTaskDone()
	{
		if (ObjectPooler.instance == null) 
		{
			if(parentTrans != null)
			{
				if(this.transform.parent.name.Contains("(Clone)"))
				{
					Destroy(this.transform.parent.gameObject);
					return;
				} 
			}

			Destroy(gameObject);
			return;
		}

		if (ObjectPooler.instance.ObjPush (this.gameObject.name, this.gameObject) == false) 
		{
			if(parentTrans != null)
			{
				if(this.transform.parent.name.Contains("(Clone)"))
				{
					Destroy(this.transform.parent.gameObject);
					return;
				} 
			}

			Destroy(gameObject);
		}
	}
}
