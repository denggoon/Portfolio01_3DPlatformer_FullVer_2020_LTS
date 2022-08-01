using UnityEngine;
using System.Collections;

public class LogoScreen : MonoBehaviour {

	void Awake()
	{
		if (Application.isEditor)
			Application.runInBackground = true;
	}
	// Use this for initialization
	IEnumerator Start () 
	{

	
		if (UITimelineManager.instance.loopTasks == true) 
		{
			UITimelineManager.instance.loopTasks = false;
		}

		yield return StartCoroutine(UITimelineManager.instance.TaskCoroutine ());

        UnityEngine.SceneManagement.SceneManager.LoadScene ("TitleScene");
	}

}
