using Mediapipe.Unity.Sample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SandBoxUiContorller : MonoBehaviour
{
    private BaseRunner _baseRunner;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button pause_resumeButton;
    [SerializeField] private Button pause_quitButton;

    [SerializeField] private Button dev_PauseBtn;


    private void Awake()
    {
        Debug.Log("Awake");
        _baseRunner = GameObject.FindObjectOfType<BaseRunner>();

        if (_baseRunner == null)
        {
            Debug.LogError("BaseRunner is not found in the scene. Please ensure it is added.");
        }

        pause_resumeButton.onClick.AddListener(OnResumeButtonClicked);
        pause_quitButton.onClick.AddListener(OnQuitButtonClicked);


        dev_PauseBtn.onClick.AddListener(ShowPause);
        Debug.Log("Awake2");
    }

    public void Start()
    {
        _baseRunner.Play();
    }

    public void ShowPause()
    {
        Debug.Log("Show Pause");
        pauseMenu.SetActive(true);
        _baseRunner.Pause();
    }

    public void HidePause()
    {
        pauseMenu.SetActive(false);
    }

    public void OnResumeButtonClicked()
    {
        HidePause();
        _baseRunner.Resume();
    }

    public void OnQuitButtonClicked()
    {
        HidePause();
        RealSenseManager.RealSenseManagerFactory.GetManager().Dispose();
        SceneManager.LoadScene("MainMenu");
    }


}
