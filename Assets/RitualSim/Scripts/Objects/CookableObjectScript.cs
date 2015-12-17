using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CookableObjectScript : NetworkBehaviour
{
	public float CookSpeed = 10;

	[SyncVar] float CookProgress = 0; // 0 - uncooked, 1 - cooked, 2 - burnt

	void Update()
	{
		CookProgress += Time.deltaTime * CookSpeed;
		if ( CookProgress > 2 )
		{
			GetComponent<Renderer>().material.color = Color.black;
		}
		else if ( CookProgress > 1 )
		{
			GetComponent<Renderer>().material.color = Color.grey;
		}
	}
}
