using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public Transform cameraContainer;
    private float cameraRotateSpeed = 6f;

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Update()
    {
        //Rotate the main menu camera slowly
        cameraContainer.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * cameraRotateSpeed);
    }

}
