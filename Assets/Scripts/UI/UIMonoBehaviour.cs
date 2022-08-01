using UnityEngine;
using System.Collections;

public class UIMonoBehaviour : MonoBehaviour 
{
	/* 
	 * 모든 UI효과 관련 함수를 소유하고 있는 스크립트들은 본 스크립트를 상속 받기를 권장합니다. 
	 */

	public virtual void UISendMessage (string methodNameArgs) //본 스크립트를 상속받는 스크립트에서 전달받은 함수명을 찾아 파라메터를 전달하여 실행합니다. 
	{
		string[] splitSeperator = {"_"}; //전달받은 문자열은 _ 으로 함수명과 파라메터가 구분되어있습니다. 
		string[] methodInfo = methodNameArgs.Split(splitSeperator, System.StringSplitOptions.RemoveEmptyEntries);

		if (methodInfo.Length == 2) 
		{
			SendMessage (methodInfo [0], (float)System.Convert.ToDouble(methodInfo [1])); //파라메터가 있으면 float으로 변환후 실행 
		} else {
			SendMessage (methodInfo [0]); //파라메터가 없으면 그냥 실행 
		}
	}

	public virtual void CallBackCompletion() //함수가 종료되었을경우 본 함수를 부르도록 권장합니다. 
	{
		if(UITimelineManager.instance == null) return;

		UITimelineManager.instance.SetComplete (true); //함수가 종료되었음을 알려줍니다. 
	}

}
