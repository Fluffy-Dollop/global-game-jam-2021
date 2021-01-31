using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MLAPI;
using MLAPI.NetworkedVar;
using UnityEngine.InputSystem;

public enum GameState
{
    StartMenu,
    GameCountdown,
    GamePlay,
    GameWinner,
}

public class GameManager : NetworkedBehaviour
{
    public GameObject[] itemPrefabs;
    public bool lazyInitialized;

    [SyncedVar]
    public GameState gameState = GameState.StartMenu;

    void Awake()
    {
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
        switch (gameState)
        {
            case (GameState.StartMenu):
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
            case (GameState.StartMenu):
                break;
            case (GameState.GameCountdown):
                Debug.Log("Counting down...");
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
        gameState = newState;
        Debug.Log("Switch to new state: " + gameState);
        switch (gameState)
        {
            case (GameState.StartMenu):
                break;
            case (GameState.GameCountdown):
                Debug.Log("Countdown!");
                break;
            case (GameState.GamePlay):
                Debug.Log("Play!");
                break;
            case (GameState.GameWinner):
                Debug.Log("WINNER!");
                break;
        }
    }
}
