using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tweener : MonoBehaviour
{
    public enum UIAnimationTypes
    {
        Move,
        Scale,
        Fade
    }

    [Tooltip("Describe what this tweener does (optionnal).")]
    [TextArea]
    public string description;
    
    public GameObject objectToAnimate;
    
    public UIAnimationTypes animationType;
    public LeanTweenType easeType;
    public float duration;
    public float delay;

    public int loops = 1;
    public bool pingpong;
    public bool loopClamp;

    [Tooltip("If the animation type is 'scale' or 'fade', begin the animation from the value 'from'.")]
    public bool startPositionOffset;
    public Vector3 from;
    public Vector3 to;

    public bool showOnEnable = true;
    public UnityEvent onCompleteCallback;

    private LTDescr _tweenObject;

    private void OnEnable()
    {
        if (showOnEnable)
        {
            Show();
        }
    }

    [ContextMenu("Show")]
    public void Show()
    {
        // Cancel previous tweens
        if (_tweenObject != null)
        {
            Cancel();
        }

        HandleTween();
    }

    public void HandleTween()
    {
        if (objectToAnimate == null)
        {
            objectToAnimate = gameObject;
        }

        switch (animationType)
        {
            case UIAnimationTypes.Move:
                Move();
                break;
            case UIAnimationTypes.Fade:
                Fade();
                break;
            case UIAnimationTypes.Scale:
                Scale();
                break;
        }

        _tweenObject.setDelay(delay);
        _tweenObject.setEase(easeType);
        _tweenObject.setOnComplete(OnComplete);
        _tweenObject.setLoopCount(loops);

        if (loopClamp)
        {
            _tweenObject.setLoopClamp();
        }

        if (pingpong)
        {
            _tweenObject.setLoopPingPong();
        }
    }

    public void Move()
    {
        objectToAnimate.GetComponent<RectTransform>().anchoredPosition = from;
        
        _tweenObject = LeanTween.move(objectToAnimate.GetComponent<RectTransform>(), to, duration);
    }

    public void Fade()
    {
        if (gameObject.GetComponent<CanvasGroup>() == null)
        {
            gameObject.AddComponent<CanvasGroup>();
        }

        if (startPositionOffset)
        {
            objectToAnimate.GetComponent<CanvasGroup>().alpha = from.x;
        }

        _tweenObject = LeanTween.alphaCanvas(objectToAnimate.GetComponent<CanvasGroup>(), to.x, duration);
    }

    public void Scale()
    {
        if (startPositionOffset)
        {
            objectToAnimate.GetComponent<RectTransform>().localScale = from;
        }

        _tweenObject = LeanTween.scale(objectToAnimate.GetComponent<RectTransform>(), to, duration);
    }

    [ContextMenu("Pause")]
    public void Pause()
    {
        _tweenObject?.pause();
    }
    
    [ContextMenu("Resume")]
    public void Resume()
    {
        _tweenObject?.resume();
    }

    [ContextMenu("Cancel")]
    public void Cancel()
    {
        if (_tweenObject != null)
        {
            if (startPositionOffset)
            {
                switch (animationType)
                {
                    case UIAnimationTypes.Fade:
                        objectToAnimate.GetComponent<CanvasGroup>().alpha = from.x;
                        break;
                    case UIAnimationTypes.Scale:
                        objectToAnimate.GetComponent<RectTransform>().localScale = from;
                        break;
                    case UIAnimationTypes.Move:
                        objectToAnimate.GetComponent<RectTransform>().anchoredPosition = from;
                        break;
                }
            }
            
            LeanTween.cancel(_tweenObject.id);
        }
    }

    public void OnComplete()
    {
        onCompleteCallback?.Invoke();
    }
}
