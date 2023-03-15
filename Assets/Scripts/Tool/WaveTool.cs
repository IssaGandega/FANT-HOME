using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using UnityEngine;

public class WaveTool : MonoBehaviour
{
    public GhostSpawner spawner;
    public Action<int> NewWave;

    public string waveName = "Wave";
    public Wave[] waves = new Wave[3];

    public int index = 0;
    private float timer;
    private TimerUI timerUI;

    private void Start()
    {
        timerUI = GameManager.instance.inGameUiManager.timerUI;
        index = 0;
        if (waves.Length == 0) return;
        SpawnWave(waves[index]);
    }

    [ContextMenu("NextWave")]
    // ReSharper disable Unity.PerformanceAnalysis
    private void GoToNextWave()
    {
        index++;
        if (index == 3 || index == 5 || index == 7)
        {
            AudioManager.Instance.PlayNextMusic();
        }
        if (index > waves.Length-1) return;
        SpawnWave(waves[index]);
    }

    private void SpawnWave(Wave wave)
    {
        if (wave.enemies.Length == 0) return;
        NewWave?.Invoke(index);
        GameManager.instance.messageDisplayer.DisplayText("Wave " + index, MessageDisplayer.TextHeight.HEADER, 3);
        foreach (var enemy in wave.enemies)
        {
            spawner.MakeWave(enemy.number, enemy.myType.ToString());
        }

        StartCoroutine(WaveDuration(wave.spawnTime));
    }

    private IEnumerator WaveDuration(float t)
    {
        float timer = t;
        float maxTime = t;
        while (timer>=0)
        {
            yield return new WaitForEndOfFrame();
            timer -= Time.deltaTime;
            timerUI.UpdateSliderValue(timer/t);
        }
        GoToNextWave();
    }
}
