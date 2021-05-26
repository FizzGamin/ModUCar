using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishMenuUI : ToggleableUI
{
    // Start is called before the first frame update
    void Start()
    {
        UIManager.SetFinishMenuUI(this);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Calls the players respawn method to respawn and closes the deathMenuUI.
    /// </summary>
    public void Restart()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerController>().Respawn();
        Close();
    }

    /// <summary>
    /// Loads the main menu which is scene index 0.
    /// </summary>
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Quits the game.
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }
}
