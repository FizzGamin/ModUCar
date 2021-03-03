using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicPauseMenuUI : MonoBehaviour, PauseMenuUI
{
    private const string PAUSE_SCREEN_NAME = "PauseScreen";
    private const string RESUME_NAME = "Resume";
    private const string OPTIONS_NAME = "Options";
    private const string QUIT_NAME = "Quit";

    private Button ResumeButton;
    private Button OptionsButton;
    private Button QuitButton;
    private GameObject PauseScreen;
    void Start()
    {
        UIManager.SetPauseMenuUI(this);

        PauseScreen = transform.Find(PAUSE_SCREEN_NAME).gameObject;
        ResumeButton = GetButtonByName(RESUME_NAME);
        OptionsButton = GetButtonByName(OPTIONS_NAME);
        QuitButton = GetButtonByName(QUIT_NAME);

        ResumeButton.onClick.AddListener(Close);

        gameObject.SetActive(false);
    }

    private Button GetButtonByName(string name)
    {
        return PauseScreen.transform.Find(name).GetComponent<Button>();
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        GameManager.GetPlayer().PassControl();
    }
}
