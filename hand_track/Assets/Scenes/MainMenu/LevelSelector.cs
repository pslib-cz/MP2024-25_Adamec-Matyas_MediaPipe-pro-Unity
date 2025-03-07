using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    public static LevelSelector instance;

    public LevelData[] levels;
    public Transform buttonContainer; 
    public GameObject buttonPrefab; 
    public Button submitButton;

    public AudioSource audioSource; 

    private Button currentlySelectedButton = null; 
    private int selectedLevel = -1;

    public GameObject LevelListParent;
    public GameObject LevelDetail;
    public TextMeshProUGUI LevelDetail_heading;
    public TextMeshProUGUI LevelDetail_maxScore;
    public TextMeshProUGUI LevelDetail_blocks;
    public TextMeshProUGUI LevelDetail_time;
    public TextMeshProUGUI LevelDetail_Description;

    private LevelSaveData[] levelSaveDatas;

    public Color standardItemColor = new Color(1, 1, 1, 1);
    public Color selectedItemColor = new Color(1, 1, 1, 1);

    public Image selectedImage;
    public Image percentImage;
    public TextMeshProUGUI percentText;

    public void SaveData()
    {
        LevelSaveDataContainer saveContainer = new LevelSaveDataContainer();

        foreach (LevelData level in levels)
        {
            Debug.Log($"Filling Level: {level.levelId}");
            LevelSaveData levelSaveData = new LevelSaveData
            {
                levelId = level.levelId,
                maxScore = -1
            };
            saveContainer.levels.Add(levelSaveData);
        }

        SaveManager.Save(new Save<LevelSaveDataContainer>("levelData", saveContainer));
    }


    public void LoadData()
    {
        LevelSaveDataContainer saveContainer = SaveManager.Load<LevelSaveDataContainer>().saveData as LevelSaveDataContainer;
        Debug.Log(saveContainer);

        Debug.Log($"Loaded Level Data: {saveContainer.levels.Count}");

        levelSaveDatas = saveContainer.levels.ToArray();

    }


    private void Awake()
    {
        makeListBig();
        LoadData();
        instance = this;
        Debug.Log($"Filling Levels: ");

        foreach (LevelData level in levels)
        {
            GameObject button = Instantiate(buttonPrefab, buttonContainer);
            button.GetComponentInChildren<TMP_Text>().text = level.levelName;

            button.GetComponentsInChildren<Image>()[2].sprite = level.levelIcon;


            Button btnComponent = button.GetComponent<Button>();
            btnComponent.onClick.AddListener(() => OnLevelSelected(btnComponent, level));
        }

        submitButton.onClick.AddListener(OnSubmit);
    }

    public void selectLevel(int level)
    {
        Debug.Log("Selecting level: " + level);
        for (int i = 0; i < buttonContainer.childCount; i++)
        {
            Button button = buttonContainer.GetChild(i).GetComponent<Button>();
            LevelData levelData = levels[i];
            if (levelData.levelId == level)
            {
                OnLevelSelected(button, levelData);
                return;
            }
        }

    }

    private void OnLevelSelected(Button button, LevelData level)
    {
        if (currentlySelectedButton != null)
        {
            currentlySelectedButton.GetComponent<Image>().color = standardItemColor;
        }

        currentlySelectedButton = button;
        button.GetComponent<Image>().color = selectedItemColor;

        audioSource.clip = level.levelMusic; 
        audioSource.time = audioSource.clip.length / 2;
        audioSource.Play(); 
        audioSource.loop = true; 

        selectedLevel = level.levelId;
        Debug.Log($"Selected Level: {selectedLevel}");

        ShowDetails(level);
    }

    float listSmall = 425.52f;
    float listBig = 1064.8f;

    private void makeListBig()
    {
        StartCoroutine(LerpWidth(LevelListParent.GetComponent<RectTransform>(), listSmall, listBig, 0.5f));
    }
    private void makeListSmall()
    {
        StartCoroutine(LerpWidth(LevelListParent.GetComponent<RectTransform>(), listBig, listSmall, 0.5f));
    }

    private IEnumerator LerpWidth(RectTransform rectTransform, float startWidth, float endWidth, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float newWidth = Mathf.Lerp(startWidth, endWidth, elapsed / duration);
            rectTransform.sizeDelta = new Vector2(newWidth, rectTransform.sizeDelta.y);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rectTransform.sizeDelta = new Vector2(endWidth, rectTransform.sizeDelta.y);
    }

    private void ShowDetails(LevelData level)
    {
        if (LevelDetail.activeInHierarchy == false)
        { 
            makeListSmall();
            LevelDetail.SetActive(true);
        }

        string configText = level.jsonConfig.text;
        try
        {
            selectedImage.sprite = level.levelIcon;

            
            LevelJson levelJsonData = JsonUtility.FromJson<LevelJson>(configText);
            LevelDetail_heading.text = levelJsonData.name;
            LevelDetail_blocks.text = $"{levelJsonData.blocks.Length} Blocks";

            float totalSeconds = levelJsonData.Time;
            float minutes = (int)totalSeconds / 60;
            float seconds = totalSeconds % 60;
            LevelDetail_time.text = $"Time: {minutes}:{seconds}"; LevelDetail_Description.text = levelJsonData.description;

            LevelDetail_maxScore.text = $"0 / {levelJsonData.blocks.Length*100}";
            percentText.text = $"0%";
            percentImage.color = new Color(1, 0, 0, 1);
            for (int i = 0; i < levelSaveDatas.Length; i++)
            {
                if (levelSaveDatas[i].levelId == levelJsonData.id)
                {
                    var maxScore = levelSaveDatas[i].maxScore;
                    LevelDetail_maxScore.text = $"{maxScore} / {levelJsonData.blocks.Length*100}";

                    float percent = (float)maxScore / (levelJsonData.blocks.Length * 100) * 100;
                    percentText.text = $"{Mathf.RoundToInt(percent)}%";
                    Debug.Log($"{percent}%  - {maxScore} - {(levelJsonData.blocks.Length * 100)}");
                    if (maxScore < (levelJsonData.blocks.Length * 100) * 0.25)
                    {
                        percentImage.color = new Color(1, 0, 0, 1);
                    }                    
                    else if (maxScore > (levelJsonData.blocks.Length * 100) * 0.25)
                    {
                        percentImage.color = new Color(0, 1, 0, 1);
                    }
                    return;
                }
            }


        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error parsing JSON: {ex.Message}");
        }


    }

    private void OnSubmit()
    {
        if (selectedLevel != -1)
        {
            Debug.Log($"Submitted Level ID: {selectedLevel}");
            LevelManager.currentLevel = selectedLevel;
            SceneManager.LoadScene("Game");
        }
        else
        {
            Debug.Log("No level selected!");
        }

        if (currentlySelectedButton != null)
        {
            currentlySelectedButton.GetComponent<Image>().color = standardItemColor; // Reset color
        }

        currentlySelectedButton = null;
        selectedLevel = -1;
    }

    public void deselectAll()
    {
        if (currentlySelectedButton != null)
        {
            currentlySelectedButton.GetComponent<Image>().color = standardItemColor; // Reset color
        }

        currentlySelectedButton = null;
        selectedLevel = -1;
        audioSource.Stop();
        LevelDetail.SetActive(false);
        makeListBig();
    }
}

