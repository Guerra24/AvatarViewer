using DG.Tweening;
using UnityEngine;

namespace AvatarViewer.Ui.Animations
{
    public class PageSlide : BaseAnimation
    {
        public PageSlideMode Mode;

        public override Sequence StartAnimation()
        {
            sequence = DOTween.Sequence();
            switch (Mode)
            {
                case PageSlideMode.Left:
                    if (Easing == AnimationEasing.EaseIn)
                        rectTransform.anchoredPosition = new Vector2(120, rectTransform.anchoredPosition.y);
                    sequence.Insert(0, rectTransform.DOAnchorPosX(Easing == AnimationEasing.EaseIn ? 0 : 120, 0.25f));
                    break;
                case PageSlideMode.Right:
                    if (Easing == AnimationEasing.EaseIn)
                        rectTransform.anchoredPosition = new Vector2(-120, rectTransform.anchoredPosition.y);
                    sequence.Insert(0, rectTransform.DOAnchorPosX(Easing == AnimationEasing.EaseIn ? 0 : -120, 0.25f));
                    break;
            }
            sequence.SetEase(Easing == AnimationEasing.EaseIn ? Ease.OutExpo : Ease.InExpo);
            return sequence;
        }

    }

    public enum PageSlideMode
    {
        Left, Right
    }

}
