using Cysharp.Threading.Tasks;
using DG.Tweening;
using LogicUI.FancyTextRendering;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarViewer.Ui
{
    public class MarkdownDialog : MonoBehaviour
    {
        [SerializeField] private MarkdownRenderer MarkdownRenderer;
        [SerializeField] private Button Ok;

        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;
        private Sequence sequence;

        private void Awake()
        {
            canvasGroup = GetComponentInParent<CanvasGroup>();
            rectTransform = GetComponent<RectTransform>();

            canvasGroup.alpha = 0;

            Ok.onClick.AddListener(() => Hide());
        }

        private void Start()
        {
            sequence = DOTween.Sequence();

            rectTransform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
            sequence.Insert(0, canvasGroup.DOFade(1, 0.3f).SetEase(Ease.OutExpo));
            sequence.Insert(0, rectTransform.DOScale(1, 0.3f).SetEase(Ease.OutExpo));
        }

        public void SetContent(string content)
        {
            MarkdownRenderer.Source = content;
        }

        private void Hide()
        {
            sequence = DOTween.Sequence();

            sequence.Insert(0, canvasGroup.DOFade(0, 0.3f).SetEase(Ease.OutExpo));
            sequence.Insert(0, rectTransform.DOScale(1.05f, 0.3f).SetEase(Ease.OutExpo));

            HideAsync().Forget();
        }

        private async UniTaskVoid HideAsync()
        {
            await sequence.ToUniTask();
            Destroy(gameObject.transform.parent.gameObject);
        }

        private void OnDestroy()
        {
            sequence.Kill();
        }

        private void OnApplicationQuit()
        {
            sequence.Kill();
        }
    }
}
