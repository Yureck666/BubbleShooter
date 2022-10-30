using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameModeSelection : MonoBehaviour
{
    [Serializable]
    private class GameMode
    {
        public string gameModeName;
        public int sceneIndex;
    }

    [SerializeField] private GameMode[] _gameModes;
    [SerializeField] private Button startButton;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private TMP_Text modeText;

    private int _gameModeIndex;

    private void Awake()
    {
        startButton.onClick.AddListener(StartGame);
        leftButton.onClick.AddListener(() => SwitchMode(-1));
        rightButton.onClick.AddListener(() => SwitchMode(1));
        RefreshModeText();
    }

    private void StartGame()
    {
        SceneManager.LoadScene(_gameModes[_gameModeIndex].sceneIndex);
    }

    private void SwitchMode(int offset)
    {
        _gameModeIndex += offset;
        _gameModeIndex = Math.Clamp(_gameModeIndex, 0, _gameModes.Length - 1);
        RefreshModeText();
    }

    private void RefreshModeText()
    {
        modeText.text = _gameModes[_gameModeIndex].gameModeName;
    }
}
