using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIPopupMsgManager : MonoBehaviour {

	private static UIPopupMsgManager instance_;
//	private new Transform transform;

	public static UIPopupMsgManager instance
	{
		get
		{
			if (instance_ == null) 
			{
				GameObject goPopupMsgManager = (GameObject)GameObject.Instantiate(ResourcesManager.instance.ResourcesLoadCached("_UIPopupManagerCanvas"), Vector3.zero, Quaternion.identity);

				instance_ = goPopupMsgManager.GetComponent<UIPopupMsgManager>();
			}
		
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
//		transform = GetComponent<Transform> ();

		closeClickPrevent	= () => {pnlClickPrevention.SetActive(false);};
		closeAlertMessage 	= () => {pnlAlertMessage.SetActive(false);};
		closeQuestion 		= () => {pnlQuestion.SetActive(false);};

		//		btnAlertConfirm.onClick.AddListener
		//		(
		//			delegate
		//			{
		//				ToggleClickPreventPanel (false);
		//			}
		//		);
	}

	public GameObject pnlClickPrevention;
	private UnityEngine.Events.UnityAction closeClickPrevent;
	
	public GameObject pnlAlertMessage;
	public Text txtAlertMessage;
	private UnityEngine.Events.UnityAction closeAlertMessage;

	public Button btnAlertConfirm;
	
	public GameObject pnlQuestion;
	public Text txtQuestion;
	private UnityEngine.Events.UnityAction closeQuestion;

	public Button btnQuestionConfirm;
	public Button btnQuestionCancel;

	public void PopMessage(string msg, System.Action action = null) //UnityEngine.Events.UnityAction 
	{
		btnAlertConfirm.onClick.RemoveAllListeners ();
		btnAlertConfirm.onClick.AddListener (closeClickPrevent); //인스펙터에 표시되지 않지만 등록이 됩니다. 
		btnAlertConfirm.onClick.AddListener (closeAlertMessage);

		if(action != null)
			btnAlertConfirm.onClick.AddListener
			(
				delegate {
					action();
				}
			);

		pnlClickPrevention.SetActive (true);
		pnlAlertMessage.SetActive (true);
		txtAlertMessage.text = msg;


	}

	public void PopQuestion(string msg, UnityEngine.Events.UnityAction yesAction = null, UnityEngine.Events.UnityAction noAction = null)
	{
		btnQuestionConfirm.onClick.RemoveAllListeners ();
		btnQuestionCancel.onClick.RemoveAllListeners ();

		btnQuestionConfirm.onClick.AddListener (closeClickPrevent);
		btnQuestionCancel.onClick.AddListener (closeClickPrevent);

		btnQuestionConfirm.onClick.AddListener (closeQuestion);
		btnQuestionCancel.onClick.AddListener (closeQuestion);

		if(yesAction != null)
			btnQuestionConfirm.onClick.AddListener(yesAction);

		if(noAction != null)
			btnQuestionCancel.onClick.AddListener(noAction);

		pnlClickPrevention.SetActive (true);
		pnlQuestion.SetActive (true);
		txtQuestion.text = msg;
	}
}
