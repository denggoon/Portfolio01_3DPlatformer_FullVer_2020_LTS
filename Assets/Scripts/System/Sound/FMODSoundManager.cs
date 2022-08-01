using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;

public class FMODSoundManager : MonoBehaviour
{
	private static FMODSoundManager instance_;

	public static FMODSoundManager instance
	{
		get
		{
			return instance_;
		}
	}

	void OnDestoy()
	{
		Debug.Log ("Destoyed!");
		StopBGM ();
		StopCondBGM ();
		StopAmbient ();
		instance_ = null;
	}

	new public Transform transform;

	private EventInstance BGMsource_;
	private EventInstance condBGMsrc_;
	private EventInstance ambient_;
	
	public bool bPlayNullSound = true;
	public string missingSoundStr = "";

	public bool MasterBGMAmbientMute = false;
	public bool MasterSFXMute = false;
	
	public float MasterBGMAmbientVolume = 1.0F;
	public float MasterSFXVolume = 1.0F;

	public FMOD.Studio.System fmodStudioSys;
	FMOD.System fmodLowLvSys;

	void Awake() 
	{
		instance_ = this;
		transform = GetComponent<Transform>();

		fmodStudioSys = RuntimeManager.StudioSystem;
		fmodLowLvSys = RuntimeManager.CoreSystem;

		if (SoundBoard.instance.soundInfoList.Count != 0)
		{
			foreach (var soundInfo in SoundBoard.instance.soundInfoList)
			{
				fmodStudioSys.lookupID("event:/" + soundInfo.eventPath, out soundInfo.GUID);
			}
		}

		FMOD.OUTPUTTYPE outType;

        FMOD.RESULT result = fmodLowLvSys.getOutput(out outType);

        Debug.Log("###########Prev FMOD Output Type: " + outType.ToString());

        if (Application.platform == RuntimePlatform.Android)
        {
            fmodLowLvSys.setOutput(FMOD.OUTPUTTYPE.OPENSL);

            FMOD.RESULT newResult = fmodLowLvSys.getOutput(out outType);

            Debug.Log("###########New FMOD Output Type: " + outType.ToString());
        }
    }

	void OnDestroy()
	{
		BGMsource_.stop (FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

		ambient_.stop (FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

		condBGMsrc_.stop (FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

		instance_ = null;
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.CapsLock))
		{
			bPlayNullSound = !bPlayNullSound;
		}
	}
	
	public EventInstance GetAmbient()
	{
		return this.ambient_;
	}
	
	public EventInstance GetBGM()
	{
		return this.BGMsource_;
	}

	public EventInstance GetCondBGM()
	{
		return this.condBGMsrc_;
	}
	public void BGMMute(bool isMute)
	{
		Bus bgmMuteBus;

		fmodStudioSys.getBus("bus:/Music", out bgmMuteBus);

		bgmMuteBus.setMute (isMute);
	}
	
	public void AmbientMute(bool isMute)
	{
		Bus ambMuteBus;

		fmodStudioSys.getBus("bus:/Ambience", out ambMuteBus);
		
		ambMuteBus.setMute (isMute);
	}
	
	public void EffectMute(bool isMute)
	{
		MasterSFXMute = isMute;

		Bus sfxMuteBus;

		fmodStudioSys.getBus("bus:/FX", out sfxMuteBus);

		sfxMuteBus.setMute (isMute);

	}
	
	public void SoundMute( bool isMute ) 
	{
		this.MasterBGMAmbientMute = isMute;
		this.BGMMute( isMute );
		this.AmbientMute( isMute );
		
		this.MasterSFXMute = isMute;
		this.EffectMute( isMute );
	}

	public EventInstance PlaySoundAndReturnEvent(string path, GameObject obj)
	{
		if (path == "")
		{
			Debug.LogWarning("Has not set any sound path to this action. Track down where it came from!");
			this.missingSoundStr = "No Path";
			return default(FMOD.Studio.EventInstance);
		}

		EventDescription fmodEventDesc;
		EventInstance fmodEvent;
		if (path.Contains ("{")) 
		{
			fmodStudioSys.getEvent(path, out fmodEventDesc);
		} else {

			fmodStudioSys.getEvent("event:/" + path, out fmodEventDesc);
		}

		fmodEventDesc.createInstance(out fmodEvent);

		FMOD.ATTRIBUTES_3D attrib3D = FMODUnity.RuntimeUtils.To3DAttributes(obj);
		
		fmodEvent.set3DAttributes(attrib3D);
		fmodEvent.start ();
		return fmodEvent;
		
	}
	
	public void PlayBGMSound(string path)
	{
		if (path == "")
		{
			Debug.LogWarning("Has not set any sound path to this action. Track down where it came from!");
			this.missingSoundStr = "No Path";
			return;
		}

		EventDescription bgmDesc;

		fmodStudioSys.getEvent("event:/" + path, out bgmDesc);

		bgmDesc.createInstance(out BGMsource_);

		BGMsource_.start ();
	}

	public void StopBGM(FMOD.Studio.STOP_MODE mode = FMOD.Studio.STOP_MODE.IMMEDIATE)
	{
		BGMsource_.stop(mode);
	}


	public void PlayOverlapAmbient(string path, bool isLoop)
	{
		if (path == "")
		{
			Debug.LogWarning("Has not set any sound path to this action. Track down where it came from!");
			this.missingSoundStr = "No Path";
			return;
		}

		EventDescription ambientDesc;

		fmodStudioSys.getEvent("event:/" + path, out ambientDesc);

		ambientDesc.createInstance(out ambient_);

		ambient_.start ();
	}

	public void StopAmbient(FMOD.Studio.STOP_MODE mode = FMOD.Studio.STOP_MODE.IMMEDIATE)
	{
		ambient_.stop(mode);
	}

	public void PlayCondBGMSound(string path)
	{
		if (path == "")
		{
			Debug.LogWarning("Has not set any sound path to this action. Track down where it came from!");
			this.missingSoundStr = "No Path";
			return;
		}

		EventDescription condBGMDesc;

		fmodStudioSys.getEvent("event:/" + path, out condBGMDesc);

		condBGMDesc.createInstance(out condBGMsrc_);

		condBGMsrc_.start ();
	}

	public void StopCondBGM(FMOD.Studio.STOP_MODE mode = FMOD.Studio.STOP_MODE.IMMEDIATE)
	{
		condBGMsrc_.stop (mode);
	}
	
	public void PlayOneShotAtPoint(string path , Vector3 pos)
	{
		if (MasterSFXMute == true) return;
		if (path == "")
		{
			Debug.LogWarning("Has not set any sound path to this action. Track down where it came from!");
			this.missingSoundStr = "No Path";
			return;
		}

		EventDescription oneShotDesc;
		EventInstance oneShotEvent;

		if (path.Contains ("{")) 
		{
			fmodStudioSys.getEvent(path, out oneShotDesc);

		} else {
			fmodStudioSys.getEvent("event:/" + path, out oneShotDesc);
		}

		oneShotDesc.createInstance(out oneShotEvent);

		oneShotEvent.set3DAttributes(RuntimeUtils.To3DAttributes(pos));
		oneShotEvent.start();
		oneShotEvent.release();
	}

	public void SetBGMVolume(float val)
	{

	}
	
	public void SetAmbientVolume(float val)
	{

	}
	
	public void SetSFXVolume(float val)
	{
		this.MasterSFXVolume = val;
	}
}