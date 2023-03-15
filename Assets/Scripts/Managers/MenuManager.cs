using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button[] mainButtons;
    
    public void SwitchScene()
    {
        SceneManager.LoadScene("TestsScene");
    }

    public void SetMainButtons(bool enable)
    {
        foreach (var button in mainButtons)
        {
            button.interactable = enable;
        }
    }
}
