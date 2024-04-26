using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    private TMP_Text Title;
    private TMP_Text Content;
    private Button Ok;
    private Button Cancel;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Sequence sequence;

    private void Awake()
    {
        Title = transform.Find("Title").GetComponent<TMP_Text>();
        Content = transform.Find("Content").GetComponent<TMP_Text>();
        Ok = transform.Find("Actions/Ok").GetComponent<Button>();
        Cancel = transform.Find("Actions/Cancel").GetComponent<Button>();

        Ok.onClick.AddListener(() => Hide());
        Cancel.onClick.AddListener(() => Hide());

        canvasGroup = GetComponentInParent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        canvasGroup.alpha = 0;
    }

    private void Start()
    {
        sequence = DOTween.Sequence();

        rectTransform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
        sequence.Insert(0, canvasGroup.DOFade(1, 0.3f).SetEase(Ease.OutExpo));
        sequence.Insert(0, rectTransform.DOScale(1, 0.3f).SetEase(Ease.OutExpo));
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
        Ok.onClick.AddListener(action);
    }

    public void SetOnCancelAction(UnityAction action)
    {
        Cancel.onClick.AddListener(action);
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
