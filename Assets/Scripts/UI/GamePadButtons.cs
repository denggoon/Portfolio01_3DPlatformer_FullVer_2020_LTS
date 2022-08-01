using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class GamePadButtons : MonoBehaviour{

//	private GamePad gamePadSystem;
	private SpriteRenderer btnImage;
	public Camera uiCamera;

	void Start()
	{
		btnImage = GetComponent<SpriteRenderer> ();
	}

//	public void SetGamePadSystem(GamePad gamePad)
//	{
//		gamePadSystem = gamePad;
//	}

	public void OnMouseEnter()
	{
		btnImage.color = Color.red;
	}

//	public void OnMouseDown()
//	{
//	}

	public void OnMouseExit()
	{
		btnImage.color = Color.white;
	}

	public void OnMouseUp()
	{
		btnImage.color = Color.white;
	}
}
