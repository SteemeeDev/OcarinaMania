using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.J))
        {
            PlayGame();
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("CopyOfUIScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}  

