using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void TrainButton()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void MiniGameButton()
    {
        SceneManager.LoadScene("BearMiniGame");
    }

    public void ExitButton()
    {
        Application.Quit();
    }
    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
