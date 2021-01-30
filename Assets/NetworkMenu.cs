using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkMenu : MonoBehaviour
{
    public Camera menuCamera;
    public TMPro.TMP_InputField ServerIPField;
    public string serverIP = "";
    public TMPro.TMP_InputField PlayerNameField;
    public string playerName = "";

    public void CollectInput()
    {
        serverIP = ServerIPField.text;
        playerName = PlayerNameField.text;
    }

    public void DisableDisplay()
    {
        transform.Find("Display").gameObject.SetActive(false);
        menuCamera.enabled = false;
    }

    public void EnableDisplay()
    {
        transform.Find("Display").gameObject.SetActive(true);
        menuCamera.enabled = true;
    }
}
