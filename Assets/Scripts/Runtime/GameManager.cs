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
    GameCountdown,
    GamePlay,
    GameWinner,
}

public class GameManager : NetworkedBehaviour
{
    public GameObject[] itemPrefabs;
    public bool lazyInitialized;

    [SyncedVar]
    public GameState gameState = GameState.None;

    [SyncedVar]
    float countdownValue;
    [SerializeField]
    float countdownStart = 3f;

    [SyncedVar]
    public string winner = "";

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
        if (IsHost && !lazyInitialized)
        {
            lazyInitialized = true;

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
            }
        }

        RunGameState();
    }


    public void NextGameState()
    {
        Debug.Log("Manually editing state");
        switch (gameState)
        {
            case (GameState.None):
                SetGameState(GameState.GameCountdown);
                break;
            case (GameState.GameCountdown):
                SetGameState(GameState.GamePlay);
                break;
            case (GameState.GamePlay):
                SetGameState(GameState.GameWinner);
                break;
            case (GameState.GameWinner):
                SetGameState(GameState.GameCountdown); // does not go back to start menu
                break;
        }
    }

    void RunGameState()
    {
        switch(gameState)
        {
            case (GameState.None):
                countdownValue = countdownStart;
                break;
            case (GameState.GameCountdown):
                if (IsServer || IsHost)
                {
                    countdownValue -= Time.deltaTime;
                    InvokeClientRpcOnEveryone(ServerMessage, "Counting down..." + Mathf.Ceil(countdownValue));

                    if (countdownValue <= 0f)
                    {
                        SetGameState(GameState.GamePlay);
                        countdownValue = countdownStart;
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
            Debug.Log("Switch to new state: " + gameState);
            switch (gameState)
            {
                case (GameState.None):
                    break;
                case (GameState.GameCountdown):
                    InvokeClientRpcOnEveryone(ServerMessage, "Initiating Countdown!");
                    break;
                case (GameState.GamePlay):
                    InvokeClientRpcOnEveryone(ServerMessage, "Initiating Play!");
                    break;
                case (GameState.GameWinner):
                    InvokeClientRpcOnEveryone(ServerMessage, "Initiating WINNER!");
                    break;
            }
        }
    }

    [ClientRPC]
    private void ServerMessage(string message)
    {
        Debug.Log(message);
    }
}
