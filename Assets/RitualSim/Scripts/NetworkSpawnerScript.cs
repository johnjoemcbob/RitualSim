using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetworkSpawnerScript : NetworkBehaviour
{
	public GameObject Prefab_BeefMeat;

	private bool HasServerSpawned = false;

	public void FixedUpdate()
	{
		if ( HasServerSpawned || ( !NetworkServer.active ) ) return;

        SpawnObjects();

		HasServerSpawned = true;
	}

	private void SpawnObjects()
	{
		//GameObject beefmeat = (GameObject) Instantiate( Prefab_BeefMeat, transform.position + new Vector3( 0, 5, 0 ), transform.rotation );
        //NetworkServer.Spawn( beefmeat );
	}
}
