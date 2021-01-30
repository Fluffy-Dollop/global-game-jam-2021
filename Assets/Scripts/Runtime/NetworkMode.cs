using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Spawning;

public class NetworkMode : MonoBehaviour
{
    public bool isHost = false;
    public Vector3 positionToSpawnAt = new Vector3(0, 1000, 1000);
    public Quaternion rotationToSpawnWith = Quaternion.Euler(0f,0f,0f);

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    public void Setup()
    {
        //NetworkingManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;

        if (SystemInfo.graphicsDeviceName == null) // if this is headless, then it must be a server :/
        {
            NetworkingManager.Singleton.StartServer();
            Debug.Log("MLAPI started as a Server");
        }
        else if (isHost == true)
        {
            NetworkingManager.Singleton.StartHost(positionToSpawnAt);
            Debug.Log("MLAPI started as a Host");
        }
        else
        {
            NetworkingManager.Singleton.StartClient();
            Debug.Log("MLAPI started as a Client");
        }
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientId, MLAPI.NetworkingManager.ConnectionApprovedDelegate callback)
    {
        //Your logic here
        bool approve = true;
        bool createPlayerObject = true;

        // The prefab hash. Use null to use the default player prefab
        // If using this hash, replace "MyPrefabHashGenerator" with the name of a prefab added to the NetworkedPrefabs field of your NetworkingManager object in the scene
        ulong? prefabHash = SpawnManager.GetPrefabHashFromGenerator("Player");

        //If approve is true, the connection gets added. If it's false. The client gets disconnected
        //callback(createPlayerObject, prefabHash, approve, positionToSpawnAt, rotationToSpawnWith);
    }
}
