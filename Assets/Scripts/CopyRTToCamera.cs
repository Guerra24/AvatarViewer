using UnityEngine;

namespace AvatarViewer
{
    public class CopyRTToCamera : MonoBehaviour
    {
        public RenderTexture Source;

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(Source, destination);
        }
    }
}
