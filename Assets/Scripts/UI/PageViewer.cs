using System.Collections.Generic;
using AvatarViewer.UI.Animations;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AvatarViewer.UI
{
    public class PageViewer : MonoBehaviour
    {

        [SerializeField] private GameObject _headerPageSeparator;
        [SerializeField] private TMP_Text _headerPageText;
        [SerializeField] private Button _headerBackButton;
        [SerializeField] private GameObject _header;
        [SerializeField] private GameObject _content;
        [SerializeField] private GameObject _initialPage;

        public UnityEvent GoBackInitial;

        public bool HideOnGoBackInitial;

        private Stack<GameObject> pageStack = new();

        private bool _working;

        void Awake()
        {
            _headerBackButton.onClick.AddListener(GoBack);

            OpenPage(_initialPage, false);
        }

        private void GoBack()
        {
            if (_working)
                return;
            _working = true;
            GoBackAsync().Forget();
        }

        private async UniTaskVoid GoBackAsync()
        {
            if (pageStack.Count > 1)
            {
                var oldPage = _content.transform.GetChild(0).gameObject;
                var animOut = oldPage.GetComponent<PageSlide>();
                animOut.Mode = PageSlideMode.Left;
                animOut.Easing = AnimationEasing.EaseOut;
                await animOut.StartAnimation().ToUniTask();
                Destroy(oldPage);

                pageStack.Pop();

                var page = Instantiate(pageStack.Peek(), _content.transform, false);
                var pageSlide = page.AddComponent<PageSlide>();
                pageSlide.Mode = PageSlideMode.Right;
                pageSlide.Easing = AnimationEasing.EaseIn;

                var items = _header.transform.childCount;
                Destroy(_header.transform.GetChild(items - 1).gameObject);
                Destroy(_header.transform.GetChild(items - 2).gameObject);
            }
            else
            {
                if (HideOnGoBackInitial)
                {
                    if (TryGetComponent<BaseAnimation>(out var anim))
                    {
                        anim.Easing = AnimationEasing.EaseOut;
                        await anim.StartAnimation().ToUniTask();
                    }
                    gameObject.SetActive(false);
                }
            }
            GoBackInitial.Invoke();
            _working = false;
        }

        public void OpenPage(GameObject template, bool animate = true)
        {
            if (_working)
                return;
            _working = true;
            OpenPageAsync(template, animate).Forget();
        }

        private async UniTaskVoid OpenPageAsync(GameObject template, bool animate = true)
        {
            if (pageStack.Count > 0)
            {
                var oldPage = _content.transform.GetChild(0).gameObject;
                var animOut = oldPage.GetComponent<PageSlide>();
                animOut.Mode = PageSlideMode.Right;
                animOut.Easing = AnimationEasing.EaseOut;
                await animOut.StartAnimation().ToUniTask();
                Destroy(oldPage);
            }

            var page = Instantiate(template, _content.transform, false);
            var pageSlide = page.AddComponent<PageSlide>();
            pageSlide.AutoStart = animate;

            var details = page.GetComponent<PageViewerPage>();

            if (pageStack.Count > 0)
            {
                var pageSeparator = Instantiate(_headerPageSeparator);
                pageSeparator.transform.SetParent(_header.transform, false);
            }

            var pageText = Instantiate(_headerPageText, _header.transform, false);
            pageText.text = details.LocalizedTitle.IsEmpty ? details.Title : await details.LocalizedTitle.GetLocalizedStringAsync();

            pageStack.Push(template);
            _working = false;
        }

    }
}
