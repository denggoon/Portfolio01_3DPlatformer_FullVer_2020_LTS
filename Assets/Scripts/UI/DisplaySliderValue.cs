using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisplaySliderValue : MonoBehaviour {

	public Text uiTxtSliderValue;
	public Slider sliderRef;
	public bool divByMaxValue = true;
	// Use this for initialization
	void Start () {
		if(uiTxtSliderValue == null)
			uiTxtSliderValue = GetComponent<Text>();
	}
	
	public void SetSliderValueToText(float sliderVal)
	{
		string strVal = "";
		if(divByMaxValue)
		{
			if(sliderRef != null)
			{
				Debug.Log(sliderVal);
				Debug.Log(sliderRef.maxValue);
				strVal = (sliderVal / sliderRef.maxValue).ToString();
			}

		} else {
			strVal = sliderVal.ToString();
		}

		uiTxtSliderValue.text = strVal;
	}
}
