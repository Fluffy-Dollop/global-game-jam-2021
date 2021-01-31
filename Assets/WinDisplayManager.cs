using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinDisplayManager : MonoBehaviour
{
    public GameManager gameManager;
    public TMPro.TMP_Text winDisplayText;

    private void Update()
    {
        if (gameManager.gameState == GameState.GameWinner)
        {
            winDisplayText.gameObject.SetActive(true);
            winDisplayText.text = "Winner: " + gameManager.winner;
        }
        else
        {
            winDisplayText.gameObject.SetActive(false);
            winDisplayText.text = "";
        }
    }
}
