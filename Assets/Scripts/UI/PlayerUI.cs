using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Image[] weaponIcons;
    [SerializeField] private GameObject[] lifeBars;
    
    public void UpdateWeaponUI(WeaponsSO newWeapon)
    {
        weaponIcons[0].sprite = newWeapon.icon;
        weaponIcons[1].sprite = newWeapon.flashLight.icon;
    }

    public void UpdateHealthUI(int newHealth)
    {
        var clampHealth = Mathf.Clamp(newHealth,0, 4);

        for (int i = 0; i < lifeBars.Length; i++)
        {
            lifeBars[i].SetActive(i + 1 <= clampHealth);
        }
    }
}
