using UnityEngine;
using System.Collections;

namespace UnityStandardAssets.Vehicles.Ball
{
	public class CameraFollowScript : MonoBehaviour
	{
		public Transform target;            // The position that that camera will be following.
		public float smoothing = 5f;        // The speed with which the camera will be following.
		public float CameraSensitivity = 5f; // The speed with which the camera will rotate
		public float Distance = 5;

		Vector3 offset;                     // The initial offset from the target.
		private Vector3 Rotation = new Vector3( 0, 0, 0 );

		void Start()
		{
			if ( target )
			{
				// Calculate the initial offset.
				offset = transform.position - target.position;
			}

			//Distance = Vector3.Distance( transform.position, target.position );

			// Capture the cursor
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}

		void FixedUpdate()
		{
			// Input for returning cursor control
			if ( Input.GetKeyDown( KeyCode.Escape ) )
			{
				Cursor.visible = !Cursor.visible;
				if ( Cursor.visible )
				{
					Cursor.lockState = CursorLockMode.None;
				}
				else
				{
					Cursor.lockState = CursorLockMode.Locked;
				}
			}

			if ( !target ) return;

			// Input for rotation
			Rotation = new Vector3( 0, 0, 0 );
			if ( !Cursor.visible )
			{
				if ( Mathf.Abs( Input.GetAxis( "Mouse Y" ) ) > 0.1f )
				{
					Rotation.x += -Input.GetAxis( "Mouse Y" ) * CameraSensitivity;
						Rotation.x = Mathf.Clamp( Rotation.x, -180, 180 );
				}
				if ( Mathf.Abs( Input.GetAxis( "Mouse X" ) ) > 0.1f )
				{
					Rotation.y += Input.GetAxis( "Mouse X" ) * CameraSensitivity;
				}
			}

			// Rotation around the center
			transform.RotateAround( target.position, transform.right, Rotation.x * smoothing * Time.deltaTime );
			transform.RotateAround( target.position, transform.up, Rotation.y * smoothing * Time.deltaTime );

			// Now project backwards to the distance specified and place the camera there
			Vector3 targetCamPos = target.position - ( transform.forward * Distance );
			//transform.position = position_camerabuff;

			// Create a postion the camera is aiming for based on the offset from the target.
			//Vector3 targetCamPos = target.position + offset;

			// Smoothly interpolate between the camera's current position and it's target position.
			transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);
			//transform.position = target.position;

			// Look at the center
			transform.LookAt( target.position );
		}
	}
}
