using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingPlane : MonoBehaviour
{
    GameManager gameManager;
    Collider bounds;
    List<GameObject> walls = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        // disable these on start
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<MeshCollider>().enabled = false;
        bounds = transform.Find("Bounds").GetComponent<Collider>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        foreach (GameObject wall in GameObject.FindGameObjectsWithTag("LobbyWall"))
        {
            walls.Add(wall);
        }
    }

    private void Update()
    {
        switch(gameManager.gameState)
        {
            case GameState.GameCountdown:
            case GameState.GameLobby:
                CloseWalls();
                break;
            case GameState.GamePlay:
            case GameState.GameWinner:
                OpenWalls();
                break;
        }
    }

    void CloseWalls()
    {
        foreach (GameObject wall in walls)
        {
            wall.SetActive(true);
        }
    }

    void OpenWalls()
    {
        foreach (GameObject wall in walls)
        {
            wall.SetActive(false);
        }
    }

    public void Respawn(GameObject target)
    {
        Debug.Log("Got target to respawn " + target.name);

        Vector3 newPos = new Vector3(
            transform.position.x + Random.Range(-transform.lossyScale.x, transform.lossyScale.x),
            transform.position.y,
            transform.position.z + Random.Range(-transform.lossyScale.z, transform.lossyScale.z)
        );

        Debug.Log("New Position: " + newPos);

        switch(target.tag)
        {
            case "Player":
                target.GetComponent<FPC>().Warp(newPos);
                break;
            case "item":
            default:
                target.transform.position = newPos;
                break;
        }
    }

    public bool IsInside(Vector3 pos)
    {
        if (bounds.bounds.Contains(pos))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
