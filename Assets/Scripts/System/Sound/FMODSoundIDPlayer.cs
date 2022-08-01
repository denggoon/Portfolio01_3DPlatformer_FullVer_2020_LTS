using UnityEngine;
using FMODUnity;

public class FMODSoundIDPlayer : MonoBehaviour 
{
	public string soundID;

	void Start()
	{
		if (!string.IsNullOrEmpty (soundID)) 
		{
			SoundInfo soundInfo = SoundBoard.instance.GetPlayerSoundInfo(soundID);

			if(soundInfo != null)
			{
                //FMOD_StudioEventEmitter emitter = this.gameObject.AddComponent<FMOD_StudioEventEmitter>();
                //emitter.path = "event:/" + soundInfo.eventPath;

                StudioEventEmitter emitter = gameObject.AddComponent<StudioEventEmitter>();
				//EventReference eventRef = EventReference.Find(soundInfo.eventPath);
				//if(eventRef.IsNull)
				//            {
				//	eventRef = EventReference.Find("event:/" + soundInfo.eventPath);
				//}
				emitter.PlayEvent = EmitterGameEvent.ObjectStart;
				emitter.StopEvent = EmitterGameEvent.ObjectDestroy;

				emitter.EventReference.Guid = soundInfo.GUID;
                emitter.EventReference.Path = "event:/" + soundInfo.eventPath;

			} else {
				Debug.Log("null");
			}
		}
	}
}
