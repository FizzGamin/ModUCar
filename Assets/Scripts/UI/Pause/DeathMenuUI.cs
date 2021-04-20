using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathMenuUI : ToggleableUI
{
    // Start is called before the first frame update
    void Start()
    {
        UIManager.SetDeathMenuUI(this);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Calls the players respawn method to respawn and closes the deathMenuUI.
    /// </summary>
    public void Respawn()
    {
        GameObject player = GameObject.Find("Player");
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
