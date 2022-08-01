using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* 
 * 각종 UI에 달린 함수들을 리스트에 등록된 순서대로 호출하는 매니저 스크립트입니다.
 */

[System.Serializable]
public class UITask 
{
	/* 
	 * UI의 각 기능을 좀더 편리하게 관리하기 위해 UITask라는 클래스를 운용합니다.
	 */

	public float preDelay; //해당 함수를 실행하기전 대기하는 delay 값입니다.

	public string methodName; //실제로 호출하게되는 함수의 이름입니다.
	public float fltArgs; //함수와 함께 같이 넘겨질 파라메터입니다. (현재는 float만) 

	public GameObject methodObj; //해당 함수를 가지고 있는 객체입니다. 

	public bool waitForCallBack; //해당 함수가 다 끝나서 콜백을 줄때까지 기다릴지 말지를 결정합니다. 
	public float enduranceTime; //기다려도 콜백이 오지 않을경우 몇초를 기다릴지 설정합니다. 기본값 60ms 

	public float postDelay; //함수가 끝나고 몇초를 기다린후 다음 함수로 놈어갈지에 대한 값 설정입니다. 
}

public class UITimelineManager : MonoBehaviour 
{
	public string sendMsgPrefix = "UISendMessage"; 
	//UITimelineManager는 UIMonoBehaviour를 상속받는 클래스들에 한해서 관리하는 스크립트입니다.
	//모든 함수 호출은 UISendMessage에 함수명과 파라메터를 보내는 방식으로 진행됩니다.

//	[SerializeField]
//	private int taskIndexInProgress = 0; //현재 진행중인 태스크의 번호를 표시해줍니다. 

	[SerializeField]
	private bool taskComplete; //현재 진행중인 태스크가 종료되었는지를 표시해줍니다.

	[SerializeField]
	private float enduranceTime; //현재 진행중인 태스크를 몇초째 기다리고 있는지 표시해줍니다.

	public bool startOnAwake;
	public bool loopTasks;
	public List<UITask> lstTasks = new List<UITask>(); //태스크 목록입니다. 

	//외부에서 해당 함수가 종료되었을때 호출하여야할 함수입니다. UITimelineManager.instance.SetComplete()로 호출합니다. 
	public void SetComplete(bool flag)
	{
		taskComplete = flag;
	}

	private static UITimelineManager instance_;
	
	public static UITimelineManager instance
	{
		get
		{
			return instance_;
		}
	}

	void OnDestroy()
	{
		instance_ = null;
	}

	void Awake()
	{
		instance_ = this;
	}

	void Start()
	{
		if (startOnAwake) 
		{
			ExecuteTask();
		}
	}

	public void ExecuteTask()
	{
		StartCoroutine(TaskCoroutine());
	}

	public IEnumerator TaskCoroutine()
	{
		for (int i=0; i<lstTasks.Count; i++) //등록된 태스크의 숫자만큼 기동합니다. 
		{
//			taskIndexInProgress = i; //현재 진행되는 태스크의 번호 설정 
//			Debug.Log(taskIndexInProgress);
			taskComplete = false; //기본적으로 태스크는 완료되어있지 않습니다. 

			UITask task = lstTasks[i];

//			Debug.Log("PreDelay");
			if(task.preDelay > 0F)
			{
				yield return new WaitForSeconds(task.preDelay); //태스크를 시작하기전 설정된 값만큼 대기합니다. 
			}

//			Debug.Log("Message Call: " + task.methodName);
			if(string.IsNullOrEmpty(task.methodName) == false) //태스크 명이 등롣외어 있지 않으면 함수호출을 하지 않습니다. 
			{
				task.methodObj.SendMessage(sendMsgPrefix, task.methodName+"_"+task.fltArgs);
				//태스크명과 함수가 파라메터가 등록되어있는경우 UISendMessage함수를 호출하고 메소드명과 파라메터를 합친 문자열을 보냅니다. 
			}

			if(task.waitForCallBack) //해당 함수가 콜백을 기다리도록 설정되어있는경우 
			{
//				Debug.Log("Wait for Callback");
				enduranceTime = task.enduranceTime; //대기 시간을 설정합니다.

				if(enduranceTime <= 0F) //대기시간이 설정되어있지 않은경우, 기본값은 600입니다. 
					enduranceTime = 600F;

				while (taskComplete == false && enduranceTime >= 0F) //태스크가 완료되었거나 대기시간이 0이 될때까지 
				{
					enduranceTime -= Time.deltaTime; //기다립니다. 
					yield return null;
				}

				enduranceTime = 0F; //태스크가 종료되면 태스크 종료 표시및 시간을 초기화 합니다. 
				taskComplete = true;
			}

//			Debug.Log("PostDelay");
			if(task.postDelay > 0F) //태스크 종료후 설정된 시간만큼 대기합니다. 
			{
				yield return new WaitForSeconds(task.postDelay);
			}
		}

		if (loopTasks) 
		{
			StartCoroutine(TaskCoroutine ());
		}
	}
}
