using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MLAPI;
using MLAPI.NetworkedVar;
using UnityEngine.InputSystem;

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

    public float countdownLength = 3.0f;
    [SyncedVar] private float co
    void Awake()
    {
    }

    private void Start()
    {
        SceneManager.LoadScene("Scenes/Level1", LoadSceneMode.Additive);
        countdownCurrent.OnValueChanged += CountDownChanged;
    }

    public bool ServerAuthorized()
    {
        return NetworkingManager.Singleton.IsServer || NetworkingManager.Singleton.IsHost;
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

        if (ServerAuthorized())
        {
            Debug.Log("I am Authorized!");
            RunGameState();
        }
    }


    public void NextGameState()
    {
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


    /// <summary>
    /// Only allowed by server or host, changes values for networked components during play
    /// </summary>
    void RunGameState()
    {
        switch(gameState.Value)
        {
            case (GameState.None):
                break;
            case (GameState.GameCountdown):
                if (countdownCurrent.Value <= 0f)
                {
                    SetGameState(GameState.GamePlay);
                    countdownCurrent.Value = countdownLength;
                } else
                {
                    countdownCurrent.Value -= Time.deltaTime;
                    Debug.Log("Server triggering new countdown value:" + countdownCurrent.Value);
                }
                break;
            case (GameState.GamePlay):
                break;
            case (GameState.GameWinner):
                break;
        }
    }

    /// <summary>
    /// Only allowed by server or host, changes a game state
    /// </summary>
    /// <param name="newState"></param>
    public void SetGameState(GameState newState)
    {
        // only host or server can update game state
        if (ServerAuthorized())
        {
            gameState.Value = newState;
            switch (gameState.Value)
            {
                case (GameState.None):
                    break;
                case (GameState.GameCountdown):
                    countdownCurrent.Value = countdownLength;
                    break;
                case (GameState.GamePlay):
                    break;
                case (GameState.GameWinner):
                    break;
            }
        }
    }

    /// <summary>
    /// Listener that triggers when a game state has changed
    /// </summary>
    /// <param name="prevGameState"></param>
    /// <param name="newGameState"></param>
    public void GetGameStateValueChanged(GameState prevGameState, GameState newGameState)
    {
        Debug.Log("Switch from " + prevGameState + " to new state: " + newGameState);
        switch (newGameState)
        {
            case (GameState.None):
                break;
            case (GameState.GameCountdown):
                Debug.Log("Counting Down!");
                break;
            case (GameState.GamePlay):
                Debug.Log("Start!");
                break;
            case (GameState.GameWinner):
                break;
        }
    }
    
    void CountDownChanged(float prevValue, float newValue)
    {
        if (gameState.Value == GameState.GameCountdown)
        {
            Debug.Log("Counting down... " + Mathf.Floor(newValue));
        }
    }
}
