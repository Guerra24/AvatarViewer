using AvatarViewer.UI.Settings;
using UnityEngine;
#if UNITY_STANDALONE_WIN
using UnityRawInput;
#endif

namespace AvatarViewer.UI.Setting
{
    public class BlendshapeList : BaseSettingsPage
    {

        [SerializeField] private BlendshapeController _template;

        private bool rawInputAlreadyRunning;

        void Start()
        {
            foreach (var anim in ApplicationState.CurrentAvatar.Blendshapes)
            {
                CreateItem(anim.Key, anim.Value);
            }
#if UNITY_STANDALONE_WIN
            rawInputAlreadyRunning = RawInput.IsRunning;
            if (!rawInputAlreadyRunning)
            {
                RawInput.WorkInBackground = true;
                RawInput.Start();
            }
#endif
        }

        public void CreateItem(string name, AvatarBlendshape avatarBlendshape)
        {
            var item = Instantiate(_template, this.transform, false);
            item.AvatarBlendshape = avatarBlendshape;
            item.LoadValues(name);
        }


        private void OnDestroy()
        {
#if UNITY_STANDALONE_WIN
            if (!rawInputAlreadyRunning)
                RawInput.Stop();
#endif
        }

        private void OnApplicationQuit()
        {
#if UNITY_STANDALONE_WIN
            if (!rawInputAlreadyRunning)
                RawInput.Stop();
#endif
        }
    }
}
