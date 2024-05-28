using DG.Tweening;
using UnityEngine;

namespace AvatarViewer.UI.Animations
{
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
                    sequence.Insert(0, canvasGroup.DOFade(1, 0.3f).SetEase(Ease.OutExpo));
                    sequence.Insert(0, rectTransform.DOScale(1, 0.3f).SetEase(Ease.OutExpo));
                    break;
                case AnimationEasing.EaseOut:
                    sequence.Insert(0, canvasGroup.DOFade(0, 0.15f).SetEase(Ease.OutExpo));
                    sequence.Insert(0, rectTransform.DOScale(1.05f, 0.15f).SetEase(Ease.OutExpo));
                    break;
            }
            //sequence.SetEase(Ease.OutExpo);
            return sequence;
        }

    }
}
