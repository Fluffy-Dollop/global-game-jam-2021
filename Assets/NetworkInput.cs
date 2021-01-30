using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NetworkInput : MonoBehaviour
{
    public NetworkMode networkMode;
    public TMP_InputField serverIP_Input;

    void Start()
    {
        networkMode = GetComponent<NetworkMode>();
    }
}
