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
    [SerializeField]
    float countdownWinStart = 5f;

    [SyncedVar]
    public string winner = "";

    [SerializeField] int SpawnNumberCrowns = 1;
    [SerializeField] int SpawnNumberNormal = 2;
    [SerializeField] int SpawnNumberUtility = 2; // also once in the utility spot
    [SerializeField] int SpawnNumberRare = 1;

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

        if (IsServer || IsHost)
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
                    CountDownToGameState(GameState.GamePlay, countdownWinStart, "Counting down to begin game...");
                }
                break;
            case (GameState.GamePlay):
                Debug.Log("Playing...");
                break;
            case (GameState.GameWinner):
                Debug.Log("Who won?");
                if (IsServer || IsHost)
                {
                    CountDownToGameState(GameState.GameLobby, countdownStart, "Counting down to restart game...");
                }
                break;
        }
    }

    void CountDownToGameState(GameState newGameState, float nextStart, string message)
    {
        countdownValue -= Time.deltaTime;
        InvokeClientRpcOnEveryone(ServerMessage, message + Mathf.Ceil(countdownValue));

        if (countdownValue <= 0f)
        {
            countdownValue = nextStart;
            SetGameState(newGameState);
        }
    }

    public void SetGameState(GameState newState)
    {
        gameState = newState;
        Debug.Log("switching to new state: " + gameState);
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
                countdownValue = countdownWinStart; // resetting for the win state
                break;
            case (GameState.GameWinner):
                InvokeClientRpcOnEveryone(ServerMessage, "WINNER!");
                break;
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
            }
        }
    }

    private void CleanSpawned()
    {
        foreach (GameObject item in GameObject.FindGameObjectsWithTag("item"))
        {
            Debug.Log("Unspawning: " + item.name);
            NetworkedObject no = item.GetComponent<NetworkedObject>();
            if (no.IsSpawned)
            {
                no.UnSpawn();
            }
            Destroy(item.gameObject);
        }
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
                    LoopAssignSpawn(SpawnNumberCrowns, GetAvailableSpawnList("CrownSpawn"), itemPrefab);
                    break;
                case ItemType.Utility:
                    LoopAssignSpawn(1, GetAvailableSpawnList("UtilitySpawn"), itemPrefab);
                    LoopAssignSpawn(SpawnNumberUtility, GetAvailableSpawnList("ItemSpawn"), itemPrefab);
                    break;
                case ItemType.Rare:
                    LoopAssignSpawn(SpawnNumberRare, GetAvailableSpawnList("ItemSpawn"), itemPrefab);
                    break;
                case ItemType.Normal:
                    LoopAssignSpawn(SpawnNumberNormal, GetAvailableSpawnList("ItemSpawn"), itemPrefab);
                    break;
            }
        }
    }

    public void LoopAssignSpawn(int count, List<ItemSpawn> spawnList, GameObject itemPrefab)
    {
        for (var i = 0; i < count; i++)
        {
            var item = (GameObject)Instantiate(itemPrefab, RandomStartPlanePosition(), RandomRotation());
            AssignSpawn(spawnList, item);
        }
    }

    public void AssignSpawn(List<ItemSpawn> spawnList, GameObject item)
    {
        if (spawnList.Count > 0)
        {
            int index = Random.Range(0, spawnList.Count - 1);
            ItemSpawn spawn = spawnList[index];
            spawn.item = item.GetComponent<ItemBehavior>();
            item.transform.position = spawn.transform.position;
        }

        NetworkedObject no = item.GetComponent<NetworkedObject>();
        if (!no.IsSpawned)
        {
            no.Spawn();
        }
    }

    public void Respawn(ItemBehavior item)
    {
        switch (item.itemType)
        {
            case ItemType.Crown:
                AssignSpawn(GetAvailableSpawnList("CrownSpawn"), item.gameObject);
                break;
            case ItemType.Utility:
                AssignSpawn(GetAvailableSpawnList("UtilitySpawn"), item.gameObject);
                AssignSpawn(GetAvailableSpawnList("ItemSpawn"), item.gameObject);
                break;
            case ItemType.Rare:
                AssignSpawn(GetAvailableSpawnList("ItemSpawn"), item.gameObject);
                break;
            case ItemType.Normal:
                AssignSpawn(GetAvailableSpawnList("ItemSpawn"), item.gameObject);
                break;
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
