// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public enum RecordActions { playerMovement, crateCollision, createPoster }
public enum RecordStatus { stopped = 1, paused, recording }

[System.Serializable]
public struct SerializableVector3
{
	public float x;
	public float y;
	public float z;
}

[System.Serializable]
public struct SerializableQuaternion
{
	public float x;
	public float y;
	public float z;
	public float w;
}

public class RecordGameplay : MonoBehaviour {
	public static RecordGameplay SP;
	public RecordStatus recordStatus;
	private float startedRecording =0;
	private List<RecordedEvent> replayData = new List<RecordedEvent>();
	private float pausedAt = 0;

	void Awake(){

		SP =this;
		recordStatus = RecordStatus.stopped;
	}

	public float RecordTime()
	{
		if (recordStatus != RecordStatus.recording)
			Debug.LogError("###RecordGamePlay.cs - Can't get time!");
		return Time.realtimeSinceStartup - startedRecording;
	}
	public void PauseRecording()
	{
		if (recordStatus != RecordStatus.recording)
			Debug.LogError("###RecordGamePlay.cs - Can't pause! " + recordStatus);
		pausedAt = RecordTime();
		recordStatus = RecordStatus.paused;
	}
	public void ResumeRecording(float startTime = 0F)
	{
		if (startTime == 0F) 
		{
			startTime = pausedAt;
		}

		//Delete all actions after STARTTIME. Continue Recording from this point
		if (replayData.Count >= 0)
		{
			for (int i = replayData.Count - 1; i >= 0; i--)
			{
				RecordedEvent action = replayData[i];
				if (action.time >= startTime)
					replayData.Remove(action);
			}
		}
		pausedAt = startTime;
		StartRecording();
	}

	public void StartRecording(float recordDuration = 0F, float recordInterval = 0F)
	{
		switch (recordStatus) 
		{
			case RecordStatus.paused:
				Debug.LogWarning("###RecordGamePlay.cs - Resume Recording!");
				startedRecording = Time.realtimeSinceStartup - pausedAt;
			break;

			case RecordStatus.stopped:
				Debug.LogWarning("###RecordGamePlay.cs - Start Recording!");
				startedRecording = Time.realtimeSinceStartup;
				replayData = new List<RecordedEvent>();

				if(recordingDuration == 0F)
					recordingDuration = recordDuration;

				if(recordingInterval == 0F)
					recordingInterval = recordInterval;

			break;

			case RecordStatus.recording:
				Debug.LogWarning("###RecordGamePlay.cs - Already Recording in progress!");
				return;
		}

		recordStatus = RecordStatus.recording;

		StartCoroutine (RecordCoroutine (GameRuleManager.instance.playerMove));
	}

	public bool IsRecording(){
		return recordStatus == RecordStatus.recording;
	}
	public bool IsPaused(){
		return recordStatus == RecordStatus.paused;
	}
	//Hereby multiple defenitions so that you can add as much data as you want.
	public void AddAction(RecordActions action)
	{
		AddAction(action, Vector3.zero);
	}

	public void AddAction(RecordActions action, Vector3 position)
	{
		AddAction(action, position, Quaternion.identity, 0);
	}

	public void PrintReplayRecord()
	{
		if (replayData.Count <= 0) 
		{
			Debug.LogWarning("###RecordGamePlay.cs - No Data to Print!");
			return;
		}

//		RecordedEvent[] arrRecord = replayData.ToArray();
			
		ExporterFunc<float> floatToDoubleExporter = 
			delegate (float floatVal, JsonWriter writer)
			{
				writer.Write(System.Convert.ToDouble(floatVal));
            };

//		ExporterFunc<Vector3> vectorToStringExporter = 
//			delegate (Vector3 vectorValue, JsonWriter writer)
//		{
//			writer.Write(vectorValue.ToString());
//		};
//
//		ExporterFunc<Quaternion> QuaternionToStringExporter = 
//			delegate (Quaternion quaternionValue, JsonWriter writer)
//		{
//			writer.Write(quaternionValue.ToString());
//		};

		JsonMapper.RegisterExporter<float> (floatToDoubleExporter);
//		JsonMapper.RegisterExporter<Vector3> (vectorToStringExporter);
//		JsonMapper.RegisterExporter<Quaternion> (QuaternionToStringExporter);

		string jsonizedRecord = JsonMapper.ToJson (replayData);

		if (!string.IsNullOrEmpty (jsonizedRecord)) 
		{
			File.WriteAllText(Application.persistentDataPath + "/RecordedReplay.txt", jsonizedRecord);

			FileStream fs = new FileStream(Application.persistentDataPath + "/RecordReplayRaw.txt", FileMode.Create);

			BinaryFormatter bf = new BinaryFormatter();

			bf.Serialize(fs, replayData);

			fs.Close();
		}
	}

	List<RecordedEvent> ConvertBinaryToRecordedEvent()
	{
		List<RecordedEvent> readableRecord = new List<RecordedEvent>();

		FileStream fs = new FileStream(Application.persistentDataPath + "/RecordReplayRaw.txt", FileMode.Open);
		
		BinaryFormatter bf = new BinaryFormatter();
		
		readableRecord = bf.Deserialize(fs) as List<RecordedEvent>;
		
		fs.Close();

		return readableRecord;
	}

	public float recordingInterval;
	public float recordingDuration;
	public float recordingTimer;

