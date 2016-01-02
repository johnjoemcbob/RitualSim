using UnityEngine;
using System.Collections;

public class TreeObjectScript : MonoBehaviour
{
    void OnCollisionStay( Collision collision )
	{
		Rigidbody body = collision.transform.GetComponent<Rigidbody>();
		if ( body && collision.transform.GetComponent<CanTreeObjectScript>() && ( !body.isKinematic ) )
		{
			body.isKinematic = true;
		}
	}

	void OnCollisionExit( Collision collision )
	{
		Rigidbody body = collision.transform.GetComponent<Rigidbody>();
		if ( body && collision.transform.GetComponent<CanTreeObjectScript>() )
		{
			body.isKinematic = false;
		}
	}
}
