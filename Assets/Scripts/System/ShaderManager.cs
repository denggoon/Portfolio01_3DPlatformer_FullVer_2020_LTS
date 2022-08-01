using UnityEngine;

public class ShaderManager : MonoBehaviour {

	private static ShaderManager instance_;
	
	public static ShaderManager instance
	{
		get
		{
			return instance_;
		}
	}

	void OnDestroy()
	{
		instance_ = null;
	}

	public Shader[] shaderList;

	void Awake()
	{
		instance_ = this;
	}

	void Start()
	{
//		ReassignShadersInScene ();
	}

	public void ReassignShadersInScene()
	{
		ReassignShadersInRenderers(FindObjectsOfType<Renderer>());
	}

	public void ReassignShadersInRenderers(Renderer[] rendererList)
	{
		if (rendererList == null)
			return;
		if (rendererList.Length == 0)
			return;

//		Debug.Log ("Shader Assigning: " + Time.realtimeSinceStartup);

		for (int i=-0; i<rendererList.Length; i++) 
		{
			for(int j=0; j<shaderList.Length; j++)
			{
//				Debug.Log(rendererList[i].sharedMaterial.shader.name + " / " +  shaderList[j].name);
				if(rendererList[i].sharedMaterial.shader.name == shaderList[j].name)
				{
					rendererList[i].sharedMaterial.shader = Shader.Find(rendererList[i].sharedMaterial.shader.name);
				}
			}
		}

		rendererList = null;
//		Debug.Log ("Shader Assigning Done: " + Time.realtimeSinceStartup);
	}

	public void ReassignShadersInProjector(Projector proj)
	{
		if (proj == null)
			return;

		Debug.Log ("Shader Assigning: " + Time.realtimeSinceStartup);
		
		for(int j=0; j<shaderList.Length; j++)
		{
//			Debug.Log(proj.material.shader.name + " / " +  shaderList[j].name);
			if(proj.material.shader.name == shaderList[j].name)
			{
				proj.material.shader = Shader.Find(proj.material.shader.name);
			}
		}

		Debug.Log ("Shader Assigning Done: " + Time.realtimeSinceStartup);
	}
}
