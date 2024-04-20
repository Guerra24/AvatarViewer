using Klak.Spout;
using OpenSee;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace AvatarViewer
{
    public class MainSceneLoader : MonoBehaviour
    {
        public PostProcessLayer PostProcessLayer;
        public SpoutSender SpoutSender;
        public OpenSeeIKTarget IKTarget;
        public OpenSeeVRMDriver Driver;

        private void Start()
        {
            RuntimeSettings.Apply(PostProcessLayer, SpoutSender, IKTarget, Driver);
        }
    }
}
