using UnityEngine;

namespace AvatarViewer
{
    public class CopyRTToCamera : MonoBehaviour
    {
        public RenderTexture Source;

        private Camera BlitCamera;
        [SerializeField]
        private Camera MainCamera;

        private void Awake()
        {
            BlitCamera = GetComponent<Camera>();
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            var scale = BlitCamera.aspect / MainCamera.aspect;
            Graphics.Blit(Source, destination, new Vector2(scale, 1), new Vector2((1 - scale) / 2f, 0));
        }
    }
}
