using DG.Tweening;
using UnityEngine;

public class PanelSlide : BaseAnimation
{

    public override Sequence StartAnimation()
    {
        sequence = DOTween.Sequence();
        switch (Easing)
        {
            case AnimationEasing.EaseIn:
                rectTransform.anchoredPosition = new Vector2(rectTransform.rect.width, rectTransform.anchoredPosition.y);
                sequence.Insert(0, rectTransform.DOAnchorPosX(0, 0.25f).SetEase(Ease.OutExpo));
                break;
            case AnimationEasing.EaseOut:
                sequence.Insert(0, rectTransform.DOAnchorPosX(rectTransform.rect.width, 0.25f).SetEase(Ease.OutExpo));
                break;
        }

        return sequence;
    }
}
