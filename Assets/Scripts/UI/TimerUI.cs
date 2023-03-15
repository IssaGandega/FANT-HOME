using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void UpdateSliderValue(float newValue)
    {
        slider.value = newValue;
    }
}
