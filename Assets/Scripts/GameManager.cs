using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public WaveTool waveTool;
    public RoomsManager roomsManager;
    public PlayerSpawnManager playerManager;
    public MessageDisplayer messageDisplayer;
    public InGameUIManager inGameUiManager;

    private void Awake()
    {
        if(instance) Destroy(gameObject);
        else
        {
            instance = this;
        }

        if (waveTool == null) waveTool = FindObjectOfType<WaveTool>();
        if (roomsManager == null) roomsManager = FindObjectOfType<RoomsManager>();
        if (playerManager == null) playerManager = FindObjectOfType<PlayerSpawnManager>();
        if (messageDisplayer == null) messageDisplayer = FindObjectOfType<MessageDisplayer>();
        if (inGameUiManager == null) inGameUiManager = FindObjectOfType<InGameUIManager>();
    }
}
