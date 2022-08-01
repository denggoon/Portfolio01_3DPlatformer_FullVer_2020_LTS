using UnityEngine;
using System.Collections;

public class AlphaBeamCam : MonoBehaviour {

	public GameObject alphaObj;
	public Color objColor;

	public PlayerMoveCC player;

	void Update()
	{
		if(player == null)
		{
			player = FindObjectOfType(typeof(PlayerMoveCC)) as PlayerMoveCC;
		}

		if (player == null)
						return;

		Vector3 direction = player.transform.position - this.transform.position;

		Ray alphaRay = new Ray (this.transform.position, direction);
		RaycastHit hit;

		Debug.DrawRay (this.transform.position, alphaRay.direction, Color.cyan);


		if (Physics.Raycast(alphaRay, out hit) )
		{
			if(hit.collider.CompareTag("Player"))
			{
				if(alphaObj != null)
					alphaObj.GetComponent<Renderer>().material.color = objColor;

				alphaObj = null;

			} else {

				if(alphaObj == null)
				{
					if(hit.collider.gameObject.GetComponent<Renderer>() == null) return;

					alphaObj = hit.collider.gameObject;

					objColor = alphaObj.GetComponent<Renderer>().material.color;
					alphaObj.GetComponent<Renderer>().material.color = new Color(objColor.r, objColor.g, objColor.b, 0.5F);
				}
			}
		}

	}
}
