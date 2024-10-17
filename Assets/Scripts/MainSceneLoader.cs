using Klak.Spout;
using OpenSee;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace AvatarViewer
{
    public class MainSceneLoader : MonoBehaviour
    {
        [SerializeField] private PostProcessLayer PostProcessLayer;
        [SerializeField] private SpoutSender SpoutSender;
        [SerializeField] private PostProcessVolume PostProcessVolume;

        private void Start()
        {
            RuntimeSettings.Apply(PostProcessLayer, SpoutSender, PostProcessVolume);
        }
    }
}
