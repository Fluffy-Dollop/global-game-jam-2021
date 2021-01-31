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
        base.Start();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public override void OnPickUp()
    {
        // put it on our head
        transform.localPosition = new Vector3(0, 1, 0);
        transform.localEulerAngles = new Vector3(0, 90, 0);
    }

    public override void OnActivate()
    {
        // trigger victory!
        gameManager.SetGameState(GameState.GameWinner);
        gameManager.winner = holdingPlayer.GetComponent<FPC>().playerName.Value;
        Debug.Log("winner: " + gameManager.winner);
    }
}
