using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemSpawnType
{
    Normal,
    Crown
}

public class ItemSpawn : MonoBehaviour
{
    public ItemSpawnType itemSpawnType = ItemSpawnType.Normal;
    public ItemBehavior item;

    private void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }
}
