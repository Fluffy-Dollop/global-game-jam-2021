using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Prototyping;

public class HeartItem : ItemBehavior
{
    public override void OnActivate()
    {
        // give player controlling you more health
        holdingPlayer.GetComponent<FPC>().Heal(1.0f);

        // respawn the heart
        Respawn();
    }
}
