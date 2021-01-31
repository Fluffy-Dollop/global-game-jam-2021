using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaController : MonoBehaviour {
    private Material lavaMaterial;
    // Start is called before the first frame update
    void Start() {
        MeshRenderer r = GetComponent<MeshRenderer>();
        r.material = new Material(r.sharedMaterial);
        lavaMaterial = r.material;
    }

    // Update is called once per frame
    void Update() {
        float t = Time.timeSinceLevelLoad * 0.1f;
        float x = Mathf.PerlinNoise(t, 0f);
        float y = Mathf.PerlinNoise(0.0f, t);
        lavaMaterial.SetFloat("_DistortX", x);
        lavaMaterial.SetFloat("_DistortY", y);
    }
}
