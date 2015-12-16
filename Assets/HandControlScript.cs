using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class HandControlScript : NetworkBehaviour
{
	public float Smoothing = 5;
	public GameObject[] Hands;
	public GameObject[] PhysicalHands;

	private float[] HandTargetAngles = { 0, 0 };
	private Vector3[] HandTargetPositions = { new Vector3( 0, 0, 0 ), new Vector3( 0, 0, 0 ) };
	private Vector3 LastEyeAngles = new Vector3( 0, 0, 0 );

	void Start()
	{
		if ( !isLocalPlayer ) return;

		Hands[0] = Camera.main.transform.GetChild( 0 ).GetChild( 0 ).gameObject;
		Hands[1] = Camera.main.transform.GetChild( 1 ).GetChild( 0 ).gameObject;
	}

	void Update()
	{
		if ( !isLocalPlayer ) return;

		UpdateHand( 0, KeyCode.Q, new Vector3( 0.15f, 0.22f, 1 ) );
		UpdateHand( 1, KeyCode.E, new Vector3( -0.15f, 0.22f, 1 ) );

		LastEyeAngles = transform.eulerAngles;
	}

	void UpdateHand( int hand, KeyCode key, Vector3 position )
	{
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

	// From http://stackoverflow.com/questions/1878907/the-smallest-difference-between-2-angles
	// Stops weird bugs with C# modulus for correctly finding smallest difference between two angles
	private float mod( float a, int n )
	{
		return a - Mathf.Floor( a / n ) * n;
	}
}
