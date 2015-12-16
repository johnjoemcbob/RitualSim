using System;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityStandardAssets.Vehicles.Ball
{
    public class Ball : NetworkBehaviour
    {
        [SerializeField] private float m_MovePower = 5; // The force added to the ball to move it.
        [SerializeField] private bool m_UseTorque = true; // Whether or not to use torque to move the ball.
        [SerializeField] private float m_MaxAngularVelocity = 25; // The maximum velocity the ball can rotate at.
        [SerializeField] private float m_JumpPower = 2; // The force added to the ball when it jumps.

        [SerializeField] private Camera Camera_Attached; // The attached camera, for reseting

		private const float k_GroundRayLength = 1f; // The length of the ray to check if the ball is grounded.
        private Rigidbody m_Rigidbody;
		private GameObject PlayerContainer;

        private void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            // Set the maximum angular velocity.
            GetComponent<Rigidbody>().maxAngularVelocity = m_MaxAngularVelocity;

			// Setup the networked player
			if ( isLocalPlayer )
			{
				Camera_Attached = Camera.main;
				Camera.main.GetComponent<CameraFollowScript>().target = transform;
			}

			// Reset positioning
			Reset();
        }


        public void Move(Vector3 moveDirection, bool jump)
        {
			if ( !isLocalPlayer ) return;

            // If using torque to rotate the ball...
            if (m_UseTorque)
            {
                // ... add torque around the axis defined by the move direction.
                m_Rigidbody.AddTorque(new Vector3(moveDirection.z, 0, -moveDirection.x)*m_MovePower);
            }
            else
            {
                // Otherwise add force in the move direction.
                m_Rigidbody.AddForce(moveDirection*m_MovePower);
            }

            // If on the ground and jump is pressed...
            if (Physics.Raycast(transform.position, -Vector3.up, k_GroundRayLength) && jump)
            {
                // ... add force in upwards.
                m_Rigidbody.AddForce(Vector3.up*m_JumpPower, ForceMode.Impulse);
            }

			if ( transform.position.y < -1 )
			{
				Reset();
			}
        }

		private void Reset()
		{
			// Back to the center of the world
			transform.position = new Vector3( 0, 1, 0 );

			// Null the velocity
			GetComponent<Rigidbody>().velocity = new Vector3( 0, 0, 0 );
			GetComponent<Rigidbody>().angularVelocity = new Vector3( 0, 0, 0 );

			// Reset the camera
			Camera_Attached.transform.position = transform.position;
		}
    }
}
