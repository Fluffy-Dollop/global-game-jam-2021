using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MLAPI;

public class GameStateTracker : NetworkedBehaviour
{
    public GameManager gameManager;
    public TMP_Text text;

    [SerializeField] float blinkGameState = 1f;
    [SerializeField] float blinkTime = 0f;
    bool blinkOn = true;

    // Update is called once per frame
    void Update()
    {
        if (gameManager.gameState == GameState.GameLobby) {
            if (IsServer || IsHost)
            {
                text.text = "Lobby: Press \"ENTER\" to begin the match.";
            } else {
                text.text = "Lobby: Ask the host to press \"ENTER\" to begin the match.";
            }
        } else
        {
            text.text = "";
        }

        blinkTime += Time.deltaTime;

        if (blinkOn && blinkTime >= blinkGameState)
        {
            blinkOn = false;
            blinkTime = 0f;
        } else if (!blinkOn && blinkTime >= blinkGameState * 0.1f)
        {
            blinkOn = true;
            blinkTime = 0f;
        }
    }
}
