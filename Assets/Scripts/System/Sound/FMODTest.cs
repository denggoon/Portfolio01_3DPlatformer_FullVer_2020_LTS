using UnityEngine;
using System.Collections;

public class FMODTest : MonoBehaviour {

	FMOD.Studio.EventDescription fmodEventDesc;
	FMOD.Studio.EventInstance fmodEvent;

	// Use this for initialization
	void Start ()
	{
		fmodEventDesc = FMODUnity.RuntimeManager.GetEventDescription("event:/Monster/Bo_Shoot_Move");

		fmodEventDesc.createInstance(out fmodEvent);

		fmodEvent.start ();
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.Delete))
		{
			//FMOD.Studio.CueInstance cue;

			//fmodEvent.getCue("KeyOff", out cue);

			//cue.trigger();

			bool IsKeyOffExists = false;
			fmodEventDesc.hasSustainPoint(out IsKeyOffExists);

			if(IsKeyOffExists)
            {
				fmodEvent.keyOff();
            }
		}
	}
}
