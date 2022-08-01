using UnityEngine;
using System.Collections;

public class RotateObject : MonoBehaviour {

	new public Transform transform;  
	public Vector3 axisSpeed;
	public float speed = 2;
	
	public string[] noisePaths;
	public float intervalTime;

	new public Renderer renderer;
	private bool isVisible = true;
	public bool isMakingNoise = false;

	void Awake()
	{
		transform = GetComponent<Transform>();
		renderer = GetComponent<Renderer>();
	}
	void Start()
	{
		StartCoroutine(IntervalNoiseMaker(intervalTime));
//		isVisible = false;
	}

	IEnumerator IntervalNoiseMaker(float interval = 0F)
	{
		if(noisePaths == null || noisePaths.Length == 0) yield break;
		if(interval == 0F) yield break;

		isMakingNoise = true;

		while(true)
		{
			MakeNoise();
			yield return new WaitForSeconds(interval);
		}
	}

	void Update()
	{
//		IsVisibleFromCamera();

		if(isVisible == false) return;

		transform.Rotate( axisSpeed * speed * Time.deltaTime);
	}

	void IsVisibleFromCamera()
	{
		if(renderer.IsVisibleFrom(Camera.main))
		{
			isVisible = true;
			
			//			if(isMakingNoise == false)
			//			{
			//				StartCoroutine("IntervalNoiseMaker", intervalTime);
			//			}
			
		} else {
			isVisible = false;
			
			//			if(isMakingNoise == true)
			//			{
			//				isMakingNoise = false;
			//				StopCoroutine("IntervalNoiseMaker");
			//			}
		}
	}

	void MakeNoise()
	{
//		if(SoundBoard.instance != null)
//			SoundBoard.instance.PlayFromSoundBoard("SND_GMK_006_LOOP", this.transform.position);
	}
	
	public string ChooseRandomSoundFX()
	{
		if(noisePaths == null || noisePaths.Length == 0) return "";
		
		if(noisePaths.Length == 1)
		{
			return noisePaths[0];
		}
		
		int random = Random.Range(0, noisePaths.Length);
		
		return noisePaths[random];
	}
}
