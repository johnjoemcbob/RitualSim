using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [Serializable]
    public class MouseLook
    {
        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        public bool clampVerticalRotation = true;
        public float MinimumX = -90F;
		public float MaximumX = 90F;
		public bool UseHorizontal = false;
		public bool ClampHorizontalRotation = false;
		public float MinimumY = -45F;
		public float MaximumY = 45F;
        public bool smooth;
        public float smoothTime = 5f;


		public Quaternion m_CharacterTargetRot;
        public Quaternion m_CameraTargetRot;


        public void Init(Transform character, Transform camera)
        {
			if ( character )
            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = camera.localRotation;
        }


        public void LookRotation(Transform character, Transform camera)
        {
            float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
			float xRot = CrossPlatformInputManager.GetAxis( "Mouse Y" ) * YSensitivity;
			float yRotCam = 0;
			if ( UseHorizontal )
			{
				yRotCam = yRot;
			}

			m_CharacterTargetRot.Set( m_CharacterTargetRot.y, 0f, 0f, m_CharacterTargetRot.w );
			m_CharacterTargetRot *= Quaternion.Euler( yRot, 0f, 0f );
			m_CameraTargetRot *= Quaternion.Euler( -xRot, 0f, 0f );

            if(clampVerticalRotation)
				m_CameraTargetRot = ClampRotationAroundXAxis( m_CameraTargetRot, MinimumX, MaximumX );

			if ( ClampHorizontalRotation )
			{
				m_CharacterTargetRot = ClampRotationAroundXAxis( m_CharacterTargetRot, MinimumY, MaximumY );
			}
			m_CharacterTargetRot.Set( 0f, m_CharacterTargetRot.x, 0f, m_CharacterTargetRot.w );

            if(smooth)
            {
				if ( character )
					character.localRotation = Quaternion.Slerp (character.localRotation, m_CharacterTargetRot,
						smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp (camera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
            }
            else
            {
				if ( character )
					character.localRotation = m_CharacterTargetRot;
                camera.localRotation = m_CameraTargetRot;
            }
        }


		Quaternion ClampRotationAroundXAxis( Quaternion q, float min, float max )
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);

			angleX = Mathf.Clamp( angleX, min, max );

            q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }
    }
}
