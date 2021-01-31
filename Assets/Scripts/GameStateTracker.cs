using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameStateTracker : MonoBehaviour
{
    public GameManager gameManager;
    public TMP_Text text;

    // Update is called once per frame
    void Update()
    {
        text.text = gameManager.gameState.ToString();
    }
}
