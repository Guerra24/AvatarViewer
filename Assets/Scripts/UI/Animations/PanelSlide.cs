using DG.Tweening;
using UnityEngine;

namespace AvatarViewer.UI.Animations
{
    public class PanelSlide : BaseAnimation
    {

        public PanelSlideMode Mode = PanelSlideMode.Left;

        public override Sequence StartAnimation()
        {
            sequence = DOTween.Sequence();
            switch (Easing)
            {
                case AnimationEasing.EaseIn:
                    rectTransform.anchoredPosition = new Vector2(Mode == PanelSlideMode.Right ? rectTransform.rect.width : -rectTransform.rect.width, rectTransform.anchoredPosition.y);
                    sequence.Insert(0, rectTransform.DOAnchorPosX(0, 0.25f).SetEase(Ease.OutExpo));
                    break;
                case AnimationEasing.EaseOut:
                    sequence.Insert(0, rectTransform.DOAnchorPosX(Mode == PanelSlideMode.Right ? rectTransform.rect.width : -rectTransform.rect.width, 0.25f).SetEase(Ease.OutExpo));
                    break;
            }

            return sequence;
        }
    }

    public enum PanelSlideMode
    {
        Left, Right
    }
}
