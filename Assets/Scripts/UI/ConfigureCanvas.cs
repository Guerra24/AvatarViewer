using UnityEngine;

namespace AvatarViewer.UI
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
