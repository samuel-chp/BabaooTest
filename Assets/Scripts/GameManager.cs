using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private ScoreHolder scoreHolder;
    
    public float maxGameDuration = 180f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public float GetBestScore()
    {
        return scoreHolder.bestScore;
    }

    public void SaveScore(float score)
    {
        scoreHolder.bestScore = score;
    }
}
