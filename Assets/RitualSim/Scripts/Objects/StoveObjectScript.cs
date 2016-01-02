using UnityEngine;
using System.Collections;

public class StoveObjectScript : MonoBehaviour
{
	void OnTriggerEnter( Collider other )
	{
		ToggleCooking( other, true );
	}

	void OnTriggerExit( Collider other )
	{
		ToggleCooking( other, false );
	}

	void ToggleCooking( Collider other, bool cook )
	{
		CookableObjectScript cookable = other.gameObject.GetComponent<CookableObjectScript>();
		if ( cookable )
		{
			cookable.SetCooking( cook );
		}
	}
}
