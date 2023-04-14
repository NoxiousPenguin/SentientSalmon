using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManagerClone : MonoBehaviour
{
    [SerializeField] private GameObject gameMenu;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject tutorial;
    [SerializeField] private GameObject timeAndScore;
    [SerializeField] private SpawnSalmon spawnSalmonScript;
    [SerializeField] private CatchFish catchFishScript;
    private bool gamePaused;
    private bool gameOver;

    void Awake()
    {
        ActivateButtons(gameMenu);
        ActivateButtons(pauseMenu);
    } 
            
    private void Start(){
        gamePaused = false;
        gameOver = false;
        EvolutionManager.Instance.StartEvolution();
    }

    private void Update(){
        if (!gameOver && spawnSalmonScript.timeRemaining <= 0){
            EndGame();
        }

        if (!gameOver && Input.GetKeyDown(KeyCode.P)){
            if (gamePaused){
                UnpauseGame();
            } else {
                PauseGame();
            }
        }
    }

    public void EndGame(){
        gameOver = true;
        Time.timeScale = 0f;
        gameMenu.SetActive(true);
        timeAndScore.SetActive(false);
        scoreText.text = "Score: " + catchFishScript.score.ToString();
    }

    public void RestartGame(){
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu(){
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScreen");
    }

    private void PauseGame(){
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        tutorial.SetActive(true);
        gamePaused = true;
    }

    private void UnpauseGame(){
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        tutorial.SetActive(false);
        gamePaused = false;
    }

    // adds callback functions to buttons dependent on its name
    private void ActivateButtons(GameObject gameObject)
    {
        if (gameObject != null)
        {
            Button[] buttons = gameObject.GetComponentsInChildren<Button>();
            foreach (Button button in buttons)
            {
                Debug.Log("Name of button: " + button.name);
                if (button.name == "MainMenuButton")
                {
                    button.onClick.AddListener(delegate { GoToMainMenu(); });
                }

                else if (button.name == "RestartButton")
                {
                    button.onClick.AddListener(delegate { RestartGame(); });
                }

            }
        }
    }
}
