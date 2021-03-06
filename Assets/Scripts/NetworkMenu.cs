using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class NetworkMenu : MonoBehaviour
{
    public TMPro.TMP_InputField ServerIPField;
    public string serverIP = "";
    public TMPro.TMP_InputField PlayerNameField;
    public string playerName = "";
    public TMPro.TMP_InputField RelayIPField;
    public string relayIP = "";

    private void Start()
    {
        NetworkData netData = SaveLoadNetwork.LoadData();
        if (netData != null) {
            ServerIPField.text = netData.serverIP;
            PlayerNameField.text = netData.playerName;
            RelayIPField.text = netData.relayIP;
        }
    }

    public void CollectInput()
    {
        serverIP = ServerIPField.text;
        playerName = PlayerNameField.text;
        relayIP = RelayIPField.text;
        SaveLoadNetwork.SaveData(this);
    }

    public void DisableDisplay()
    {
        transform.Find("Display").gameObject.SetActive(false);
    }

    public void EnableDisplay()
    {
        transform.Find("Display").gameObject.SetActive(true);
    }
}

[System.Serializable]
public class NetworkData
{
    public string serverIP;
    public string playerName;
    public string relayIP;

    public NetworkData(NetworkMenu networkMenu)
    {
        serverIP = networkMenu.serverIP;
        playerName = networkMenu.playerName;
        relayIP = networkMenu.relayIP;
    }
}

public static class SaveLoadNetwork
{
    public static string NetworkDataPath()
    {
        return Application.persistentDataPath + "/Network.data";
    }
    public static void SaveData(NetworkMenu networkMenu)
    {
        string savePath = NetworkDataPath();
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(savePath, FileMode.Create);
        NetworkData netData = new NetworkData(networkMenu);
        formatter.Serialize(stream, netData);
        stream.Close();
    }

    public static NetworkData LoadData()
    {
        string savePath = NetworkDataPath();
        if (File.Exists(savePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(savePath, FileMode.Open);
            NetworkData netData = formatter.Deserialize(stream) as NetworkData;
            stream.Close();
            return netData;
        } else
        {
            Debug.LogError("Error, could not find save file in path " + savePath);
            return null;
        }
    }
}