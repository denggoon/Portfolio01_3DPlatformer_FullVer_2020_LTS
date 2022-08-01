using UnityEngine;
using System.Collections;

public class ShaderAssigner : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		Renderer[] renderers = this.transform.GetComponentsInChildren<Renderer> ();
		
		ShaderManager.instance.ReassignShadersInRenderers (renderers);
		ShaderManager.instance.ReassignShadersInProjector (GetComponentInChildren<Projector> ());
	}
}
