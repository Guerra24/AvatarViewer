using UnityEngine;
using UnityEngine.UI;

namespace AvatarViewer.UI
{
    public class PageViewerButton : MonoBehaviour
    {
        public GameObject Page;

        private PageViewer pageViewer;

        void Start()
        {
            pageViewer = GetComponentInParent<PageViewer>();
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            pageViewer.OpenPage(Page);
        }


    }
}
