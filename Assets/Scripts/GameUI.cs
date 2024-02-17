using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI flagLeft;
    public TextMeshProUGUI scoreTextWin;
    public TextMeshProUGUI scoreTextLose;
    public GameObject winPanel;
    public GameObject losePanel;

    private GameplayManager gameplayManager;

    public event Action onNewGameClicked;

    public void Init(GameplayManager gameplayManager)
    {
        this.gameplayManager = gameplayManager;

        this.UpdateFlagLeft();
        this.winPanel.SetActive(false);
        this.losePanel.SetActive(false);
    }

    public void UpdateFlagLeft()
    {
        this.flagLeft.text = this.gameplayManager.FlagLeft.ToString();
    }

    public void ShowLose(int score)
    {
        this.scoreTextLose.text = $"{score}";
        this.losePanel.SetActive(true);
    }

    public void ShowWin(int score)
    {
        this.scoreTextWin.text = $"{score}";
        this.winPanel.SetActive(true);
    }

    public void OnNewGameButtonClicked()
    {
        this.onNewGameClicked?.Invoke();
    }
}
