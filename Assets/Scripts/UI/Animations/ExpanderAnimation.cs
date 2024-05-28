using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarViewer.UI.Animations
{
    public class ExpanderAnimation : BaseAnimation
    {
        private RectMask2D Mask;

        protected override void Awake()
        {
            base.Awake();
            Mask = GetComponent<RectMask2D>();
        }

        public override Sequence StartAnimation()
        {
            sequence = DOTween.Sequence();
            switch (Easing)
            {
                case AnimationEasing.EaseIn:
                    rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 47);
                    Mask.padding = new Vector4(0, 0, 0, 100);
                    sequence.Insert(0, rectTransform.DOAnchorPosY(-53, 0.3f));
                    sequence.Insert(0, DOTween.To(() => Mask.padding, (x) => Mask.padding = x, new Vector4(0, 0, 0, 0), 0.3f));
                    sequence.SetEase(Ease.OutCubic);
                    break;
                case AnimationEasing.EaseOut:
                    sequence.Insert(0, rectTransform.DOAnchorPosY(47, 0.3f));
                    sequence.Insert(0, DOTween.To(() => Mask.padding, (x) => Mask.padding = x, new Vector4(0, 0, 0, 100), 0.3f));
                    sequence.SetEase(Ease.InExpo);
                    break;
            }
            return sequence;
        }
    }
}
