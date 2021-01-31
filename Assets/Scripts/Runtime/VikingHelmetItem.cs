using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Prototyping;

public class VikingHelmetItem : ItemBehavior
{
    float multiplier = 1.5f;

    public AudioSource audioSource;

    public override void OnPickUp()
    {
        // play theme song
        if (audioSource)
        {
            audioSource.Play();
        }

        // put it on our head
        transform.localPosition = new Vector3(0, 1, 0);
        transform.localEulerAngles = new Vector3(0, 90, 0);

        // give player controlling you enhanced speed
        holdingPlayer.GetComponent<FPC>().speed *= multiplier;
    }

    public override void OnDrop()
    {
        // stop the theme song
        if (audioSource)
        {
            audioSource.Stop();
        }

        // give player controlling you reduced speed
        holdingPlayer.GetComponent<FPC>().speed /= multiplier;
    }
}
