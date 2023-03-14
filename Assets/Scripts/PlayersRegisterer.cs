using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayersRegisterer : MonoBehaviour
{
    
    [SerializeField] private int playersMaxNumber;
    
    [SerializeField] private Button LaunchButton;
    [SerializeField] private TMP_Text playersNumDisp;

    private void Start()
    {
        playersNumDisp.text = "Players : " + PlayersManager.instance.GetPlayerCount + "/" + playersMaxNumber;
    }

    public void OnPlayerJoined(PlayerInput player)
    {
        Debug.Log("Join");
        if (PlayersManager.instance.GetPlayerCount >= playersMaxNumber) return;
        PlayersManager.instance.PlayerJoin(player);
        
        int playCount = PlayersManager.instance.GetPlayerCount;
        LaunchButton.interactable = playCount >= playersMaxNumber;

        playersNumDisp.text = "Players : " + playCount + "/" + playersMaxNumber;
        
    }
}
