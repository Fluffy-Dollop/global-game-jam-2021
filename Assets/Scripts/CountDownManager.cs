using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountDownManager : MonoBehaviour
{
    public GameManager gameManager;
    public TMPro.TMP_Text countDownText;

    private void Update()
    {
        if (gameManager.gameState == GameState.GameCountdown)
        {
            countDownText.gameObject.SetActive(true);
            countDownText.text = "Countdown: " + Mathf.Ceil(gameManager.countdownValue);
        } else
        {
            countDownText.gameObject.SetActive(false);
            countDownText.text = "";
        }
    }
}
