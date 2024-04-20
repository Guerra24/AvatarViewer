using Klak.Spout;
using UnityEngine;

namespace AvatarViewer
{
    public class Spout2Controller : MonoBehaviour
    {
        private Camera Camera;
        private RenderTexture RenderTexture;
        private SpoutSender SpoutSender;

        public CopyRTToCamera Final;

        private int Width, Height;

        private void Awake()
        {
            Camera = GetComponent<Camera>();
            SpoutSender = GetComponent<SpoutSender>();
            Width = ApplicationState.RuntimeWidth;
            Height = ApplicationState.RuntimeHeight;
        }

        private void Start()
        {
            Resize();
        }

        private void Update()
        {
            if (ApplicationState.RuntimeWidth != Width || ApplicationState.RuntimeHeight != Height)
            {
                Width = ApplicationState.RuntimeWidth;
                Height = ApplicationState.RuntimeHeight;
                Debug.Log("Resized RT");
                Resize();
            }
        }

        private void Resize()
        {
            if (RenderTexture != null)
                Destroy(RenderTexture);
            RenderTexture = new RenderTexture(Width, Height, 24, RenderTextureFormat.ARGB32);
            SpoutSender.sourceTexture = Final.Source = Camera.targetTexture = RenderTexture;
        }

        private void OnDestroy()
        {
            Destroy(RenderTexture);
            RenderTexture = null;
        }

        private void OnApplicationQuit()
        {
            Destroy(RenderTexture);
            RenderTexture = null;
        }

    }
}
