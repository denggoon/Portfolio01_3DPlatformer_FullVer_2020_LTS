using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//스크린을 반으로 나눠 좌측 스크린은 조이스틱, 우측 스크린은 버튼 액션을 하는 드래그 인풋 방식입니다. 
public class DragInput : MonoBehaviour 
{
	/*
	private static DragInput instance_;
	
	public static DragInput instance
	{
		get
		{
			return instance_;
		}
	}

	public bool isCenterFixed = true;

	private Vector2 standardSize; //UI의 기준이 되는 해상도 입니다. 이보다 커지면 Absolute로, 작으면 Relative 방식을 취합니다. 
	private float standardDpi;
	private Vector2 defaultCenterRatio;  //기준 중심을 맞추기 위한 비율입니다. (화면을 반으로 갈라 왼쪽 화면 기준)  

	public Vector2 centerPosInchLeftBottom; //좌측 하단 기준 몇 인치 떨어진 곳이 조이스틱 중심점인가? 
	public Vector2 centerPos; //Rect 기준 

//	private Rect LScreenRect; //좌측 화면 분할 
//	private Rect RScreenRect; //우측 화면 분할 

//	private float buttonWSize = Screen.width * .1F; //ONGUI를 통해 표시되는 버튼의 폭과 높이는 화면의 25%, 20%이다. 
//	private float buttonHSize = Screen.height * .05F;

	private bool showDebugUI = true;

	public float posX; //X위치 
	public float posY; //Y위치 

	public bool buttonPressed; //버튼이 눌렸는가? 
	public bool specButtonPressed;

	private float specBtnWSize = Screen.width * .2F; //ONGUI를 통해 표시되는 버튼의 폭과 높이는 화면의 25%, 20%이다. 
	private float specBtnHSize = Screen.height * .16F;

	private Rect specBtnRect;
	//여기까지 외부에서 DragInput.을 통해 접근할수 있는 변수 

	private float joyStickW;
	private float joyStickH;

	private Vector2 beginTouchPos; //터치 시작 위치 
	private Vector2 moveTouchPos; //이동시 터치 위치 
	
	private Vector2 directionVector; //방향 벡터 
	private float dragMagnitude; //드래그되는 거리 
	
	private Vector2 deltaTouchVector; //터치의 운동량 
	
	public Texture2D joystickTex; //표시되는 조이스틱의 텍스쳐 
	
	private List<int> beginFingerIDs = new List<int>(); //좌측 화면에서 터치가 시작되는 터치들의 fingerID를 여기서 보관 

	void Awake()
	{
		instance_ = this;
	}

	void Start()
	{
		//화면을 둘로 나눈다. 
//		LScreenRect = new Rect (0, 0, Screen.width * .5F, Screen.height);
//		RScreenRect = new Rect (Screen.width * .5F, 0, Screen.width * .5F, Screen.height);

//		standardSize = new Vector2(2560.0F, 1440.0F); //갤럭시 S5 
//		standardDpi = 577.0F;
//		centerPosInchLeftBottom = new Vector3(.8F, .7F); //좌측 하단 기준 .8인치, .7인치 
//
//		float InchWide = Screen.width / Screen.dpi; //가로로 몇인치 
//		float InchHigh = Screen.height / Screen.dpi; //세로로 몇인치  
//
//		float inchWideInPixel = Screen.width / InchWide; //1인치 길이가 몇 픽셀? 
//		float inchHighInPixel = Screen.height / InchHigh; //1인치 높이가 몇 픽셀? 
//
//		centerPos = new Vector2(centerPosInchLeftBottom.x * inchWideInPixel, Screen.height - (centerPosInchLeftBottom.y * inchHighInPixel));
//
//		isCenterFixed = true;
//
//		joyStickW = (Screen.width / 480.0F) * joystickTex.width; //표시되는 조이스틱의 크기는 480*320 기준으로 100이다. 
//		joyStickH = (Screen.height / 320.0F) * joystickTex.height;
//
//		specBtnRect = new Rect(Screen.width - specBtnWSize * 1.1F, Screen.height - specBtnHSize * 1.1F, specBtnWSize, specBtnHSize);
	}

//	void OnGUI()
//	{
//		if (moveTouchPos != Vector2.zero)  //움직임이 있을경우에만 조이스틱을 표시한다. 
//		{
//			GUI.DrawTexture (new Rect (beginTouchPos.x - joyStickW *.2F * .5F, beginTouchPos.y - joyStickH *.2F * .5F, 
//			                           joyStickW * .2F, joyStickH *.2F), 
//			                 joystickTex);
//
//			GUI.DrawTexture (new Rect (moveTouchPos.x - joyStickW * .5F, moveTouchPos.y - joyStickH * .5F, 
//			 	                      	joyStickW, joyStickH),
//			                			joystickTex);
//		}
//	}

	// Update is called once per frame
	void Update() 
	{
		if(Input.GetKeyDown(KeyCode.BackQuote))
		{
			showDebugUI = !showDebugUI;
		}

//		TouchBeginToDragInput (); //터치 아날로그 스틱 방식을 사용한다. 

		if (Input.GetKeyDown (KeyCode.Escape)) //안드로이드 에서의 백버튼을 씬선택메뉴로. 
		{
			Application.LoadLevel("MainScene");
		}
	}

//	void TouchBeginToDragInput() //최초 터치한 지점을 기준으로 드래그해서 움직이는 방식 
//	{
//		buttonPressed = false; //기본적으로 우측화면 터치를 통한 버튼입력은 매프레임 false이다. 
//		specButtonPressed = false; //스폐셜 버튼도 마찬가지다. 
//
//		foreach (Touch touch in Input.touches) //화면에 일어난 모든 터치를 전제로 한다. 
//		{
//			float yInverse = Screen.height - touch.position.y; //터치 입력의 좌표는 Rect방식과 달라 Y값이 반대이므로 
//
//			Vector2 touchPosition = touch.position; //이런식으로 Y값을 재배치한다. 
//
//			touchPosition = new Vector2(touch.position.x, yInverse);		
//
//			if(touch.phase.Equals(TouchPhase.Began)) //만약 본 터치가 막 터치를 시작한 터치인 경우 
//			{
////				if (LScreenRect.Contains (touchPosition)) //좌측 화면에 있는경우 
////				{
////					beginTouchPos = touchPosition; //터치 시작 위치 등록 
////
////					if(isCenterFixed)
////					{
////						moveTouchPos = touchPosition;
////					}
////					beginFingerIDs.Add(touch.fingerId); //본 터치의 핑거 아이디를 등록 
////
////				} else 
//				if (RScreenRect.Contains(touchPosition)) //아닌경우 우측 화면으로 인식, 
//				{
////					if(specBtnRect.Contains(touchPosition)) //우측화면중에서도 스폐설 버튼이 위치할곳을 터치하였는가? 
////					{
////						specButtonPressed = true; //그렇다면 스폐셜 버튼이 눌리었다. 
////					} else {
//						buttonPressed = true; //아니라면 일반 버튼이 눌리었다. 
////					}
//				}
//			}
//
////			if(touch.phase.Equals(TouchPhase.Moved)) //터치가 이동중인경우 
////			{
////				if (LScreenRect.Contains (touchPosition)) //좌측 화면인경우 
////				{
////					moveTouchPos = touchPosition; //이동 위치 매프레임 별로 등록 
////				} else { //아닌경우 
////
////					if(beginFingerIDs.Contains(touch.fingerId)) //핑거 아이디가 등록된경우 좌측 에서 시작해 우측 화면에서 끝났다고 볼수 있다. 
////					{
////						moveTouchPos = Vector2.zero; //좌측 화면을 벗어난경우에는 움직임 취소 
////					} else {
//////						buttonPressed = true; //우측 화면에서 터치가 이동하는것은 버튼 프레스로 봐야할지 슬라이드로 봐야할지 아직 고민중 
////					}
////				}
////			}
////
////			if(touch.phase.Equals(TouchPhase.Ended)) //터치가 끝났으면 
////			{
////				if(beginFingerIDs.Contains(touch.fingerId)) //등록된 핑거 아이디 리스트에 잇는지 확인 
////				{
////					beginFingerIDs.Remove(touch.fingerId); //만약 있다면 좌측 화면에서 시작한 터치이고, 제거할 필요가 있다. 
////				}
////
////				if (LScreenRect.Contains (touchPosition)) //끝나는 시점에서 좌측 화면에서 끝났다면 움직임을 초기화 한다. 
////				{
////					beginTouchPos = Vector2.zero;
////					moveTouchPos = Vector2.zero;
////				}
////
////			}
//		}
//
////		if(isCenterFixed)
////		{
////			beginTouchPos = centerPos;
////		}
////
////		if (moveTouchPos != Vector2.zero)  //움직임이 있었던 경우 
////		{
////			moveTouchPos = new Vector2(
////				Mathf.Clamp(moveTouchPos.x, beginTouchPos.x - (Screen.width * .1F), beginTouchPos.x + (Screen.width * .1F)),
////				Mathf.Clamp(moveTouchPos.y, beginTouchPos.y - (Screen.height * .2F), beginTouchPos.y + (Screen.height * .2F)));
////
////			//터치 이동 좌표는 터치 시작점에서 아무리 멀어져도 X츅운 화면 폭의 10% 이하로, Y축은 화면 높이의 20%로 제한한다. 
////
////			directionVector = beginTouchPos - moveTouchPos;
////			//이 값을 통해 방향 벡터를 구한다. 
////		} else {
////			directionVector = Vector2.zero; //이동이 없으면 당연히 방향벡터도 없다. 
////		}
////
////		posX = -directionVector.x / (Screen.width * .1F);
////		posY = directionVector.y / (Screen.height * .2F);
////
////		posX = Mathf.Clamp (posX, -1F, 1F);
////		posY = Mathf.Clamp (posY, -1F, 1F);
//
//		//posX와 posY는 Input.GetAxis를 대체하기 위한 값으로서 무조건 -1에서 1사이의 값으로 제한되며, X값은 화면 폭의 10%를 100%로 봤을때의 값을, Y값은 화면 높이의 20%를 100%로 봤을때의 값을 제한한 것이다. 
//	}

//	public Rect GetRRect()
//	{
//		return this.RScreenRect;
//	}
*/
}

