using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MLAPI;
using MLAPI.NetworkedVar;
using UnityEngine.InputSystem;
using MLAPI.Messaging;

public enum GameState
{
    None,
    GameLobby,
    GameCountdown,
    GamePlay,
    GameWinner,
}

public class GameManager : NetworkedBehaviour
{
    public GameObject[] itemPrefabs;

    [SyncedVar]
    public GameState gameState = GameState.None;

    /// <summary>
    /// Coundown stuff
    /// </summary>
    [SyncedVar]
    public float countdownValue;
    [SerializeField]
    float countdownStart = 3f;

    [SyncedVar]
    public string winner = "";

    [SerializeField] int SpawnNumberCrowns = 1;
    [SerializeField] int SpawnNumberNormal = 2;
    [SerializeField] int SpawnNumberUtility = 2; // also once in the utility spot
    [SerializeField] int SpawnNumberRare = 1;


    List<ItemBehavior> spawnedItems = new List<ItemBehavior>();

    void Awake()
    {
        countdownValue = countdownStart;
    }

    private void Start()
    {
        SceneManager.LoadScene("Scenes/Level1", LoadSceneMode.Additive);
    }

    public GameObject FindClosestItem(Vector3 position, float range, GameObject exclude)
    {
        GameObject closest = null;

        Collider[] colliders;
        // Presuming the object you are testing also has a collider 0 otherwise
        if ((colliders = Physics.OverlapSphere(position, range)).Length > 1)
        {
            foreach (var collider in colliders)
            {
                var go = collider.gameObject;
                // ignore certain cases
                if (!go) { continue; }
                //Debug.Log("found object " + go.name);
                if (go.tag != "item") { continue; } // for now ignore non-items!
                if (go == exclude) { continue; }
                // if we made it this far, pick the closest
                if (!closest || (go.transform.position - position).sqrMagnitude < (closest.transform.position - position).sqrMagnitude)
                {
                    closest = go;
                }
            }
        }

        Debug.Log("FindClosestItem returning " + (closest ? closest.name : "<null>"));
        return closest;
    }

    // Update is called once per frame
    void Update()
    {
        RunGameState();
    }


    public void NextGameState(FPC player)
    {
        InvokeClientRpcOnEveryone(SendMessage, player.playerName.Value + " is manually editing state " + Time.deltaTime);
        switch (gameState)
        {
            case (GameState.None):
                SetGameState(GameState.GameLobby);
                break;
            case (GameState.GameLobby):
                SetGameState(GameState.GameCountdown);
                break;
            case (GameState.GameCountdown):
                SetGameState(GameState.GamePlay);
                break;
            case (GameState.GamePlay):
                SetGameState(GameState.GameWinner);
                break;
            case (GameState.GameWinner):
                SetGameState(GameState.GameLobby); // does not go back to start menu
                break;
        }
    }

    void RunGameState()
    {
        switch(gameState)
        {
            case (GameState.None):
                if (IsServer || IsHost)
                {
                    countdownValue = countdownStart;
                }
                break;
            case (GameState.GameLobby):
                break;
            case (GameState.GameCountdown):
                if (IsServer || IsHost)
                {
                    countdownValue -= Time.deltaTime;
                    InvokeClientRpcOnEveryone(ServerMessage, "Counting down..." + Mathf.Ceil(countdownValue));

                    if (countdownValue <= 0f)
                    {
                        countdownValue = countdownStart;
                        SetGameState(GameState.GamePlay);
                    }
                }
                break;
            case (GameState.GamePlay):
                Debug.Log("Playing...");
                break;
            case (GameState.GameWinner):
                Debug.Log("Who won?");
                break;
        }
    }

    public void SetGameState(GameState newState)
    {
        if (IsServer || IsHost)
        {
            gameState = newState;
            Debug.Log( "switching to new state: " + gameState);
            switch (gameState)
            {
                case (GameState.None):
                    break;
                case (GameState.GameLobby):
                    InvokeClientRpcOnEveryone(ServerMessage, "Game Lobby!");
                    CleanSpawned();
                    StartLobby();
                    break;
                case (GameState.GameCountdown):
                    CleanSpawned();
                    InvokeClientRpcOnEveryone(ServerMessage, "Initiating Countdown!");
                    break;
                case (GameState.GamePlay):
                    PlaySpawnItems();
                    InvokeClientRpcOnEveryone(ServerMessage, "Let's Play!");
                    break;
                case (GameState.GameWinner):
                    InvokeClientRpcOnEveryone(ServerMessage, "WINNER!");
                    break;
            }
        }
    }

