using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MLAPI;

public class GameManager : NetworkedBehaviour
{
    public GameObject torchPrefab;
    public bool lazyInitialized;

    void Awake()
    {
    }

    private void Start()
    {
        SceneManager.LoadScene("Scenes/Level1", LoadSceneMode.Additive);
    }

    public GameObject FindClosestItem(Vector3 position, float range)
    {
        GameObject closest = null;

        Collider[] colliders;
        // Presuming the object you are testing also has a collider 0 otherwise
        if ((colliders = Physics.OverlapSphere(position, range)).Length > 1)
        {
            foreach (var collider in colliders)
            {
                var go = collider.gameObject;
                // ignore certain cases
                if (!go) { continue; }
                //Debug.Log("found object " + go.name);
                if (go.tag != "item") { continue; } // for now ignore non-items!
                // if we made it this far, pick the closest
                if (!closest || (go.transform.position - position).sqrMagnitude < (closest.transform.position - position).sqrMagnitude)
                {
                    closest = go;
                }
            }
        }

        Debug.Log("FindClosestItem returning " + (closest ? closest.name : "<null>"));
        return closest;
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkingManager.Singleton.IsHost && !lazyInitialized)
        {
            lazyInitialized = true;
            // spawn a torch
            var spawnPosition = new Vector3(
                Random.Range(-1.0f, 1.0f),
                0.0f,
                Random.Range(-1.0f, 1.0f));

            var spawnRotation = Quaternion.Euler(
                0.0f,
                Random.Range(0, 180),
                0.0f);
            var torch = (GameObject)Instantiate(torchPrefab, spawnPosition, spawnRotation);
            torch.GetComponent<NetworkedObject>().Spawn();
        }
    }
}
