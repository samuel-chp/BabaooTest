using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textUI;
    [SerializeField] private float minRemainingTimeAnimation = 5f;
    
    private Tweener _tweener;
    private bool _tweenPlaying;
    
    private float _elapsedTime;
    private float MaxGameDuration => GameManager.Instance.maxGameDuration;
    
    public float RemainingTime => MaxGameDuration - _elapsedTime;

    public float ElapsedTime => _elapsedTime;

    private void Awake()
    {
        _tweener = GetComponent<Tweener>();
    }

    void Update()
    {
        _elapsedTime += Time.deltaTime;
        
        UpdateText();

        if (RemainingTime < minRemainingTimeAnimation && _tweenPlaying == false)
        {
            _tweener.Show();
            _tweenPlaying = true;
        }
        
        if (_elapsedTime > MaxGameDuration)
        {
            StartCoroutine(FindObjectOfType<PuzzleManager>().Lose());
            gameObject.SetActive(false);
        }
    }

    private void UpdateText()
    {
        int minutes = (int)((RemainingTime) / 60f);
        int seconds = (int)(RemainingTime - minutes * 60f);
        textUI.text = minutes.ToString() + "'" + seconds.ToString() + "\"";
    }
}
