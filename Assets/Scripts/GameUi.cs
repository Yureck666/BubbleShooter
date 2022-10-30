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
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private int homeSceneIndex;
    [SerializeField] private float fadeTime;

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
        
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }

    public void SetActive(bool active)
    {
        canvasGroup.DOFade(active ? 1 : 0, fadeTime);
        canvasGroup.blocksRaycasts = active;
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
