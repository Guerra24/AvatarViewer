using AvatarViewer.Ui.Animations;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarViewer.Ui
{
    public class ModernExpander : MonoBehaviour
    {
        public GameObject ExpandedContent;
        public GameObject AnimationPlaceholder;
        public Button Button;

        private ExpanderAnimation expanderAnimation;
        private LayoutElement realLayoutElement, placeholderLayoutElement;
        private RectTransform realRectTransform;
        private CanvasGroup realCanvasGroup;

        private void Awake()
        {
            Button.onClick.AddListener(OnClick);
            expanderAnimation = ExpandedContent.GetComponent<ExpanderAnimation>();
            realLayoutElement = ExpandedContent.GetComponent<LayoutElement>();
            realRectTransform = ExpandedContent.GetComponent<RectTransform>();
            realCanvasGroup = ExpandedContent.GetComponent<CanvasGroup>();
            placeholderLayoutElement = AnimationPlaceholder.GetComponent<LayoutElement>();
        }

        public void OnClick()
        {
            OnClickAsync().Forget();
        }

        private async UniTaskVoid OnClickAsync()
        {
            expanderAnimation.Easing = !ExpandedContent.activeSelf ? AnimationEasing.EaseIn : AnimationEasing.EaseOut;

            switch (expanderAnimation.Easing)
            {
                case AnimationEasing.EaseIn:
                    realCanvasGroup.alpha = 0;
                    ExpandedContent.SetActive(!ExpandedContent.activeSelf);
                    await UniTask.Yield();
                    await UniTask.NextFrame();

                    placeholderLayoutElement.preferredHeight = realRectTransform.rect.height;

                    AnimationPlaceholder.SetActive(true);
                    realLayoutElement.ignoreLayout = true;
                    realCanvasGroup.alpha = 1;

                    await expanderAnimation.StartAnimation().ToUniTask();

                    realLayoutElement.ignoreLayout = false;
                    AnimationPlaceholder.SetActive(false);
                    break;
                case AnimationEasing.EaseOut:
                    placeholderLayoutElement.preferredHeight = realRectTransform.rect.height;

                    AnimationPlaceholder.SetActive(true);
                    realLayoutElement.ignoreLayout = true;

                    await expanderAnimation.StartAnimation().ToUniTask();

                    ExpandedContent.SetActive(!ExpandedContent.activeSelf);
                    realLayoutElement.ignoreLayout = false;

                    AnimationPlaceholder.SetActive(false);
                    break;
            }
        }
    }
}
