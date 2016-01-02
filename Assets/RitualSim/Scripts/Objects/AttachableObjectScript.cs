using UnityEngine;
using System.Collections;

public class AttachableObjectScript : MonoBehaviour
{
	public Transform AttachTop;
	public Transform AttachBottom;

	void OnCollisionEnter( Collision collision )
	{
		AttachableObjectScript attachable = collision.gameObject.GetComponent<AttachableObjectScript>();
		if ( attachable )
		{
			// Always prioritise the gameobject which is below to do the parenting
			if ( collision.contacts[0].point.y > transform.position.y )
			{
				collision.transform.SetParent( transform );
				collision.transform.localEulerAngles = Vector3.zero;
				collision.transform.localPosition = AttachTop.localPosition - attachable.AttachBottom.localPosition;
			}
		}
	}
}
