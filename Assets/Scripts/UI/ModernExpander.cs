using UnityEngine;
using UnityEngine.UI;

namespace AvatarViewer.Ui
{
    public class ModernExpander : MonoBehaviour
    {
        public GameObject ExpandedContent;

        void Start()
        {
            var button = GetComponentInChildren<Button>();
            button.onClick.AddListener(OnClick);
        }

        public void OnClick()
        {
            ExpandedContent.SetActive(!ExpandedContent.activeSelf);
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }
    }
}
