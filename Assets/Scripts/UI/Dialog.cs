using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AvatarViewer.UI
{
    public class Dialog : MonoBehaviour
    {
        [SerializeField] private TMP_Text Title;
        [SerializeField] private TMP_Text Content;
        [SerializeField] private Button Ok;
        [SerializeField] private Button Cancel;
        [SerializeField] private RectTransform dialogContent;

        private CanvasGroup canvasGroup;
        private Sequence sequence;

        private void Awake()
        {
            if (Ok != null)
                Ok.onClick.AddListener(() => Hide());
            if (Cancel != null)
                Cancel.onClick.AddListener(() => Hide());

            canvasGroup = GetComponent<CanvasGroup>();
            dialogContent = GetComponent<RectTransform>();

            canvasGroup.alpha = 0;
        }

        private void Start()
        {
            sequence = DOTween.Sequence();

            dialogContent.localScale = new Vector3(1.05f, 1.05f, 1.05f);
            sequence.Insert(0, canvasGroup.DOFade(1, 0.3f).SetEase(Ease.OutExpo));
            sequence.Insert(0, dialogContent.DOScale(1, 0.3f).SetEase(Ease.OutExpo));
        }

        public void SetTitle(string title)
        {
            Title.text = title;
        }

        public void SetContent(string content)
        {
            Content.text = content;
        }

        public void SetOnOkAction(UnityAction action)
        {
            if (action != null)
                Ok.onClick.AddListener(action);
        }

        public void SetOnCancelAction(UnityAction action)
        {
            if (action != null)
                Cancel.onClick.AddListener(action);
        }

        private void Hide()
        {
            sequence = DOTween.Sequence();

            sequence.Insert(0, canvasGroup.DOFade(0, 0.3f).SetEase(Ease.OutExpo));
            sequence.Insert(0, dialogContent.DOScale(1.05f, 0.3f).SetEase(Ease.OutExpo));

            HideAsync().Forget();
        }

        private async UniTaskVoid HideAsync()
        {
            await sequence.ToUniTask();
            Destroy(gameObject);
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
