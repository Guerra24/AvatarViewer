using AvatarViewer.Ui;
using Klak.Spout;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarViewer
{
    public class Spout2Controller : MonoBehaviour
    {
        private Camera Camera;
        private RenderTexture RenderTexture;
        private SpoutSender SpoutSender;

        [SerializeField] private MainController MainController;
        //[SerializeField] private CopyRTToCamera Final;
        [SerializeField] private RawImage UITarget;

        private int Width, Height;

        private void Awake()
        {
            Camera = GetComponent<Camera>();
            Camera.depthTextureMode = DepthTextureMode.Depth;
            SpoutSender = GetComponent<SpoutSender>();
            Width = ApplicationState.RuntimeWidth;
            Height = ApplicationState.RuntimeHeight;
#if UNITY_EDITOR_WIN
            SpoutSender.spoutName = "AvatarViewer (DEBUG)";
#endif
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
            UITarget.texture = SpoutSender.sourceTexture /*= Final.Source*/ = Camera.targetTexture = RenderTexture;
            UITarget.GetComponent<AspectRatioFitter>().aspectRatio = Camera.aspect;

            float _1OverAspect = 1f / Camera.aspect;
            Camera.fieldOfView = 2f * Mathf.Atan(Mathf.Tan(MainController.CurrentCameraPreset.FOV * Mathf.Deg2Rad * 0.5f) * _1OverAspect) * Mathf.Rad2Deg;
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