	public IEnumerator RecordCoroutine(PlayerMoveCC player)
	{
		recordingTimer = recordingDuration;
		while (recordStatus == RecordStatus.recording) 
		{
			if(player == null || GameRuleManager.instance.eGameStatus == E_GAME_STATUS.GAME_OVER)
			{
				Debug.LogWarning("###RecordGamePlay.cs - RecordCoroutine Stopped: Player Not Found OR Game is over");
				StopRecording();
				break;
			}

			AddAction (RecordActions.playerMovement, player.transform.position, player.transform.localRotation, player.animator.GetCurrentAnimatorStateInfo (0).shortNameHash);

			if (recordingInterval == 0F) 
			{
				recordingTimer -= Time.deltaTime;
				yield return null;
			} else {
				recordingTimer -= recordingInterval;
				yield return new WaitForSeconds(recordingInterval);
			}

			if(recordingDuration > 0 && recordingTimer <= 0)
			{
				Debug.LogWarning("###RecordGamePlay.cs - RecordCoroutine Stopped: Recording Duration Passed");
				StopRecording();
			}
		}

		if (recordStatus == RecordStatus.stopped) {
			Debug.LogWarning ("###RecordGamePlay.cs - RecordCoroutine Stopped, now prints results");
			PrintReplayRecord ();
		} else if(recordStatus == RecordStatus.paused) 
		{
			Debug.LogWarning("###RecordGamePlay.cs - RecordCoroutine Stopped for now, pause has been requested");
		}
	}

	public void AddAction(RecordActions action, Vector3 position, Quaternion rotation, int animHash)
	{
		if (!IsRecording())
		{
			//Debug.LogError("Record didn't start!");
			return;
		}
		RecordedEvent newAction = new RecordedEvent();
		newAction.recordedAction = action;
		newAction.position.x = position.x;
		newAction.position.y = position.y;
		newAction.position.z = position.z;

		newAction.rotation.x = rotation.x;
		newAction.rotation.y = rotation.y;
		newAction.rotation.z = rotation.z;
		newAction.rotation.w = rotation.w;

		newAction.time = RecordTime();
		newAction.animHash = animHash;
		replayData.Add(newAction);
	}
	public void StopRecording()
	{
		if(recordStatus != RecordStatus.recording)
			Debug.LogError("###RecordGamePlay.cs - Can't STOP!");
		stoppedAtLength = RecordTime();
		recordStatus = RecordStatus.stopped;
	}
	private float stoppedAtLength = 0;
	public float LastRecordLength()
	{
		if (recordStatus == RecordStatus.paused) return pausedAt;
		if (recordStatus != RecordStatus.stopped) return RecordTime();
		return stoppedAtLength;
	}
	public string RecordedDataToReadableString(){
		string output ="Replay data:\n";
		foreach(RecordedEvent action in replayData){
			output+= action.time+": "+action.recordedAction+"\n";
		}
		return output;
	}
	public string RecordedDataToString()
	{
		string output = "";
		foreach (RecordedEvent action in replayData)
		{
			output += action.time + "#" + (int)action.recordedAction + "#" + action.position.ToString() + "#" +
				action.rotation.ToString() + "\n";
		}

		Debug.Log (">>>>>>>>" + output);

		return output;
	}
	public List<RecordedEvent> GetEventsList(){
		return replayData;
	}

	/* //Add the right URL to your upload script
public IEnumerator UploadData()
{
WWWForm wwwForm = new WWWForm();
wwwForm.AddField("replayData", RecordedDataToString());
WWW www = new WWW("http://www.YOURSITE.com/uploadData.php", wwwForm);
yield return www;
Debug.Log("Uploaded replay data!");
}
*/


	public void StartRecordingForUI()
	{
		this.StartRecording();
	}

	public Text spawnReplayerText;

	public void ToggleReplayer()
	{
		if (GameRuleManager.instance.replayer == null) 
		{
			spawnReplayerText.text = "리플레이어 삭제";

            GameObject replayerPrefab = ChooseShadowModel();

			if(replayerPrefab != null)
				GameObject.Instantiate(replayerPrefab, replayerPrefab.transform.position, replayerPrefab.transform.rotation);
		} else {
			spawnReplayerText.text = "리플레이어 생성"; 

			Destroy(GameRuleManager.instance.replayer.transform.parent.gameObject);
		}
	}

    GameObject ChooseShadowModel()
    {
        GameObject shadowModel = null;

        string modelName = GameRuleManager.instance.playerMove.animator.name;

        if (modelName.Contains("TT"))
        {
            shadowModel = ResourcesManager.instance.LoadGameObject("Shadow_TT");
        }
        else
        if (modelName.Contains("MUZI"))
        {
            shadowModel = ResourcesManager.instance.LoadGameObject("Shadow_MUZI");
        }
        else 
        if(modelName.Contains("BROWN"))
        {
            shadowModel = ResourcesManager.instance.LoadGameObject("Shadow_BROWN");
        }

        return shadowModel;
    }

	public void ExecuteReplay()
	{
		if (GameRuleManager.instance.replayer != null)
			GameRuleManager.instance.replayer.StartReplay ();
	}

	public Text recordStatusTxt;
	void Update()
	{
		if(recordStatusTxt != null)
		{
			switch (recordStatus) 
			{
				case RecordStatus.recording:
					recordStatusTxt.text = "녹화중"; 
				break;

				case RecordStatus.paused:
					recordStatusTxt.text = "일시정지"; 
				break;

				case RecordStatus.stopped:
					recordStatusTxt.text = "녹화중이 아님"; 
				break;

				default:
					recordStatusTxt.text = "녹화중이 아님"; 
				break;
			}
		}
	}
}

[System.Serializable]
public class RecordedEvent {
	public RecordActions recordedAction;
	public float time;

	public SerializableVector3 position;
//	public Vector3 position;

	public SerializableQuaternion rotation;
//	public Quaternion rotation;
	public int animHash;
}