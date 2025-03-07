using RealSenseManager;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader instance;

    public LevelData[] levels; 
    public AudioSource audioSource;
    
    private LevelData currentLevelData;
    private Block[] blocks;

    private float timer = 0f;
    private int currentBlockIndex = 0; 

    private float levelTime;

    public GameObject countDown;

    public TMPro.TextMeshProUGUI timerText;
    public TMPro.TextMeshProUGUI scoreText;

    public bool isPaused = false;

    public int score = 0;


    private void Awake()
    {

        instance = this;

        timer = 0f;
        score = 0;
        var currentLevelId = LevelManager.currentLevel;
        if (currentLevelId == -1)
        {
            Debug.LogError("No level selected");
            currentLevelId = 0;
        }
        loadLevel(currentLevelId);
        loadBlocks();
        audioSource.clip = currentLevelData.levelMusic;
        Play();

    }

    public void addScore(int points)
    {
        score += points;
        scoreText.text = $"Score: {score}";
    }

    public void PauseLevel()
    {
        isPaused = true;
        audioSource.Pause();
        //GameUiContorller.instance.ShowPause();
    }

    public void ResumeLevel()
    {
        isPaused = false;
        audioSource.UnPause();
    }

    public void RestartLevel()
    {
        score = 0;
        timer = 0f;
        timerText.text = "";
        currentBlockIndex = 0;
        audioSource.Stop();
        BlockSpawner.instance.deleteAllBlocks();
        Play();
        
    }

    public void QuitLevel()
    {
        score = 0;
        timer = 0f;
        timerText.text = "";
        currentBlockIndex = 0;
        audioSource.Stop();
        LevelManager.isGameStarted = false;
        IRealSenseManager realsense = RealSenseManagerFactory.GetManager();
        //RealSenseManagerPc realsense = RealSenseManagerPc.Instance;
        realsense.Dispose();
        SceneManager.LoadScene("MainMenu");
    }

    public async void Play()
    {
        await StartCountdownAsync();

        audioSource.Play();
        LevelManager.isGameStarted = true;
        isPaused = false;
    }

    private async Task StartCountdownAsync()
    {
        countDown.SetActive(true);

        var textComponent = countDown.GetComponentInChildren<TextMeshProUGUI>();

        string[] countdownTexts = { "3", "2", "1", "Go" };

        foreach (string text in countdownTexts)
        {
            textComponent.text = text;
            await Task.Delay(text == "Go" ? 500 : 1000); // Shorter delay for "Go"
        }

        countDown.SetActive(false);
    }


    void loadLevel(int id)
    {
        // go forach levels to find levelId inside LevelData
        foreach (LevelData level in levels)
        {
            if (level.levelId == id)
            {
                //Debug.Log("Loading level: " + level.levelName);
                currentLevelData = level;
                break;
            }
        }
    }

    void loadBlocks()
    {
        string configText = currentLevelData.jsonConfig.text;
        try
        {
            LevelJson levelJsonData = JsonUtility.FromJson<LevelJson>(configText);
            blocks = levelJsonData.blocks;
            levelTime = levelJsonData.Time;
            Debug.Log($"Blocks loaded: {blocks.Length}");

        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error parsing JSON: {ex.Message}");
        }
    }
    private void Update()
    {
        if (!LevelManager.isGameStarted) return;
        if (isPaused) return;

        timer += Time.deltaTime;
        timerText.text = timer.ToString("F2");

        if (timer >= levelTime)
        {
            Debug.Log("Level completed");
            LevelManager.isGameStarted = false;
            audioSource.Stop();
            GameUiContorller.instance.ShowFinish(score);
        }

        if (currentBlockIndex < blocks.Length)
        {
            Block currentBlock = blocks[currentBlockIndex];

            if (timer >= currentBlock.time - 4.75f) // block speed 10f, block spawn distance 50f-2f
            {
                if (currentBlock.type == "long")
                {
                    //Debug.Log($"Spawned long block ID: {currentBlock.id} at ({currentBlock.position.x}, {currentBlock.position.y})");
                    BlockSpawner.instance.spawnLongBlock(currentBlock.position.x, currentBlock.position.y, currentBlock.isRight, currentBlock.points, currentBlock.longData);
                }
                else
                {
                    BlockSpawner.instance.spawnBlock(currentBlock.position.x, currentBlock.position.y, currentBlock.isRight, currentBlock.points);
                }
                //Debug.Log($"Spawned block ID: {currentBlock.id} at ({currentBlock.position.x}, {currentBlock.position.y})");

                currentBlockIndex++;
            }
        }
    }

    private void OnApplicationQuit()
    {
        IRealSenseManager realsense = RealSenseManagerFactory.GetManager();
        //RealSenseManagerPc realsense = RealSenseManagerPc.Instance;
        realsense.Dispose();
    }
    
    public void playSound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip, 0.2f);
    }

}
