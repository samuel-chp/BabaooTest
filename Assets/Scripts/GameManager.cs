using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public const string BEST_SCORE_KEY = "BestScore";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
    }

    public static void GoToGame()
    {
        SceneManager.LoadScene(1);
    }

    public static void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public static void CloseGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    public static void SaveBestScore(int scoreInSeconds)
    {
        int currentBest = PlayerPrefs.GetInt(BEST_SCORE_KEY, 3600);
        if (scoreInSeconds < currentBest)
        {
            PlayerPrefs.SetInt(BEST_SCORE_KEY, scoreInSeconds);
        }
    }

#if UNITY_EDITOR
    [MenuItem("Tools/GameManager/ResetScore")]
    public static void ResetBestScore()
    {
        PlayerPrefs.DeleteKey(BEST_SCORE_KEY);
    }
#endif
}
