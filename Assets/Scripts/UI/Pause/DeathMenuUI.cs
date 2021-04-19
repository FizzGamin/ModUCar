using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathMenuUI : ToggleableUI
{
    private const string DEATH_SCREEN_NAME = "DeathScreen";
    private const string RESPAWN_NAME = "Respawn";
    private const string MENU_NAME = "Main Menu";
    private const string QUIT_NAME = "Quit";

    private Button RespawnButton;
    private Button MenuButton;
    private Button QuitButton;
    private GameObject DeathScreen;

    public GameObject playerPrefab;
    private Vector3 spawnPos;

    // Start is called before the first frame update
    void Start()
    {
        UIManager.SetDeathMenuUI(this);

        /*DeathScreen = transform.Find(DEATH_SCREEN_NAME).gameObject;
        RespawnButton = GetButtonByName(RESPAWN_NAME);
        MenuButton = GetButtonByName(MENU_NAME);
        QuitButton = GetButtonByName(QUIT_NAME);

        RespawnButton.onClick.AddListener(Close);*/
        
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Respawn()
    {
        GameObject player = GameObject.Find("Player");
        player.GetComponent<PlayerController>().Respawn();
        //GameObject deathCam = GameObject.FindGameObjectWithTag("MainCamera");
        //Destroy(deathCam);
        //Instantiate(playerPrefab);
        //GameObject.FindGameObjectWithTag("Player").transform.position = spawnPos;
        //Destroy(GameObject.FindGameObjectWithTag("Ragdoll"));
        Close();
    }

    public void GoToMainMenu()
    {
        //main menu scene is at index 0
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Debug.Log("The game has quit");
        Application.Quit();
    }

    public void SetSpawnPoint(Vector3 pos)
    {
        spawnPos = pos;
    }
    private Button GetButtonByName(string name)
    {
        return DeathScreen.transform.Find(name).GetComponent<Button>();
    }
}
