using UnityEngine;
using System.Collections;

public class Tiling : MonoBehaviour 
{

	//attach this script to all child that has renderer
	//after your Data set than remove this script from all object

	public int LightMapIndexNumber;
	public Vector4 LightMapOffset;


	void OnDrawGizmos()
	{
		LightMapIndexNumber = gameObject.GetComponent<Renderer> ().lightmapIndex;
		LightMapOffset = gameObject.GetComponent<Renderer> ().lightmapScaleOffset;
	}
}
