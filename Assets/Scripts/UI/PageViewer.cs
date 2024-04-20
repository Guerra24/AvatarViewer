using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AvatarViewer.Ui
{
    public class PageViewer : MonoBehaviour
    {

        public GameObject _headerPageSeparator;
        public GameObject _headerPageText;
        public GameObject _headerBackButton;
        public GameObject _header;
        public GameObject _content;
        public GameObject _initialPage;

        public UnityEvent GoBackInitial;

        public bool HideOnGoBackInitial;

        private Stack<GameObject> pageStack = new();

        void Awake()
        {
            _headerBackButton.GetComponent<Button>().onClick.AddListener(GoBack);

            OpenPage(_initialPage);
        }

        private void GoBack()
        {
            if (pageStack.Count > 1)
            {
                Destroy(_content.transform.GetChild(0).gameObject);

                pageStack.Pop();

                Instantiate(pageStack.Peek(), _content.transform, false);

                var items = _header.transform.childCount;
                Destroy(_header.transform.GetChild(items - 1).gameObject);
                Destroy(_header.transform.GetChild(items - 2).gameObject);
            }
            else
            {
                if (HideOnGoBackInitial)
                    gameObject.SetActive(false);
            }
            GoBackInitial.Invoke();
        }

        public void OpenPage(GameObject template)
        {
            if (pageStack.Count > 0)
                Destroy(_content.transform.GetChild(0).gameObject);

            var page = Instantiate(template, _content.transform, false);
            var details = page.GetComponent<PageViewerPage>();

            if (pageStack.Count > 0)
            {
                var pageSeparator = Instantiate(_headerPageSeparator);
                pageSeparator.transform.SetParent(_header.transform, false);
            }

            var pageText = Instantiate(_headerPageText, _header.transform, false);
            pageText.GetComponent<TMP_Text>().text = details.Title;

            pageStack.Push(template);
        }

    }
}
