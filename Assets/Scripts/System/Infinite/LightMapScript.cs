using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightMapScript : MonoBehaviour
{
//	public bool FillData;
//	void OnDrawGizmos()
//	{
//		if (FillData)
//		{
//			Tiling[] Script=GameObject.FindObjectsOfType<Tiling>();
//
//			List<RendererInfo> rendererInfos = new List<RendererInfo>();
//
//
//			for(int i=0;i<Script.Length;i++)
//			{
//				RendererInfo info = new RendererInfo();
//				info.renderer = Script[i].gameObject.GetComponent<Renderer>();
//				info.lightmapOffsetScale = Script[i].LightMapOffset;
//				info.lightmapIndex =Script[i].LightMapIndexNumber;
//				rendererInfos.Add(info);
//			}
//
//			J_renderer = rendererInfos.ToArray();
//
//			FillData=false;
//		}
//	}

	public void FillLightMapData()
	{
		Tiling[] Script=GameObject.FindObjectsOfType<Tiling>();
		
		List<RendererInfo> rendererInfos = new List<RendererInfo>();
		
		for(int i=0;i<Script.Length;i++)
		{
			RendererInfo info = new RendererInfo();
			info.renderer = Script[i].gameObject.GetComponent<Renderer>();
			info.lightmapOffsetScale = Script[i].LightMapOffset;
			info.lightmapIndex =Script[i].LightMapIndexNumber;
			rendererInfos.Add(info);
		}
		
		J_renderer = rendererInfos.ToArray();
	}

	public List<Tiling> tilingList = new List<Tiling>();
	public void AttachTilingScriptToAllRenderers()
	{
		Renderer[] renderers = GetComponentsInChildren<Renderer> ();

		for (int i=0; i<renderers.Length; i++) 
		{
			if(renderers[i] is MeshRenderer || renderers[i] is SkinnedMeshRenderer)
				tilingList.Add(renderers[i].gameObject.AddComponent<Tiling>());
		}
	}

	public void RemoveTilingScriptToAllRenderers()
	{
		for (int i=0; i<tilingList.Count; i++) 
		{
			DestroyImmediate(tilingList[i]);
		}

		tilingList.Clear ();
	}

	[System.Serializable]
	struct RendererInfo
	{
		public Renderer 	renderer;
		public int 			lightmapIndex;
		public Vector4 		lightmapOffsetScale;
	}

	[SerializeField]
	RendererInfo[]	J_renderer;
	[SerializeField]
	Texture2D[] 	J_Lightmap;

	void Awake ()
	{
		if (J_renderer == null || J_renderer.Length == 0)
			return;

		var lightmaps = LightmapSettings.lightmaps;
		var combinedLM = new LightmapData[lightmaps.Length + J_Lightmap.Length];

		lightmaps.CopyTo(combinedLM, 0);
		for (int i = 0; i < J_Lightmap.Length;i++)
		{
			combinedLM[i+lightmaps.Length] = new LightmapData();
			combinedLM[i+lightmaps.Length].lightmapColor = J_Lightmap[i];
		}

		ApplyLights(J_renderer, lightmaps.Length);
		LightmapSettings.lightmaps = combinedLM;
	}

	
	static void ApplyLights (RendererInfo[] infos, int lightmapOffsetIndex)
	{
		for (int i=0;i<infos.Length;i++)
		{
			var info = infos[i];
			info.renderer.lightmapIndex = info.lightmapIndex + lightmapOffsetIndex;
			info.renderer.lightmapScaleOffset = info.lightmapOffsetScale;
		}
	}


}