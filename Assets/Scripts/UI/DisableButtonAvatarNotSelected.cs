using UnityEngine;
using UnityEngine.UI;

namespace AvatarViewer.Ui
{
    public class DisableButtonAvatarNotSelected : MonoBehaviour
    {
        private Button Button;

        private void Awake()
        {
            Button = GetComponent<Button>();
        }

        void Start()
        {
            Button.interactable = ApplicationState.CurrentAvatar != null;
        }

        void Update()
        {
            Button.interactable = ApplicationState.CurrentAvatar != null;
        }
    }
}
