using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bestScoreText;

    private void Start()
    {
        UpdateBestScoreText();
    }

    public void UpdateBestScoreText()
    {
        float bestScore = GameManager.Instance.GetBestScore();
        int minutes = (int)((bestScore) / 60f);
        int seconds = (int)(bestScore - minutes * 60f);
        bestScoreText.text = minutes.ToString() + "'" + seconds.ToString() + "\"";
    }
}
