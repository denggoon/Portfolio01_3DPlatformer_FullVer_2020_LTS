using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(SoundEventCaller))]
public class SoundEventCallerEditor : Editor
{
	string[] _choices = new [] {"None", "OneShot", "Loop"};
	
	public override void OnInspectorGUI()
	{
		SoundEventCaller caller = target as SoundEventCaller;
		
		caller.type = EditorGUILayout.Popup("type", caller.type, _choices);
		
		switch (caller.type) {
		case 0:
			break;
			
		case 1:
			caller.asset = (FMODAsset)EditorGUILayout.ObjectField("asset", caller.asset, typeof(FMODAsset), true);
			caller.delayTime = EditorGUILayout.FloatField("delayTime", caller.delayTime);
			break;

		case 2:
			caller.asset = (FMODAsset)EditorGUILayout.ObjectField("asset", caller.asset, typeof(FMODAsset), true);
			caller.delayTime = EditorGUILayout.FloatField("delayTime", caller.delayTime);
//			caller.isStartPlay = EditorGUILayout.Toggle("isStartPlay", caller.isStartPlay);
			caller.actionCueTime = EditorGUILayout.FloatField("actionCueTime", caller.actionCueTime);

			break;
			
		default:
			break;
		}
		
		if (GUI.changed)
			EditorUtility.SetDirty(caller);
	}
}
#endif
public enum SOUND_TYPE
{
	NONE = 0,
	ONESHOT = 1,
	LOOP = 2,
}

public class SoundEventCaller : MonoBehaviour {

	public int type = (int)SOUND_TYPE.NONE;
	public FMODAsset asset;
//	public bool isStartPlay = false;
	public float delayTime = 0.0f;
	public float actionCueTime = 0.0f;

	private Vector3 position = Vector3.zero;
	private FMOD.Studio.EventInstance fmodSoundEvent;
	
	public void PlaySound (Vector3 pos) {
		if (FMODSoundManager.instance == null)
			return;
		if (type == (int)SOUND_TYPE.NONE)
			return;

		position = pos;
		Invoke ("ActionSound", delayTime);
	}

	private void ActionSound() {

		switch (type) {
			case (int)SOUND_TYPE.ONESHOT :
				FMODSoundManager.instance.PlayOneShotAtPoint (asset.id, position);
				break;
				
			case (int)SOUND_TYPE.LOOP :
				fmodSoundEvent.stop (FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
				fmodSoundEvent = SoundBoard.instance.PlayLoopSoundFromBoard("", this.gameObject);


				break;
				
			default :
				break;
		} 
	}

	private void ActionCue () {

		if (type == (int)SOUND_TYPE.LOOP) 
		{
			//FMOD.Studio.CueInstance cue;
			//fmodSoundEvent.getCue ("KeyOff", out cue);
			//if ( cue != null ) {
			//	cue.trigger ();
			//}

			FMOD.Studio.EventDescription desc;

			fmodSoundEvent.getDescription(out desc);

			bool IsKeyOffExists = false;
			desc.hasSustainPoint(out IsKeyOffExists);

			if (IsKeyOffExists)
			{
				fmodSoundEvent.keyOff();
			}
		}
	}

//	void Start() {
//		if (type == (int)SOUND_TYPE.LOOP && isStartPlay == true) {
//			PlaySound(this.transform.position);
//		}
//	}

	void Update() {

		if (type == (int)SOUND_TYPE.LOOP) {

			fmodSoundEvent.set3DAttributes (FMODUnity.RuntimeUtils.To3DAttributes(this.gameObject));

			if (actionCueTime > 0) {
				actionCueTime -= Time.deltaTime;
			} else if ( actionCueTime < 0 ) {
				actionCueTime = 0.0f;
				ActionCue();
			}
				                               
		}
	}

	void OnDisable()
	{
		ActionCue ();	
	}

}
