using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUi : MonoBehaviour
{
    [SerializeField] private Button[] homeButtons;
    [SerializeField] private Button[] restartButtons;
    [SerializeField] private Button[] pauseButtons;
    [SerializeField] private CanvasGroup winPanel;
    [SerializeField] private CanvasGroup pausePanel;
    [SerializeField] private int homeSceneIndex;
    [SerializeField] private float fadeTime;

    private Tween _pausePanelTween;
    private void Awake()
    {
        foreach (var button in homeButtons)
        {
            button.onClick.AddListener(LoadHomeScene);
        }

        foreach (var button in restartButtons)
        {
            button.onClick.AddListener(RestartLevel);
        }

        foreach (var button in pauseButtons)
        {
            button.onClick.AddListener(SwitchPausePanelActive);
        }
        
        winPanel.alpha = 0;
        winPanel.blocksRaycasts = false;
        
        pausePanel.alpha = 0;
        pausePanel.blocksRaycasts = false;
    }

    public void SetWinPanelActive(bool active)
    {
        winPanel.DOFade(active ? 1 : 0, fadeTime);
        winPanel.blocksRaycasts = active;
    }
    
    private void SwitchPausePanelActive()
    {
        pausePanel.blocksRaycasts = !pausePanel.blocksRaycasts;
        _pausePanelTween?.Kill();
        _pausePanelTween = pausePanel.DOFade(pausePanel.blocksRaycasts ? 1 : 0, fadeTime);
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadHomeScene()
    {
        SceneManager.LoadScene(homeSceneIndex);
    }
}
