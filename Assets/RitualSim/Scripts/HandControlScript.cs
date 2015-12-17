using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class HandControlScript : NetworkBehaviour
{
	public float Smoothing = 5;
	public float InteractionRadius = 2;
	public float InteractionLength = 5;
	public float ThrowForce = 1000;
	public GameObject[] Hands;
	public GameObject[] PhysicalHands;

	private float[] HandTargetAngles = { 0, 0 };
	private Vector3[] HandTargetPositions = { new Vector3( 0, 0, 0 ), new Vector3( 0, 0, 0 ) };
	private Vector3 LastEyeAngles = new Vector3( 0, 0, 0 );

	[SyncVar] GameObject HeldObject_Left = null;
	[SyncVar] GameObject HeldObject_Right = null;

	void Start()
	{
		if ( !isLocalPlayer ) return;

		Hands[0] = Camera.main.transform.GetChild( 0 ).GetChild( 0 ).gameObject;
		Hands[1] = Camera.main.transform.GetChild( 1 ).GetChild( 0 ).gameObject;
	}

	void Update()
	{
		if ( !isLocalPlayer ) return;

		// Update positioning (i.e. move hands from down-side to front-center)
		UpdateHand( 0, KeyCode.Q, new Vector3( 0.15f, 0.22f, 1 ) );
		UpdateHand( 1, KeyCode.E, new Vector3( -0.15f, 0.22f, 1 ) );

		// Update interaction (i.e. pickup objects)
		UpdateInteraction( 0, ref HeldObject_Left );
		UpdateInteraction( 1, ref HeldObject_Right );

		// Update interpolation (i.e. objects snapping to hands)
		UpdateInterpolation( 0, HeldObject_Left );
		UpdateInterpolation( 1, HeldObject_Right );

		LastEyeAngles = transform.eulerAngles;
	}

	void UpdateHand( int hand, KeyCode key, Vector3 position )
	{
		// Input for raising and lowering hands
		if ( Input.GetKey( key ) )
		{
			HandTargetAngles[hand] = 90;
			HandTargetPositions[hand] = position;
		}
		else
		{
			HandTargetAngles[hand] = 0;
			HandTargetPositions[hand] = new Vector3( 0, 0, 0 );
		}

		// Set the sockets new position, for the physical hands to lerp towards
		Hands[hand].transform.localPosition = HandTargetPositions[hand];
		Hands[hand].transform.localEulerAngles = new Vector3( -HandTargetAngles[hand], 0, 0 );

		// Lerp the physical hands to the socket position
		float dif = mod( LastEyeAngles.y - transform.eulerAngles.y + 180, 360 ) - 180;
		PhysicalHands[hand].transform.position = Vector3.Lerp(
			PhysicalHands[hand].transform.position,
			Hands[hand].transform.position + ( transform.right * dif / 50 ),
			Smoothing * Time.deltaTime
		);

		// Lerp the physical hands to the socket rotation
		PhysicalHands[hand].transform.eulerAngles = Hands[hand].transform.eulerAngles;
		//	Vector3.Lerp(
		//	PhysicalHands[hand].transform.eulerAngles,
		//	Hands[hand].transform.eulerAngles,
		//	Smoothing * Time.deltaTime
		//);
	}

	void UpdateInteraction( int hand, ref GameObject heldobject )
	{
		// Only send the command when the state of holding changes
		//if ( Input.GetMouseButtonDown( hand ) || Input.GetMouseButtonUp( hand ) )
		{
			Cmd_UpdateInteraction( hand, Input.GetMouseButton( hand ), heldobject );
			UpdateInteraction_PickupDrop( hand, Input.GetMouseButton( hand ), ref heldobject );
		}
	}

	[Command]
	void Cmd_UpdateInteraction( int hand, bool interact, GameObject heldobject )
	{
		UpdateInteraction_PickupDrop( hand, interact, ref heldobject );
	}

	void UpdateInteraction_PickupDrop( int hand, bool interact, ref GameObject heldobject )
    {
		if ( interact )
		{
			// If something is already held, stop
			if ( heldobject ) return;

			// Trace to forward as sphere of interactivity radius
			Ray interactionray = new Ray();
			{
				interactionray.origin = PhysicalHands[hand].transform.position;
				interactionray.direction = -PhysicalHands[hand].transform.up;
			}
			Debug.DrawLine( interactionray.origin, interactionray.origin + ( interactionray.direction * InteractionLength ), Color.green, 5 );

			// Cast the ray
			//RaycastHit hit;
			RaycastHit[] hits = Physics.SphereCastAll(
				interactionray,
				InteractionRadius,
				//out hitinfo,
				InteractionLength,
				1 << LayerMask.NameToLayer( "Pickup_Fist" )
			);

			// Hit a pickup
			if ( hits.Length != 0 )
			{
				RaycastHit hit = hits[0];

				// Parent and flag as parented
				hit.transform.parent = PhysicalHands[hand].transform;
				heldobject = hit.transform.gameObject;

				// Remove gravity
				hit.transform.GetComponent<Rigidbody>().useGravity = false;
			}
		}
		else if ( heldobject )
		{
			// Find the object
			if ( heldobject )
			{
				// Enable gravity
				heldobject.transform.GetComponent<Rigidbody>().useGravity = true;

				// Unparent
				heldobject.transform.parent = null;

				// Inerit a little of the hand's velocity
				Vector3 offset = PhysicalHands[hand].transform.position - Hands[hand].transform.position;
					offset *= -ThrowForce;
					offset = new Vector3(
						Mathf.Sign( offset.x ) * Mathf.Min( Mathf.Abs( offset.x ), 200 ),
						Mathf.Sign( offset.y ) * Mathf.Min( Mathf.Abs( offset.y ), 200 ),
						Mathf.Sign( offset.z ) * Mathf.Min( Mathf.Abs( offset.z ), 200 )
					);
				heldobject.GetComponent<Rigidbody>().AddForce( offset );

				// Flag as unparented
				heldobject = null;
			}
		}
	}

	private void UpdateInterpolation( int hand, GameObject heldobject )
	{
		// If nothing is held, stop
		if ( !heldobject ) return;

		GameObject pickupobject = heldobject;
		if ( pickupobject )
		{
			pickupobject.transform.localPosition = Vector3.Lerp(
				pickupobject.transform.localPosition,
				new Vector3( 0, -1, 0 ),
				Time.deltaTime
			);
		}
	}

	private GameObject FindByNetworkID( uint id )
	{
		GameObject pickupobject = null;
		{
			GameObject[] objects = GameObject.FindGameObjectsWithTag( "Pickup" );
			foreach ( GameObject gameobject in objects )
			{
				NetworkIdentity identity = gameobject.GetComponent<NetworkIdentity>();
				if ( identity && ( identity.sceneId.Value == id ) )
				{
					pickupobject = gameobject;
				}
			}
		}
		return pickupobject;
	}

	// From http://stackoverflow.com/questions/1878907/the-smallest-difference-between-2-angles
	// Stops weird bugs with C# modulus for correctly finding smallest difference between two angles
	private float mod( float a, int n )
	{
		return a - Mathf.Floor( a / n ) * n;
	}
}
