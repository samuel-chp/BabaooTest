using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textUI;

    public UnityEvent onTimerFinishCallback;

    private float _currentRemainingTime;
    private bool _play;

    public int maxTimeInSeconds { get; set; }
    
    public int GetScore()
    {
        return maxTimeInSeconds - (int)_currentRemainingTime;
    }

    private void Update()
    {
        if (_currentRemainingTime > 0f && _play)
        {
            _currentRemainingTime -= Time.deltaTime;
            
            PrintRemainingTime();
            
            if (_currentRemainingTime <= 0f)
            {
                onTimerFinishCallback?.Invoke();
            }
        }
    }

    private void PrintRemainingTime()
    {
        int minutes = (int)(_currentRemainingTime / 60);
        int seconds = (int)(_currentRemainingTime - 60f * minutes);
        string newText;
        if (minutes > 0)
        {
            newText = string.Format("{0}\'{1}", minutes.ToString(), seconds.ToString());
        }
        else
        {
            newText = string.Format("{0}\"", seconds.ToString());
        }
        
        textUI.text = newText;
    }

    public void PauseTimer()
    {
        _play = false;
    }

    public void UnpauseTimer()
    {
        _play = true;
    }

    public void ResetTimer()
    {
        _currentRemainingTime = maxTimeInSeconds;
    }
}
