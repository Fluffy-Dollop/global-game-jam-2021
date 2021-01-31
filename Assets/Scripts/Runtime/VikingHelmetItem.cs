using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Prototyping;

public class VikingHelmetItem : ItemBehavior
{
    float multiplier = 1.5f;

    public override void OnPickUp()
    {
        // put it on our head
        transform.localPosition = new Vector3(0, 1, 0);
        transform.localEulerAngles = new Vector3(0, 90, 0);

        // give player controlling you enhanced speed
        print("VikingHelmetItem.OnPickUp: " + holdingPlayer.GetComponent<FPC>().speed + " *= " + multiplier);
        holdingPlayer.GetComponent<FPC>().speed *= multiplier;
    }

    public override void OnDrop()
    {
        print("VikingHelmetItem.OnDrop: " + holdingPlayer.GetComponent<FPC>().speed + " /= " + multiplier);
        // give player controlling you reduced speed
        holdingPlayer.GetComponent<FPC>().speed /= multiplier;
    }
}
