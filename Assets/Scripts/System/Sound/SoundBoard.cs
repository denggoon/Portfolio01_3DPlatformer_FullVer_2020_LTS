using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

[System.Serializable]
public class SoundInfo
{
	public string soundID;
	public string eventPath;
	public FMOD.GUID GUID;
}

public class SoundBoard : MonoBehaviour
{
	public List<SoundInfo> soundInfoList;

	public TextAsset balanceFile;

	private static SoundBoard instance_;
	
	public static SoundBoard instance
	{
		get
		{
			return instance_;
		}
	}
	
	void Awake() 
	{
		instance_ = this;
		
		JsonMapper.RegisterImporter<double, float>(DoubleToFloat);
		
		if(balanceFile != null)
		{
			JsonData json = JsonMapper.ToObject(balanceFile.text);
			soundInfoList = JsonMapper.ToObject<List<SoundInfo>>(json["root"].ToJson());
		}
	}
	
	float DoubleToFloat(double val)
	{
		return (float)val;
	}
	
	double FloatToDouble(float val)
	{
		return (double)val;
	}
	
	void OnDestroy()
	{
		instance_ = null; 
	}

	public virtual void PlayFromSoundBoard(string givenID)
	{
		this.PlayFromSoundBoard (givenID, this.transform.position);
	}
	
	public virtual void PlayFromSoundBoard(string givenID, Vector3 pos)
	{
//		Debug.Log (givenID);

		if(givenID.Contains("alpha_"))
			givenID = InterpreteAlphaSceneName (givenID);

		SoundInfo soundinfo = GetPlayerSoundInfo(givenID);
		
		if(soundinfo == null) return;
				
		PlaySoundInfo (soundinfo, pos, (givenID.Contains ("_BGM_") || givenID.Contains ("_AMB_")));
	}

	public virtual FMOD.Studio.EventInstance PlayLoopSoundFromBoard (string givenID, GameObject obj)
	{
		SoundInfo soundInfo = GetPlayerSoundInfo(givenID);
		
		if(soundInfo == null) 
			return default(FMOD.Studio.EventInstance);

		if (FMODSoundManager.instance != null) 
		{
			return FMODSoundManager.instance.PlaySoundAndReturnEvent (soundInfo.eventPath, obj);
		}

		return default(FMOD.Studio.EventInstance);
	}

	public string InterpreteAlphaSceneName(string sceneName)
	{
		string[] splitStr = new string[0];
		string[] bgmSeperator = new string[]{"SND_BGM_"};
		string[] ambSeperator = new string[]{"SND_AMB_"};

		if(sceneName.Contains("_BGM_"))
			splitStr = sceneName.Split (bgmSeperator, System.StringSplitOptions.RemoveEmptyEntries);

		if(sceneName.Contains("_AMB_"))
			splitStr = sceneName.Split (ambSeperator, System.StringSplitOptions.RemoveEmptyEntries);

		if (splitStr.Length != 1) 
		{
			return sceneName;
		}

		string modifiedStr = splitStr [0];

		modifiedStr = modifiedStr.Substring (0, modifiedStr.Length - 2);

		modifiedStr = modifiedStr.Replace ("alpha_", "W1S");

		if(sceneName.Contains("_BGM_"))
			return bgmSeperator [0] + modifiedStr;

		if(sceneName.Contains("_AMB_"))
			return ambSeperator [0] + modifiedStr;

		return sceneName;

	}
	
	public virtual SoundInfo GetPlayerSoundInfo(string givenID)
	{
		for(int i=0; i<soundInfoList.Count; i++)
		{
			if(soundInfoList[i].soundID.Equals(givenID))
			{
				return soundInfoList[i];
			}
		}

		return null;
	}
	
	protected virtual void PlaySoundInfo(SoundInfo soundInfo, Vector3 pos, bool isBGM)
	{
		if(soundInfo == null) return;

		if (FMODSoundManager.instance != null) 
		{
			if(isBGM)
			{ 
				if(soundInfo.eventPath.Contains("_Cond"))
				{
					FMODSoundManager.instance.PlayCondBGMSound (soundInfo.eventPath);
				} else {
					FMODSoundManager.instance.PlayBGMSound (soundInfo.eventPath);
				}
			}else {
				FMODSoundManager.instance.PlayOneShotAtPoint (soundInfo.eventPath, pos);
			}
		}
	}
}
