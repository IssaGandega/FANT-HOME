using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayersManager : MonoBehaviour
{
    public static PlayersManager instance;
    
    public List<InputDevice> devices = new List<InputDevice>();
    public GameObject PlayerGO;
    
    //[SerializeField] private List<PlayerInput> inputs = new List<PlayerInput>();

    public int GetPlayerCount => devices.Count;

    private void Awake()
    {
        if (!instance) instance = this;
        else Destroy(this.gameObject);
        
        DontDestroyOnLoad(this);
    }

    public void PlayerJoin(PlayerInput playerInput)
    {
        if (devices.Contains(playerInput.devices[0])) return;
        
        devices.Add(playerInput.devices[0]);
        
        //playerInput.transform.SetParent(gameObject.transform);
        //Destroy(playerInput.gameObject);
    }

    public void TransferPlayerInput(GameObject[] playersGO)
    {
        for (int i = 0; i < playersGO.Length; i++)
        {
            /*PlayerInput InputToCopy = inputs[i];
            InputToCopy.DeactivateInput();
            Debug.Log(InputToCopy.devices[0]);
            PlayerInput input = playersGO[i].AddComponent<PlayerInput>();
            
            
            input.actions = InputToCopy.actions;
            StartCoroutine(AssignDevice(input));
            Debug.Log(Gamepad.all[0]);
            input.SwitchCurrentActionMap(InputToCopy.currentActionMap.name);
            
            
            Debug.Log(input.currentActionMap);*/
            
            
        }
    }
}
