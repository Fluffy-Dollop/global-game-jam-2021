using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawn : MonoBehaviour
{
    public ItemType itemType;
    public ItemBehavior item;

    private void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }
}
