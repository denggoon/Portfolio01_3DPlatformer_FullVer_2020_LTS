using UnityEngine;
using System.Collections;

public class JiggleBonePhysics : MonoBehaviour {

	private Transform thisParent;
	private Rigidbody thisRigidbody;

	private Vector3 parentPosLastFrame = Vector3.zero;

	void Awake()
	{
		thisParent = this.transform.parent;
		thisRigidbody = GetComponent<Rigidbody>();

	}

	// Update is called once per frame
	void Update () 
	{
		thisRigidbody.AddForce((parentPosLastFrame - this.transform.position) * 100F);
		parentPosLastFrame = thisParent.position;
	}
}
