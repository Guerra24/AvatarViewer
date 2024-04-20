using UnityEngine;
using UnityEngine.UI;

namespace AvatarViewer.Ui
{
    public class ToggleUiElement : MonoBehaviour
    {

        public GameObject Element;

        void Start()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }

        public void OnClick()
        {
            Element.SetActive(!Element.activeSelf);
        }
    }
}
