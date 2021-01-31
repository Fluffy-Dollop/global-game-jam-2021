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
                    CleanLobby();
                    StartLobby();
                    break;
                case (GameState.GameCountdown):
                    InvokeClientRpcOnEveryone(ServerMessage, "Initiating Countdown!");
                    break;
                case (GameState.GamePlay):
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
                var spawnPosition = new Vector3(
                    Random.Range(-10.0f, 10.0f),
                    0.5f,
                    Random.Range(-10.0f, 10.0f));

                var spawnRotation = Quaternion.Euler(
                    0.0f,
                    Random.Range(0, 180),
                    0.0f);
                var item = (GameObject)Instantiate(itemPrefab, spawnPosition, spawnRotation);
                item.GetComponent<NetworkedObject>().Spawn();
                spawnedItems.Add(item.GetComponent<ItemBehavior>());
            }

            foreach (var item in spawnedItems)
            {
                Debug.Log(item.name);
            }
        }
    }

    private void CleanLobby()
    {
        foreach (ItemBehavior item in spawnedItems)
        {
            Debug.Log("Unspawning: " + item.name);
            item.Unspawn();
            Destroy(item.gameObject);
        }
        spawnedItems = new List<ItemBehavior>();
    }
}
