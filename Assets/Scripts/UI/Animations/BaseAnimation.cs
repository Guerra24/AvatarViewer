using DG.Tweening;
using UnityEngine;

public abstract class BaseAnimation : MonoBehaviour
{
    public AnimationEasing Easing;
    public bool AutoStart = true;

    protected RectTransform rectTransform;
    protected Sequence sequence;

    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    protected virtual void Start()
    {
        if (AutoStart)
        {
            AutoStart = false;
            StartAnimation();
        }
    }

    protected virtual void Update()
    {
        if (AutoStart)
        {
            AutoStart = false;
            StartAnimation();
        }
    }

    public abstract Sequence StartAnimation();

    protected virtual void OnDestroy()
    {
        sequence.Kill();
    }

    protected virtual void OnApplicationQuit()
    {
        sequence.Kill();
    }
}

public enum AnimationEasing
{
    EaseIn, EaseOut
}