using Mediapipe.Unity.Sample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameUiContorller : MonoBehaviour
{
    public static GameUiContorller instance;

    private BaseRunner _baseRunner;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button pause_resumeButton;
    [SerializeField] private Button pause_restartButton;
    [SerializeField] private Button pause_quitButton;

    [SerializeField] private GameObject finishMenu;
    [SerializeField] private Button finish_restartBtn;
    [SerializeField] private Button finish_quitBtn;
    [SerializeField] private TMPro.TextMeshProUGUI finish_scoreText;
    [SerializeField] private TMPro.TextMeshProUGUI finish_maxScoreText;

    [SerializeField] private Button dev_StartBtn;
    [SerializeField] private Button dev_PauseBtn;


    private void Awake()
    {
        _baseRunner = GameObject.FindObjectOfType<BaseRunner>();

        if (_baseRunner == null)
        {
            Debug.LogError("BaseRunner is not found in the scene. Please ensure it is added.");
        }

        instance = this;
        pause_resumeButton.onClick.AddListener(OnResumeButtonClicked);
        pause_restartButton.onClick.AddListener(OnRestartButtonClicked);
        pause_quitButton.onClick.AddListener(OnQuitButtonClicked);

        finish_restartBtn.onClick.AddListener(OnRestartButtonClicked);
        finish_quitBtn.onClick.AddListener(OnQuitButtonClicked);
        
        dev_StartBtn.onClick.AddListener(Start);
        dev_PauseBtn.onClick.AddListener(ShowPause);
    }

    public void Start()
    {
        _baseRunner.Play();
    }

    public void ShowPause()
    {
        pauseMenu.SetActive(true);
        _baseRunner.Pause();
        LevelLoader.instance.PauseLevel();

    }

    public void HidePause()
    {
        pauseMenu.SetActive(false);
    }

    public void ShowFinish(int score)
    {
        finish_scoreText.text = "Score: " + score;
        finishMenu.SetActive(true);
        setNewHighScore(score);
        _baseRunner.Pause();
    }
    public void HideFinish() {
        finishMenu.SetActive(false);
    }

    public void OnResumeButtonClicked()
    {
        HidePause();
        _baseRunner.Resume();
        LevelLoader.instance.ResumeLevel();

    }
    public void OnRestartButtonClicked() {

        HidePause();
        HideFinish();
        LevelLoader.instance.RestartLevel();
        _baseRunner.Resume();
    }
    public void OnQuitButtonClicked()
    {
        HidePause();
        HideFinish();
        LevelLoader.instance.QuitLevel();
    }


    public void setNewHighScore(int score)
    {
        var id = LevelManager.currentLevel;

        var save = SaveManager.Load<LevelSaveDataContainer>();
        var levelData = save.saveData.levels.Find(x => x.levelId == id);

        if (levelData == null)
        {
            SaveManager.SaveLevel(id, score);
        }
        else
        {
            finish_maxScoreText.text = $"maxScore: {levelData.maxScore}";
            if (levelData.maxScore < score)
            {
                SaveManager.SaveLevel(id, score);
            }
        }

    }

}
