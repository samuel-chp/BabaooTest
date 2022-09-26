using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bestScoreText;

    private void Start()
    {
        PrintBestScore();
    }

    public void Play()
    {
        GameManager.Instance.GoToGame();
    }

    public void Exit()
    {
        GameManager.Instance.CloseGame();
    }

    public void PrintBestScore()
    {
        int bestScore = PlayerPrefs.GetInt(GameManager.BEST_SCORE_KEY, 3600);
        int minutes = (int)(bestScore / 60);
        int seconds = (int)(bestScore - 60f * minutes);
        string newText;
        if (minutes > 0)
        {
            newText = string.Format("{0}\'{1}", minutes.ToString(), seconds.ToString());
        }
        else
        {
            newText = string.Format("{0}\"", seconds.ToString());
        }
        bestScoreText.text = newText;
    }
}
