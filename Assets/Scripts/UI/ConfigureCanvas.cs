using UnityEngine;

namespace AvatarViewer.Ui
{
    public class ConfigureCanvas : MonoBehaviour
    {
        public Camera Camera;

        private void Awake()
        {
            var canvas = GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera;
        }
    }
}
