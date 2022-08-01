using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.Networking;

public class BankLoader : MonoBehaviour {

	private bool localFileExists = false;
	public bool bankLoadSuccessful = false;

	public string downloadPath;
	public string statusMsg;
	public float progress;

	string webPath = "http://192.168.1.56/bundles/filebundles/banks/";
	string commonPath;
	string bankDirectory;

	void Awake()
	{
		this.enabled = false;
	}
	
	public IEnumerator Execute () 
	{
		if (DataFileManager.instance != null) 
		{
			if (DataFileManager.instance.eFileLoadMode == FILE_LOAD_MODE.OFFLINE) //온라인에서 받는 방식이 아닌 APK에 포함되어있는 뱅크파일을 불러오는경우 뱅크 로딩을 할필요가 없습니다. 
				yield break;
		}

		this.enabled = true;

		commonPath = Application.persistentDataPath;
		bankDirectory = "/banks/";
		
		yield return StartCoroutine (BankLoad());

		this.enabled = false;
	}

	void Update()
	{
		PreSceneDataLoader.instance.uiTxtProgress.text = statusMsg;
	}
	
	IEnumerator BankLoad()
	{
		if (File.Exists (commonPath + bankDirectory + "FMOD_bank_list.txt")) 
		{
			Debug.Log("Local Bank List Exists");
			statusMsg = "기기에 저장된 뱅크 파일 목록이 존재합니다."; 
			if(Application.platform == RuntimePlatform.WindowsEditor)
			{
				downloadPath = "file:///"+commonPath + bankDirectory;
			} else {
				downloadPath = commonPath + bankDirectory;
			}
			
			localFileExists = true;
			
		} else {
			Debug.Log("No Local Bank List, Download.");
			statusMsg = "기기에 저장된 뱅크 파일 목록이 존재하지 않습니다. 새로 다운로드 합니다."; 
			downloadPath = webPath;
			Directory.CreateDirectory (commonPath + bankDirectory);
		}
		
		string[] strBankList;
		using (UnityWebRequest wwwBankList = new UnityWebRequest(downloadPath + "FMOD_bank_list.txt")) 
		{
			if(Application.platform == RuntimePlatform.IPhonePlayer)
			{
				statusMsg = "뱅크 리스트 다운로드중";  //iOS에서는 while .isDone 이 제대로 작동하지 않는 보고가 있음. 
				yield return StartCoroutine(new WWWRequest(wwwBankList));

			} else {

				do {  
					float bankListProgress = wwwBankList.downloadProgress * 100F;
					progress = bankListProgress;

					int bankListPRounded = Mathf.RoundToInt (bankListProgress);
				
					statusMsg = "뱅크 리스트 다운로드중 :" + bankListPRounded + "%"; 
					yield return null;  
				} while(!wwwBankList.isDone);  
			}
		
			if (wwwBankList.error != null) {  
				Debug.LogError ("wwwBankList.error=" + wwwBankList.error);  
				statusMsg = "에러:" + wwwBankList.error; 
				yield break;  
			}
		
			if (localFileExists == false)
				File.WriteAllBytes (commonPath + bankDirectory + "FMOD_bank_list.txt", wwwBankList.downloadHandler.data);
		
			strBankList = wwwBankList.url.Split ("\n" [0]);
		}
		
		for (int i=0; i<strBankList.Length-1; i++) 
		{
			string strBankName = strBankList[i];
			Debug.Log("BankName: " + strBankName);
			
			if(File.Exists(commonPath + bankDirectory + strBankName) == false)
			{
				using(UnityWebRequest wwwBank = new UnityWebRequest(downloadPath+strBankName))
				{
					do  
					{  
						float bankProgress = wwwBank.downloadProgress * 100F;
						progress = bankProgress;

						int bankPRounded = Mathf.RoundToInt(bankProgress);
							
						statusMsg = "뱅크 파일 다운로드중:" + strBankName + "(" + bankPRounded + "%)"; 
						yield return null;  
					}while(!wwwBank.isDone);  
					
					if(wwwBank.error != null)  
					{  
						bankLoadSuccessful = false;
						Debug.LogError("wwwBank.error, " + strBankName + "=" +wwwBank.error);  
						statusMsg = "뱅크 파일을 다운로드 받는 도중 문제가 발생했습니다 :" + strBankName + " - " + wwwBank.error; 
						yield break;  
					} else {
						Debug.Log ("Writing " + strBankName + " to: " + commonPath + bankDirectory);
						File.WriteAllBytes (commonPath + bankDirectory + strBankName, wwwBank.downloadHandler.data);
					}
				}
			}
		}

		bankLoadSuccessful = true;
	}
}
