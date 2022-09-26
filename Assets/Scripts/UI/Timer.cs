using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textUI;
    [SerializeField] private int maxTimeInSeconds = 180;

    public UnityEvent onTimerFinishCallback;

    private float _currentRemainingTime;

    public int GetScore()
    {
        return maxTimeInSeconds - (int)_currentRemainingTime;
    }

    private void Start()
    {
        _currentRemainingTime = maxTimeInSeconds;
    }

    private void Update()
    {
        if (_currentRemainingTime > 0f)
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
}
