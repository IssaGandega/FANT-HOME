using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using UnityEngine;

public class InGameUIManager : MonoBehaviour
{
    public TimerUI timerUI;
    [SerializeField] private TMP_Text enemiesRemaining;

    private int enemiesNumber = 0;

    private void Start()
    {
        enemiesRemaining.text = "0";
    }

    public void UpdateEnemiesRemaining(bool increase)
    {
        enemiesNumber += increase ? 1 : -1;
        enemiesRemaining.text = enemiesNumber.ToString();
    }
}
