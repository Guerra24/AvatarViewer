using DG.Tweening;
using UnityEngine;

namespace AvatarViewer.Ui.Animations
{
    public class SpinnerAnimation : MonoBehaviour
    {
        public bool AutoStart = true;

        [SerializeField]
        private RectTransform UpperImage;
        [SerializeField]
        private RectTransform LowerImage;

        private RectTransform RectTransform;

        private Sequence sequence;
        private Tween rotation;

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            if (AutoStart)
                StartAnimation();
        }

        public void StartAnimation()
        {
            sequence = DOTween.Sequence();
            sequence.Insert(0, UpperImage.DOLocalRotate(new Vector3(0, 0, -75), 1.2f).SetEase(Ease.InOutCubic));
            sequence.Insert(0, LowerImage.DOLocalRotate(new Vector3(0, 0, 75), 1.2f).SetEase(Ease.InOutCubic));
            sequence.Insert(1.2f, UpperImage.DOLocalRotate(new Vector3(0, 0, 0), 1.2f).SetEase(Ease.InOutCubic));
            sequence.Insert(1.2f, LowerImage.DOLocalRotate(new Vector3(0, 0, 0), 1.2f).SetEase(Ease.InOutCubic));
            sequence.SetLoops(-1, LoopType.Restart);
            rotation = RectTransform.DOLocalRotate(new Vector3(0, 0, -360), 0.65f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        }

        public void StopAnimation()
        {
            sequence.Kill();
            rotation.Kill();
        }

        private void OnDestroy() => StopAnimation();

        private void OnApplicationQuit() => StopAnimation();
    }
}
