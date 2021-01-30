using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Spawning;

public enum ConnectionType
{
    Host,
    Client,
    Server,
}

public class NetworkMode : MonoBehaviour
{
    public Vector3 positionToSpawnAt = new Vector3(0, 1000, 1000);
    public bool MenuOn = true; // by default, menu is on

    public TMPro.TMP_InputField serverIPInput;
    public Quaternion rotationToSpawnWith = Quaternion.Euler(0f,0f,0f);

    public GameObject[] menuObjects;

    protected MLAPI.Transports.UNET.UnetTransport UnetTransport;

    // Start is called before the first frame update
    void Start()
    {
        UnetTransport = GetComponent<MLAPI.Transports.UNET.UnetTransport>();
        CheckToRunServer();
    }

    public void CheckToRunServer()
    {
        //NetworkingManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;

        if (SystemInfo.graphicsDeviceName == null) // if this is headless, then it must be a server :/
        {
            NetworkingManager.Singleton.StartServer();
            Debug.Log("MLAPI started as a Server");
        }
    }

    public void ConnectAs(string connectionType)
    {
        UnetTransport.ConnectAddress = serverIPInput.text;
        NetworkingManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;

        switch(connectionType)
        {
            case "Host":
                NetworkingManager.Singleton.StartHost();
                Debug.Log("MLAPI started as a Host");
                break;
            case "Client":
                NetworkingManager.Singleton.StartClient();
                Debug.Log("MLAPI started as a Client");
                break;
            case "Server":
                NetworkingManager.Singleton.StartServer();
                Debug.Log("MLAPI started as a Server");
                break;
        }

        if (NetworkingManager.Singleton.IsHost || NetworkingManager.Singleton.IsClient || NetworkingManager.Singleton.IsServer)
        {
            DisableMenuObjects();
        } else
        {
            Debug.Log("Did not connect!");
        }
    }

    private void DisableMenuObjects() {
        foreach (GameObject menuObject in menuObjects) {
            menuObject.SetActive(false);
        }
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientId, MLAPI.NetworkingManager.ConnectionApprovedDelegate callback)
    {
        //Your logic here
        //bool approve = true;
        //bool createPlayerObject = true;

        // The prefab hash. Use null to use the default player prefab
        // If using this hash, replace "MyPrefabHashGenerator" with the name of a prefab added to the NetworkedPrefabs field of your NetworkingManager object in the scene
        ulong? prefabHash = SpawnManager.GetPrefabHashFromGenerator("Player");

        //If approve is true, the connection gets added. If it's false. The client gets disconnected
        //callback(createPlayerObject, prefabHash, approve, positionToSpawnAt, rotationToSpawnWith);
    }
}
