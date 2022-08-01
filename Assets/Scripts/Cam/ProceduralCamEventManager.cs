using UnityEngine;
using System.Collections;

[System.Serializable]
public class CamEventTasks
{
	public CameraEvent camEvent;
	public float extraWaitTime;
}

public class ProceduralCamEventManager : Trap 
{
	public CamEventTasks[] tasks;
	public bool taskInProcess;
	public bool loopTasks;
	public float loopInterval;

	void Update()
	{
		if(GameRuleManager.instance.eGameStatus == E_GAME_STATUS.GAME_READY) return;
		if(GameRuleManager.instance.eGameStatus != E_GAME_STATUS.IN_PLAY) return;
		
		if (isTriggered == false)
			return;

		if (!taskInProcess) 
		{
			taskInProcess = true;

			StartCoroutine(ExecuteTask());
		}

	}


	public int taskIndex;
	IEnumerator ExecuteTask()
	{
		for(int i=0; i<tasks.Length; i++)
		{
			taskIndex = i;
			tasks[i].camEvent.SwitchTrigger(true);

			while(tasks[i].camEvent.isTriggered)
			{
				yield return null;
			}

			yield return new WaitForSeconds (tasks[i].extraWaitTime);
		}

		yield return new WaitForSeconds (loopInterval);

		if (loopTasks) 
		{
			isTriggered = true;
		} else {
			isTriggered = false;
		}

		taskInProcess = false;
	}


}