    [ClientRPC]
    private void ServerMessage(string message)
    {
        Debug.Log(message);
    }

    /// <summary>
    /// In the lobby, we randomly place items around to play with and test.
    /// </summary>
    private void StartLobby()
    {
        if (IsServer || IsHost)
        {
            // spawn various objects
            foreach (var itemPrefab in itemPrefabs)
            {
                var item = (GameObject)Instantiate(itemPrefab, RandomStartPlanePosition(), RandomRotation());
                item.GetComponent<NetworkedObject>().Spawn();
                spawnedItems.Add(item.GetComponent<ItemBehavior>());
            }

            foreach (var item in spawnedItems)
            {
                Debug.Log(item.name);
            }
        }
    }

    private void CleanSpawned()
    {
        foreach (ItemBehavior item in spawnedItems)
        {
            Debug.Log("Unspawning: " + item.name);
            item.Unspawn();
            Destroy(item.gameObject);
        }
        spawnedItems = new List<ItemBehavior>();
    }

    private void PlaySpawnItems()
    {
        foreach (var itemPrefab in itemPrefabs)
        {
            List<ItemSpawn> spawnList = new List<ItemSpawn>();
            ItemBehavior itemBehavior = itemPrefab.GetComponent<ItemBehavior>();


            switch (itemBehavior.itemType)
            {
                case ItemType.Crown:
                    AssignSpawn(GetAvailableSpawnList("CrownSpawn"), SpawnNumberCrowns, itemPrefab);
                    break;
                case ItemType.Utility:
                    AssignSpawn(GetAvailableSpawnList("UtilitySpawn"), 1, itemPrefab);
                    AssignSpawn(GetAvailableSpawnList("ItemSpawn"), SpawnNumberUtility, itemPrefab);
                    break;
                case ItemType.Rare:
                    AssignSpawn(GetAvailableSpawnList("ItemSpawn"), SpawnNumberRare, itemPrefab);
                    break;
                case ItemType.Normal:
                    AssignSpawn(GetAvailableSpawnList("ItemSpawn"), SpawnNumberNormal, itemPrefab);
                    break;
            }
        }
    }

    public void AssignSpawn(List<ItemSpawn> spawnList, int count, GameObject itemPrefab)
    {
        for (var i = 0; i < count; i++)
        {
            if (spawnList.Count <= 0)
            {
                var item = (GameObject)Instantiate(itemPrefab, RandomStartPlanePosition(), RandomRotation());
                item.GetComponent<NetworkedObject>().Spawn();
                spawnedItems.Add(item.GetComponent<ItemBehavior>());
            }
            else
            {
                int index = Random.Range(0, spawnList.Count - 1);
                ItemSpawn spawn = spawnList[index];
                spawn.item = itemPrefab.GetComponent<ItemBehavior>();
                GameObject item = Instantiate(itemPrefab, spawn.transform.position, RandomRotation());
                item.GetComponent<NetworkedObject>().Spawn();
                spawnedItems.Add(spawn.item);
            }
        }
    }

    private Vector3 RandomStartPlanePosition()
    {
        return new Vector3(
                    Random.Range(-10.0f, 10.0f),
                    0.5f,
                    Random.Range(-10.0f, 10.0f));
    }

    private Quaternion RandomRotation()
    {
        return Quaternion.Euler(
                    0.0f,
                    Random.Range(0, 180),
                    0.0f);
    }

    public List<ItemSpawn> GetAvailableSpawnList(string searchTag)
    {
        List<ItemSpawn> itemSpawns = new List<ItemSpawn>();

        foreach (GameObject itemSpawnObj in GameObject.FindGameObjectsWithTag(searchTag))
        {
            ItemSpawn itemSpawn = itemSpawnObj.GetComponent<ItemSpawn>();

            if (itemSpawn.item == null)
            {
                itemSpawns.Add(itemSpawn.GetComponent<ItemSpawn>());
            }
        }

        return itemSpawns;
    }
}
