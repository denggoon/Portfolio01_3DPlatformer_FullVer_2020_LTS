using UnityEngine;
using System.Collections;

public class SideScrollCamera : MonoBehaviour 
{
	new public Transform transform;

	public Camera cam;

	public GameObject moveStandardObj;
	private Vector3 qtrViewAngle = new Vector3(45F, 45F, 0F);
	private Vector3 quaterViewPos;

//	public Rect ScreenRect;
//	private float heightPercentage = .85F;
//	public Vector3 playerViewPos;
//	public bool isOffScreen = false;

	public Vector3 distancePos;
	public Vector3 calibratedPos;

	public PlayerMoveCC playerMove;

	public float YSmoothSpeed = 4.0F;
	public float XSmoothSpeed = 1.0F;
	public float smoothCorrectionValue;

	public Vector3 slidePos;
	public float slideRate;
	public float maxCamSlideLength;

	void Awake()
	{
		transform = GetComponent<Transform>();
	}

	// Use this for initialization
	void Start () {

		cam = GetComponent<Camera> ();

//		ScreenRect = new Rect(0F, 0F + Screen.height - (Screen.height * heightPercentage) , Screen.width, Screen.height * heightPercentage);

		XSmoothSpeed = 1F;
		YSmoothSpeed = 3F;
		slideRate = 15F;
		maxCamSlideLength = 0F;

		quaterViewPos = new Vector3 (-5.5F, 7.3F, -5.5F);

		SetToDefaultCamera ();
	}

	[SerializeField]
	private bool isCameraFunctionHeld = false;
	public void HoldCameraFunction(bool flag)
	{
		isCameraFunctionHeld = flag;
	}


	void Update()
	{
		if (playerMove == null) return;

		if (isCameraFunctionHeld)
			return;

		Vector3 newSlidePos = Vector3.zero;

		if (playerMove.faceDirection != Vector3.zero) 
		{
			slidePos += playerMove.faceDirection * slideRate * Time.deltaTime; 
		}

		slidePos = Vector3.ClampMagnitude (slidePos, maxCamSlideLength);

		slidePos = Vector3.Lerp(slidePos, Vector3.zero, Time.deltaTime * XSmoothSpeed);

		CameraFollow ();
	}

	public void SetToDefaultCamera()
	{
		SetCameraEulerAngle (qtrViewAngle);
		SetCameraDistance (quaterViewPos);
	}

	public void SetCameraEulerAngle(Vector3 givenAngle)
	{
		if (moveStandardObj == null) 
		{
			moveStandardObj = new GameObject ("StandardViewObj");
		}

		if (moveStandardObj != null) 
		{
			moveStandardObj.transform.position = Vector3.zero;
			moveStandardObj.transform.eulerAngles = new Vector3 (0F, givenAngle.y, 0F);
		}

		cam.transform.eulerAngles = givenAngle;
	}

	public void SetCameraQuaternionAngle(Quaternion giveRotation)
	{
		if (moveStandardObj == null) 
		{
			moveStandardObj = new GameObject ("StandardViewObj");
		}
		
		if (moveStandardObj != null) 
		{
			moveStandardObj.transform.position = Vector3.zero;

			Vector3 euler = giveRotation.eulerAngles;
			moveStandardObj.transform.eulerAngles = new Vector3 (0F, euler.y, 0F);
		}
		
		cam.transform.rotation = giveRotation;
	}

	public void SetCameraDistance(Vector3 givenDist)
	{
		distancePos = givenDist;
	}

	void QuarterView()
	{
		if (moveStandardObj == null) 
		{
			moveStandardObj = new GameObject ("StandardViewObj");
			moveStandardObj.transform.position = Vector3.zero;
			moveStandardObj.transform.eulerAngles = new Vector3 (0F, 45F, 0F);
		}

		CameraFollow ();
	}


	void CameraFollow()
	{
//		playerViewPos = this.cam.WorldToScreenPoint (GameRuleManager.instance.playerMove.transform.position);
		calibratedPos = GameRuleManager.instance.playerMove.transform.position + distancePos + slidePos;

//		Debug.Log(Vector2.Distance(ScreenRect.center, new Vector2(playerViewPos.x, playerViewPos.y)));

		if(GameRuleManager.instance.playerMove.savedJumpHeight > 0F)
		{
			smoothCorrectionValue = GameRuleManager.instance.playerMove.savedJumpHeight;
		} else {
			smoothCorrectionValue = Mathf.Abs(GameRuleManager.instance.playerMove.fallDelta) * 10F;
		}

		transform.position
			= new Vector3(	calibratedPos.x,
			              Mathf.Lerp(transform.position.y, calibratedPos.y, Time.deltaTime + (YSmoothSpeed * Time.deltaTime * smoothCorrectionValue)),
			              calibratedPos.z);


	}

	public void ForceToCalibration()
	{
		calibratedPos = GameRuleManager.instance.playerMove.transform.position + distancePos + slidePos;
		transform.position = calibratedPos;
	}
}

