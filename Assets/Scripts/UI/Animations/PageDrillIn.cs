using DG.Tweening;
using UnityEngine;

public class PageDrillIn : BaseAnimation
{
    private CanvasGroup canvasGroup;

    protected override void Awake()
    {
        base.Awake();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public override Sequence StartAnimation()
    {
        sequence = DOTween.Sequence();
        switch (Easing)
        {
            case AnimationEasing.EaseIn:
                canvasGroup.alpha = 0.0f;
                rectTransform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
                sequence.Insert(0, canvasGroup.DOFade(1, 0.4f));
                sequence.Insert(0, rectTransform.DOScale(1, 0.4f));
                break;
            case AnimationEasing.EaseOut:
                sequence.Insert(0, canvasGroup.DOFade(0, 0.3f));
                sequence.Insert(0, rectTransform.DOScale(1.1f, 0.3f));
                break;
        }
        sequence.SetEase(Ease.OutExpo);
        return sequence;
    }

}
