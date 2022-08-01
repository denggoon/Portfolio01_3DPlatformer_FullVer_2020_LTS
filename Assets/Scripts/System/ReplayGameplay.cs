using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class ReplayGameplay : MonoBehaviour {

	private RecordGameplay recorder;

	private Animator animator;
	private List<RecordedEvent> eventsReplay;

	private int frameCount = 0;
	private int frameLimit = 0;

	public bool IsReplay = false;

	public bool hasRetrievedList = false;

    private int idleHash;

	// Use this for initialization
	void OnDestroy()
	{
		GameRuleManager.instance.replayer = null;
	}

	void Start () 
	{
		recorder = GameRuleManager.instance.recorder;

		GameRuleManager.instance.replayer = this;

//		Debug.Log(">>>>>" + Marshal.SizeOf(typeof(RecordedEvent)) + "bytes" );
		Debug.Log ("A Vector3 Uses " + Marshal.SizeOf(typeof(Vector3)) +" bytes");
		Debug.Log ("A Quaternion Uses " + Marshal.SizeOf(typeof(Quaternion)) +" bytes");
		Debug.Log ("A float Uses " + Marshal.SizeOf(typeof(float)) +" bytes");

		animator = GetComponent<Animator> ();

        idleHash = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
    }

	public void StartReplay ()
	{
		if (IsReplay == true)
			return;

		this.transform.parent.position = GameRuleManager.instance.initPos;
		this.transform.parent.rotation = GameRuleManager.instance.initRot;

		recorder = GameRuleManager.instance.recorder;
		eventsReplay = recorder.GetEventsList ();

		if (eventsReplay.Count > 0)
			hasRetrievedList = true;

		StartCoroutine (ReplayCoroutine ());
	}

	IEnumerator ReplayCoroutine()
	{
		float intervalDuration = recorder.recordingInterval;

		for(int i=0; i<eventsReplay.Count; i++)
		{
			IsReplay = true;

			Vector3 oldPos = this.transform.position;

			SerializableVector3 recordedPos = eventsReplay[i].position;
			Vector3 newPos = new Vector3(recordedPos.x, recordedPos.y, recordedPos.z);

			Quaternion oldRot = this.transform.localRotation;

			SerializableQuaternion recordedRot = eventsReplay[i].rotation;
			Quaternion newRot = new Quaternion(recordedRot.x, recordedRot.y, recordedRot.z, recordedRot.w);

			ReproduceAnim (eventsReplay [i].animHash);

			if(intervalDuration == 0)
			{
				this.transform.position = newPos;
				this.transform.localRotation = newRot;

				yield return null;
			} else {

				float intervalTimer = 0F;

				while(intervalTimer <= intervalDuration)
				{
					float rate = intervalTimer/intervalDuration;

					this.transform.position = Vector3.Lerp(oldPos, newPos, rate);
					this.transform.localRotation = Quaternion.Lerp(oldRot, newRot, rate);
					intervalTimer += Time.deltaTime;
					yield return new WaitForSeconds(Time.deltaTime);
				}
			}
		}

		IsReplay = false;

		ReproduceAnim(idleHash);
	}

	void ReproduceAnim(int animHash)
	{
		if(animator.GetCurrentAnimatorStateInfo(0).shortNameHash != animHash)
			animator.Play (animHash);
	}
}
