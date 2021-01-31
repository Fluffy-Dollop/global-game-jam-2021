using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkedVar;
using MLAPI.Prototyping;

public class CrownItem : ItemBehavior
{
    public override void OnPickUp()
    {
        // put it on our head
        transform.localPosition = new Vector3(0, 1, 0);
        transform.localEulerAngles = new Vector3(0, 90, 0);
    }

    public override void OnActivate()
    {
        // trigger victory!
        RequestGameWin(holdingPlayer);

        // play victory music
        GetComponent<AudioSource>().Play();
    }

    public override void OnDespawn()
    {
        // stop victory music
        GetComponent<AudioSource>().Stop();
    }

    public void RequestGameWin(GameObject player)
    {
        print("CrownItem.RequestGameWin()");
        // just grant it! Why the hell not?
        ulong playerNetID = player.GetComponent<NetworkedObject>().NetworkId;
        InvokeServerRpc(RequestWinRPC, playerNetID);
    }

    [ServerRPC]
    private void RequestWinRPC(ulong playerNetID)
    {
        print("CrownItem.RequestWinRPC()");
        gameManager.winner = GetNetworkedObject(playerNetID).gameObject.GetComponent<FPC>().playerName.Value;
        gameManager.SetGameState(GameState.GameWinner);
        Debug.Log("winner: " + gameManager.winner);
    }
}
