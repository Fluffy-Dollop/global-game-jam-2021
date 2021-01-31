using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
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
        gameManager.RequestGameWin(holdingPlayer);
    }
}
