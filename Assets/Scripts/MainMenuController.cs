using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Transform cameraContainer;
    public GameObject mainMenuPanel;
    public GameObject settingsMenuPanel;

    //Settings
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown resolutionDropdown;

    private float cameraRotateSpeed = 6f;

    private void Start()
    {
        OpenMainMenu();
    }

    public void OnQualityDropdownChange()
    {
        QualitySettings.SetQualityLevel(qualityDropdown.value);
    }

    public void OnResolutionDropdownChange()
    {
        switch(resolutionDropdown.value)
        {
            case 0:
                Screen.SetResolution(1920, 1080, true);
                break;
            case 1:
                Screen.SetResolution(2560, 1440, true);
                break;
            case 2:
                Screen.SetResolution(3840, 2160, true);
                break;
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenMainMenu()
    {
        CloseAllMenus();
        mainMenuPanel.SetActive(true);
    }

    public void OpenSettingsMenu()
    {
        CloseAllMenus();
        settingsMenuPanel.SetActive(true);
    }

    public void CloseAllMenus()
    {
        mainMenuPanel.SetActive(false);
        settingsMenuPanel.SetActive(false);
    }



    public void Update()
    {
        //Rotate the main menu camera slowly
        cameraContainer.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * cameraRotateSpeed);
    }

}
