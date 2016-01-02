using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TransformPropagationScript : NetworkBehaviour
{
	[SyncVar]
	// Vector3s contain either 1 or 0 bools for each axis, defining which axis should be sent
	Vector3 PositionPropAxes;
	Vector3 RotationPropAxes;
	Vector3 ScalePropAxes;

	// Send information as an array of floats, alongside an int describing what has been sent (bitwise)
}
