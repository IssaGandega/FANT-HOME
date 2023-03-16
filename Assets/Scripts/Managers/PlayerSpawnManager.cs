using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawnManager : MonoBehaviour
{
    [SerializeField] private Room initialRoom;
    [SerializeField] private PlayerStatsSO stats;
    [SerializeField] private GameObject PlayerGO;
    [SerializeField] private CinemachineTargetGroup targetGroup;
    [SerializeField] private PlayerUI[] playersUI;
    [SerializeField] private Transform[] spawnLocations;
    [SerializeField] private Color[] colors;

    private static readonly int PlayerColor = Shader.PropertyToID("_PlayerColor");
    public List<GameObject> playersList = new List<GameObject>();

    private void Start()
    {
        if (PlayersManager.instance == null) return;
        for (int i = 0; i < PlayersManager.instance.GetPlayerCount; i++)
        {
            PlayerInput.Instantiate(PlayerGO, controlScheme: "Controller", pairWithDevice: PlayersManager.instance.devices[i]);
            Debug.Log("Player " + i + " spawned.");
        }
    }


    public void SpawnPlayer(PlayerInput playerInput)
    {
        Debug.Log("PlayerInput ID: " + playerInput.playerIndex);
        
        targetGroup.AddMember(playerInput.transform, 1,2);

        foreach (var meshRenderer in playerInput.gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer.material.SetColor(PlayerColor, colors[playerInput.playerIndex]);
        }

        playerInput.gameObject.GetComponent<Player>().NumPlayer.text = "P" + (playerInput.playerIndex + 1);
        playerInput.gameObject.GetComponent<Player>().NumPlayer.color = colors[playerInput.playerIndex];
        playerInput.gameObject.GetComponent<Player>().Arrow.color = colors[playerInput.playerIndex];
        
        GameManager.instance.AddToAliveList();

        playerInput.GetComponent<Player>().playerUI = playersUI[playerInput.playerIndex];

        GameObject playGO;
        
        (playGO = playerInput.gameObject).transform.position = spawnLocations[playerInput.playerIndex].position;

        playGO.GetComponent<Player>().curRoom = initialRoom;

        playGO.GetComponent<PlayerHealth>().curHealth = stats.maxHealth;
        playGO.GetComponent<PlayerHealth>().invincibleTime = stats.invincibleTime;
        
        playersList.Add(playGO);
        GameManager.instance.started = true;
    }
    
}
