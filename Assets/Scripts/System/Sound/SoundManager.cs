using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
	new public Transform transform;

    private static SoundManager instance_;

	public static SoundManager instance
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

    private AudioClip Sound;
    private string resourcesPath;
    private AudioSource source_;
	private AudioSource ambient;
	
	public bool bPlayNullSound = true;
	
	public bool isPVP = false;
	
	public int effectPoolCount = 30;

	private float sfxMinDistance = 0F;
	private float sfxMaxDistance = 5F;
	
	public Stack<AudioSource> SFXPool = new Stack<AudioSource>();

	public string missingSoundStr = "";

	public int curPlaySoundLogCount = 10;
	public List<string> currentPlayingSoundStr = new List<string>();

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.CapsLock))
		{
			bPlayNullSound = !bPlayNullSound;
		}

//		if(GameRuleManager.instance.playerMove != null)
//		{
//			this.transform.position = GameRuleManager.instance.player.transform.position;
//		}
	}

    void Awake() 
    {
		instance_ = this;
		transform = GetComponent<Transform>();
		DontDestroyOnLoad(this.gameObject);
		Initialize();
    }
	
	public AudioSource GetAmbient()
	{
		return this.ambient;
	}
	
	public AudioSource GetBGM()
	{
		return this.source_;
	}
	
    void Initialize()
    {
		source_ = gameObject.AddComponent<AudioSource>();
		source_.volume = 1.0F;
		
		GameObject ambientObj = new GameObject("Ambient");
		ambientObj.transform.parent = this.transform;
		
		ambient = ambientObj.AddComponent<AudioSource>();
		
		this.PrepareSFXPool();
    }
	
	void PrepareSFXPool()
	{
		for(int i=0; i<effectPoolCount; i++)
		{
			GameObject SFXPoolObj = new GameObject("SFXPoolObj"); 
			AudioSource SFXPoolSrc = SFXPoolObj.AddComponent<AudioSource>(); 
			SFXPoolSrc.loop = false; 
			SFXPoolSrc.playOnAwake = false; 
			SFXPoolSrc.minDistance = sfxMinDistance;
			SFXPoolSrc.maxDistance = sfxMaxDistance;
			SFXPoolSrc.rolloffMode = AudioRolloffMode.Linear;
			SFXPoolObj.transform.parent = this.transform;
			SFXPool.Push(SFXPoolSrc);
		}		
	}
	
	void PlayClipAtPoint(AudioClip sound, Vector3 pos, float dbVolume = 0.0F, float soundDist= 0.0F, float pitch = 1.0F)
	{
		AudioSource src = this.PopAudioFromPool();
		
		if(src != null)
		{
			src.gameObject.name = "SFXPoolObj(In Use)";
			src.clip = sound;
			src.transform.position = pos;

			if(dbVolume >= 0.0F)
			{
				src.volume = MasterSFXVolume;
			} else {
				src.volume = 1F - (0.083F * (dbVolume * -1F));
			}

			if(soundDist > 0.0F)
			{
				src.maxDistance = soundDist;
			} else {
				src.maxDistance = sfxMaxDistance;
			}

			src.pitch = pitch;
			src.Play();
		
			StartCoroutine(PushAfterAudioComplete(src, sound.length));
		}
	}
	
	AudioSource PopAudioFromPool()
	{
		if(this.SFXPool.Count > 0)
		{
			return this.SFXPool.Pop();
		} else {
//			Debug.LogWarning("Audio Pool All in Use");
			return null;
		}

	}
	
	void PushAudioToPool(AudioSource src)
	{
		this.SFXPool.Push(src);
	}
	
	IEnumerator PushAfterAudioComplete(AudioSource src, float time)
	{
		yield return new WaitForSeconds(time);
		src.gameObject.name = "SFXPoolObj";
		this.PushAudioToPool(src);
	}
	
	public bool MasterBGMAmbientMute = false;
	public bool MasterSFXMute = false;
	
	public float MasterBGMAmbientVolume = 1.0F;
	public float MasterSFXVolume = 1.0F;

	public void SetMasterBGMAmbientVolumeRealTime(float vol)
	{
		MasterBGMAmbientVolume = vol;

		if(source_ != null)
			source_.volume = vol;

		if(ambient != null)
			ambient.volume = vol * .8F;
	}

    public void BGMMute(bool isMute)
    {
        if(source_ != null)
		{
            source_.mute = isMute;
			
			if(MasterBGMAmbientMute == true)
			{
				Debug.Log("Master BGM-Ambient Mute is Set by Option Menu. BGM is Still Mute.");
				source_.mute = true;
			}
		}
    }
	
	public void AmbientMute(bool isMute)
	{
		//Debug.Log("AmbientMute: " + isMute);
		if(ambient != null)
		{
			ambient.mute = isMute;
		
			if(MasterBGMAmbientMute == true)
			{
				Debug.Log("Master BGM-Ambient Mute is Set by Option Menu. Ambient is Still Mute.");
				ambient.mute = true;
			}
		}
	}

    public void EffectMute(bool isMute)
    {
        MasterSFXMute = isMute;
    }

	public void SoundMute( bool isMute ) {

		this.MasterBGMAmbientMute = isMute;
		this.BGMMute( isMute );
		this.AmbientMute( isMute );
		
		this.MasterSFXMute = isMute;
		this.EffectMute( isMute );
	}

    public void PlayBGMSound(string path)
    {
		if(path == "") return;

		if(source_ != null)
		{
			if(source_.clip != null)
			{
				if(source_.clip.name == path && source_.isPlaying == true)
				{
					//Debug.LogWarning("You're requesting to play the sound that is already being played.");
					return; //동일한 음원의 호출이면 무시한다. isLoop이 false일때는 동일한 음원이어도 한차례 처음부터 다시 재생하는것으로 판단한다. 
				}
			}
		}
		
		
        Sound = null;

        if (Sound == null)
            Sound = ResourcesManager.instance.LoadAudioClip(path);

        if (Sound != null)
        {
            if(source_.clip != Sound)
			{
				source_.loop = true;
            	source_.clip = Sound;    
				source_.timeSamples = 0;
				source_.volume = MasterBGMAmbientVolume;
            	source_.Play();
			}
        } else {
			Debug.LogError("No Such Sound Exist! Sound Play Stops: " + path);
			source_.Stop();
		}
        
    }
	
	public void PlayBGMSound(AudioClip clip , bool flag)
	{
        if (clip != null)
        {
            source_.loop = flag;
            source_.clip = clip;
			source_.volume = MasterBGMAmbientVolume;
            source_.Play();
        }
	}
	
	public void PlayMapAmbient(int mapIdx)
	{
	}
	
	public void PlayOverlapAmbient(string path, bool isLoop)
	{
		resourcesPath = "Arcade/"; 
		
		if(ambient != null)
		{
			if(ambient.clip != null)
			{
				if(ambient.clip.name == path && ambient.isPlaying == true && isLoop == true)
				{
					//Debug.LogWarning("You're requesting to play the sound that is already being played.");
					ambient.volume = MasterBGMAmbientVolume * .8F;
					return; //동일한 음원의 호출이면 무시한다. isLoop이 false일때는 동일한 음원이어도 한차례 처음부터 다시 재생하는것으로 판단한다. 
				}
			}
		}
		
        string fullPath = resourcesPath + path;
		
        Sound = ResourcesManager.instance.LoadAudioClip(fullPath);
        if (Sound == null)
        {
            Sound = ResourcesManager.instance.LoadAudioClip(path);
        }
		
		if(Sound == null) //그래도 null 이면
		{
			if(bPlayNullSound == true)
			{
//				Sound = ResourcesManager.instance.LoadAudioClip("BounceBounce");
				Debug.LogWarning("Could not find requested audio file - [" + path + "]! Instead, Bounce Bounce!");
				this.missingSoundStr = path;
			}
		}
		
		if (Sound != null)
        {
            ambient.loop = isLoop;
            ambient.clip = Sound;
			ambient.timeSamples = 0;
			ambient.volume = MasterBGMAmbientVolume * .8F;
			ambient.Play();
        } else {
			Debug.LogError("No Such Sound Exists! Ambient Play Stops: " + path);
			ambient.Stop();
		}
		
		
	}
	
	public void PlaySound(AudioClip clip)
	{
		if( clip != null ) 
			GetComponent<AudioSource>().PlayOneShot( clip );
	}
	
	public void PlayEffectSound(string path , Vector3 pos, float dBVolume = 0.0F, float soundDist = 0.0F, float pitch = 1.0F)
    {
		if (MasterSFXMute == true) return;
		if (path == "")
		{
			Debug.LogWarning("Has not set any sound path to this action. Track down where it came from!");
			this.missingSoundStr = "No Path";
			return;
		}

		resourcesPath = "Arcade/"; 
		
        string fullPath = resourcesPath + path;
		
        Sound = ResourcesManager.instance.LoadAudioClip(fullPath);
        if (Sound == null)
        {
            Sound = ResourcesManager.instance.LoadAudioClip(path);					
        }
		
		if(Sound == null) //그래도 null 이면
		{
			if(bPlayNullSound == true)
			{
				Sound = ResourcesManager.instance.LoadAudioClip("Bounce");
				Debug.LogWarning("Could not find requested audio file - [" + path + "]! Instead, Bounce Bounce!");
				this.missingSoundStr = path;
			}
		}
		
		if(Sound != null)
		{
			this.PlayClipAtPoint(Sound, pos, dBVolume, soundDist, pitch);

			if(this.currentPlayingSoundStr.Count > curPlaySoundLogCount)
				this.currentPlayingSoundStr.Clear();

			this.currentPlayingSoundStr.Add(path);
		} else {
			Debug.LogWarning("No Such SFX Exists: " + path);
		}
			
        
    }
		
	public void PlayEffectSound(AudioSource audioSrc, AudioClip clip)
	{
		//Debug.Log ("#########PlayEffectSound: " + clip.name); 
		//본 함수는 불가피하게 Resources 폴더에서부터 읽어들이는것이 아닌 객체에 붙어있는 음악 파일들을 위해 제작되었습니다. 
		if (MasterSFXMute == true) return;
		
		if(audioSrc != null && clip != null)
		{
			if(clip.name == "Nullsound" && bPlayNullSound == false) return;
			
            audioSrc.PlayOneShot(clip);
			
			
		} else {
			//Debug.LogWarning("AudioSource or Clip is null!: " + audioSrc + " / " + clip);
		}
	}
	
	public void SetBGMVolume(float val)
	{
		if(source_ != null)
			source_.volume = val;
	}
	
	public void SetAmbientVolume(float val)
	{
		if(ambient != null)
			this.ambient.volume = val * .8F;
	}
	
	public void SetSFXVolume(float val)
	{
		this.MasterSFXVolume = val;
	}
	
	public int bgmPauseTime = 0;
	public int ambientPauseTime = 0;
	public void PauseSound(bool flag)
	{
		if(source_ != null)
		{
			if(flag)
			{
				bgmPauseTime = source_.timeSamples;
				source_.Pause(); 
			} else {
				source_.timeSamples = bgmPauseTime;
				source_.Play();
			}
		}
		
		if(ambient != null)
		{
			if(flag)
			{
				ambientPauseTime = ambient.timeSamples;
				ambient.Pause(); 
			} else {
				ambient.timeSamples = ambientPauseTime;
				ambient.Play();
			}
		}
	}
}