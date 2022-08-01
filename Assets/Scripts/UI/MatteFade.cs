using UnityEngine;
using System.Collections;

public class MatteFade : UIMonoBehaviour {

    private CanvasGroup canvasGroup;

    public void FadeIn(float time)
    {
//		Debug.Log("FadeIn");
        gameObject.SetActive(true);

		StopCoroutine("FadeMatteOut");
        StopCoroutine("FadeMatteIn");
        StartCoroutine("FadeMatteIn", time);
    }

    public void FadeOut(float time)
    {
//		Debug.Log("FadeIn");
//        gameObject.SetActive(true);

		StopCoroutine("FadeMatteOut");
		StopCoroutine("FadeMatteIn");
        StartCoroutine("FadeMatteOut", time);
    }

    IEnumerator FadeMatteIn(float time)
    {
        canvasGroup = GetComponent<CanvasGroup>();

        float i = 0F;

		float deltaTime = 0.01F;

		canvasGroup.alpha = 1F;

        do {
//			Debug.Log (i);
			canvasGroup.alpha = Mathf.Lerp (1, 0, i);
			i += deltaTime / time;

			yield return new WaitForSeconds (deltaTime);
		} while (i < 1F);

		canvasGroup.alpha = 0;

		CallBackCompletion ();
    }

    IEnumerator FadeMatteOut(float time)
    {
        canvasGroup = GetComponent<CanvasGroup>();

        float i = 0;
		float deltaTime = 0.01F;
		
		canvasGroup.alpha = 0F;

        do {
			canvasGroup.alpha = Mathf.Lerp (0, 1, i);
			i += deltaTime / time;

			yield return new WaitForSeconds (deltaTime);
		} while (i < 1F);

		canvasGroup.alpha = 1;

		CallBackCompletion ();
    }
}
