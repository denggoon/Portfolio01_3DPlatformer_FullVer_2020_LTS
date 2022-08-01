using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_IOS
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;
#endif

public class MainMenu : MonoBehaviour {

	public GameObject pnlClickPrevention;
	public GameObject pnlStageInfoPanel;
	public GameObject pnlRegisterUser;
	public GameObject pnlUserLogin;
    public GameObject pnlUserStatus;

	public Button btnLogOut;
	public Button btnRegisterUser;
	public Button btnUnregisterDev;

	public InputField iptFieldRegUserName;
	public Button btnConfirmRegister;

	public InputField iptFieldLoginUserName;
	public Button btnConfirmLogin;

	public Text txtLoginUser;
	public Text txtRecentRegUser;
	public Text txtDevice;

	public Text txtStageName;

    public Button btnSocial;
    public Button btnCash;
    public Button btnMoney;
    public Button btnMessageUnread;
    

	//유저 등록 
	public void ShowUserRegisterPopup()
	{
		pnlClickPrevention.SetActive (true);
		pnlRegisterUser.SetActive (true);

	}

	public void CloseUserRegisterPopup()
	{
		pnlClickPrevention.SetActive (false);
		pnlRegisterUser.SetActive (false);

		iptFieldRegUserName.text = "";
	}

	//유저 로그인
	public void ShowUserLoginPopup()
	{
		pnlClickPrevention.SetActive (true);
		pnlUserLogin.SetActive (true);
		
	}
	
	public void CloseUserLoginPopup()
	{
		pnlClickPrevention.SetActive (false);
		pnlUserLogin.SetActive (false);
		
		iptFieldLoginUserName.text = "";
	}
	
	public void CheckLoginUserInput()
	{
		if (string.IsNullOrEmpty(iptFieldLoginUserName.text) || iptFieldLoginUserName.text == "") 
		{
			UIPopupMsgManager.instance.PopMessage("유저명을 입력하지 않았습니다");
		} else {
            
		}
	}

    private void SetLoadingStageAfterRecordLoading(string stageName)
    {
        PlayerPrefs.SetString("LoadingSceneName", stageName);

        pnlClickPrevention.SetActive(true);
        pnlStageInfoPanel.SetActive(true);

        txtStageName.text = stageName;
    }

	//스테이지 패널 오픈 
	public void SetLoadingStageName(string stageName)
	{
//        var req = new LoadStageRecord(stageName);
//        req.OnSucceeded += (sender, e) =>
//        {
//            var rankingTxt = pnlStageInfoPanel.transform.Find("2_pnlRanking/2_txtRanking").GetComponentInChildren<Text>();
//
//            rankingTxt.text = "";
//
//            if (e.result.rankingList.Length == 0)
//            {
//                rankingTxt.text = "None";
//            }
//
//            foreach(var ranking in e.result.rankingList) 
//            {
//                rankingTxt.text += ranking.rank + ": " + ranking.name + "(" + ranking.score + ")\n";
//            }
//            
//        };

//        var handler = new RequestHandler(req);
//        handler.Post();

//        LoadAllStageRecords();

        // QQQ 서버 연결과 상관없이 일단 진행을 하자 by kks
        SetLoadingStageAfterRecordLoading(stageName);
	}

    private void LoadAllStageRecords()
    {
        
    }

	public void CloseStagePanel()
	{
		pnlClickPrevention.SetActive (false);
		pnlStageInfoPanel.SetActive (false);
	}

	public void LoadStage()
	{
		SceneManager.LoadScene("LoadingScene");
	}

    public void ShowUserStatus(bool flag)
    {
        pnlUserStatus.SetActive(flag);
    }

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) //안드로이드 에서의 백버튼을 누르면 종료된다. 
		{
			Application.Quit();
		}
	}


}
