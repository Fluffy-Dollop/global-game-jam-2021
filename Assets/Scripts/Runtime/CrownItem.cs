using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Prototyping;

public class CrownItem : ItemBehavior
{
    GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public override void OnActivate()
    {
        // trigger victory!
        gameManager.SetGameState(GameState.GameWinner);
        gameManager.winner = holdingPlayer.GetComponent<FPC>().playerName.Value;
        Debug.Log("winner: " + gameManager.winner);
    }
}
