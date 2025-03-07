using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InterFaceManager : MonoBehaviour
{
    [SerializeField] public Button quitBtn;
    [SerializeField] public Button startBtn;

    [SerializeField] public Button GoToMenuBtn;

    [SerializeField] public GameObject MenuScreen;
    [SerializeField] public GameObject LevelsScreen;

    [SerializeField] public Button GoToSandBoxBtn;

    public void Start()
    {
        LevelsScreen.SetActive(false);
        MenuScreen.SetActive(true);

        var currentLevel = LevelManager.currentLevel;
        if (currentLevel != -1)
        {

            LevelsScreen.SetActive(true);
            MenuScreen.SetActive(false);
            LevelSelector.instance.selectLevel(currentLevel);
        }

        quitBtn.onClick.AddListener(QuitGame);
        startBtn.onClick.AddListener(StartGame);
        GoToMenuBtn.onClick.AddListener(GoToMenu);
        GoToSandBoxBtn.onClick.AddListener(GoToSandBox);
    }


    public void GoToSandBox()
    {
        SceneManager.LoadScene("SandBox");
    }

    public void StartGame()
    {
        MenuScreen.SetActive(false);
        LevelsScreen.SetActive(true);
    }

    public void GoToMenu()
    {
        MenuScreen.SetActive(true);
        LevelsScreen.SetActive(false);
        LevelSelector.instance.deselectAll();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
