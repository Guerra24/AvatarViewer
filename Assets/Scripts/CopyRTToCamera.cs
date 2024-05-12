using UnityEngine;

namespace AvatarViewer
{
    public class CopyRTToCamera : MonoBehaviour
    {
        public RenderTexture Source;
        [SerializeField] private Camera MainCamera;
        [SerializeField] private Material Blit;

        private Camera BlitCamera;

        private void Awake()
        {
            BlitCamera = GetComponent<Camera>();
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Blit.SetFloat("_Scale", BlitCamera.aspect / MainCamera.aspect);
            //Graphics.Blit(Source, destination, new Vector2(scale, 1), new Vector2((1 - scale) / 2f, 0));
            Graphics.Blit(Source, destination, Blit);
        }
    }
}
