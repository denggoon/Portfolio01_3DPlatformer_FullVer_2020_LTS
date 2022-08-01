using UnityEngine;
using System.Collections;

[System.Serializable]
public class RouteInfo
{
	public Transform routeTrans;
	public float routeSpeed;
	public float routeEndWaitTime;
	public string routeEndSound;
	public string routeEndFx;
	public string routeEndAni;

	public RouteInfo(Transform trans, float speed = 0F)
	{
		routeTrans = trans;
		routeSpeed = speed;
	}
}

public class MovingRoute : MonoBehaviour {

	new public Transform transform;

	public GameObject goPatrolPattern;
//	public Transform[] routeTrans;

	public RouteInfo[] routes;

	public int routeIndex = 0;

	public virtual void Awake() //프리팹 routeTrans로 바꾸는 함수 
	{
		transform = GetComponent<Transform>();

		if (goPatrolPattern == null) 
		{
			Transform routeBox = transform.parent.Find ("route_Box");

			if(routeBox != null)
				goPatrolPattern = routeBox.gameObject;

		}

		IntepretePatternFromPrefab ();
		RemoveUnnecessaryColliders ();
	}

	public virtual void IntepretePatternFromPrefab()
	{
		if (goPatrolPattern == null)
			return;

		if (goPatrolPattern.transform.childCount <= 0)
			return;

//		routeTrans = new Transform[goPatrolPattern.transform.childCount];
//
//		for (int i=0; i<routeTrans.Length; i++) 
//		{
//			routeTrans[i] = goPatrolPattern.transform.GetChild(i);
//		}
//		
//		System.Array.Sort(routeTrans, delegate(Transform a, Transform b) { return a.gameObject.name.CompareTo(b.gameObject.name); });

		if(routes == null || routes.Length == 0)
			routes = new RouteInfo[goPatrolPattern.transform.childCount];

		for (int j=0; j<routes.Length; j++) 
		{
			Transform transformInfo = goPatrolPattern.transform.GetChild(j);
			if(routes[j] == null)
			{
				RouteInfo route = new RouteInfo(transformInfo);
				routes[j] = route;
			} else {
				routes[j].routeTrans = transformInfo;
			}
		}
		
		System.Array.Sort(routes, delegate(RouteInfo a, RouteInfo b) { return a.routeTrans.name.CompareTo(b.routeTrans.name); });
	}

	void RemoveUnnecessaryColliders()
	{
//		for (int i=0; i<routeTrans.Length; i++) 
//		{
//			Collider routeCollider = routeTrans [i].GetComponent<Collider> ();
//			if (routeCollider != null) {
//				Destroy (routeCollider);
//			}
//		}

		for (int j=0; j<routes.Length; j++) 
		{
			Collider routeCollider = routes [j].routeTrans.GetComponent<Collider> ();
			if (routeCollider != null) {
				Destroy (routeCollider);
			}
		}
	}
}
