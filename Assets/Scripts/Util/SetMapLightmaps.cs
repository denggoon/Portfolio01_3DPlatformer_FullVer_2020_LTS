using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SetMapLightmaps : MonoBehaviour {

	public List<Texture2D> lightmapFar;
	public List<Texture2D> lightmapNear;

	public bool selfStart = false;

	void Start()
	{
		if(selfStart)
		{
			ApplyLightMap();
		}
	}

	public void ApplyLightMap()
	{
		Debug.Log ("####SetMapLightmaps - ApplyLightmap!");
		int maxCount = lightmapFar.Count;
		if(lightmapNear.Count > maxCount) maxCount = lightmapNear.Count;

		LightmapData[] customLightmaps = LightmapSettings.lightmaps;

		System.Array.Resize(ref customLightmaps, maxCount);

		for(int i=0; i<customLightmaps.Length; i++)
		{
			if(lightmapFar.Count > i)
				customLightmaps[i].lightmapColor = lightmapFar[i];

			if(lightmapNear.Count > i)
				customLightmaps[i].lightmapDir = lightmapNear[i];
		}

		LightmapSettings.lightmaps = customLightmaps;
	}
}
