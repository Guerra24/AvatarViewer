using AvatarViewer.UI.Animations;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarViewer.UI
{
    public class ToggleUiElement : MonoBehaviour
    {
        [SerializeField] private AnimationEasing Easing;
        [SerializeField] private GameObject Element;

        void Awake()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }

        public void OnClick()
        {
            bool activated = false;
            if (!Element.activeSelf)
            {
                Element.SetActive(true);
                activated = true;
            }
            if (Element.TryGetComponent<BaseAnimation>(out var anim))
            {
                anim.Easing = Easing;
                anim.StartAnimation();
            }
            if (Element.activeSelf && !activated)
                Element.SetActive(false);
        }
    }
}
