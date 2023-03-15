using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public EventSystem eventSys;

    private void Awake()
    {
        if (eventSys == null) eventSys = EventSystem.current;
    }

    public void FocusOn(GameObject target)
    {
        eventSys.SetSelectedGameObject(target);
        
    }

    private void Update()
    {
        Debug.Log(eventSys.currentSelectedGameObject);
    }
}
