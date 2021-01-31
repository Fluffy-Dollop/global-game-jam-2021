using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MLAPI;
using MLAPI.NetworkedVar;

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
    public NetworkedVar<GameState> gameState = new NetworkedVar<GameState>(GameState.StartMenu);

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
        if (NetworkingManager.Singleton.IsHost && !lazyInitialized)
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
    }

    void RunGameState()
    {
        switch(gameState.Value)
        {
            case (GameState.StartMenu):
                break;
            case (GameState.GameCountdown):
                break;
            case (GameState.GamePlay):
                break;
            case (GameState.GameWinner):
                break;
        }
    }

    public void SetGameState(GameState newState)
    {
        gameState.Value = newState;
        Debug.Log("Switch to new state: " + gameState.Value);
        switch (gameState.Value)
        {
            case (GameState.StartMenu):
                break;
            case (GameState.GameCountdown):
                break;
            case (GameState.GamePlay):
                break;
            case (GameState.GameWinner):
                break;
        }
    }
}
