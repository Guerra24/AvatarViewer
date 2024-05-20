using UnityEngine;
#if UNITY_STANDALONE_WIN
using UnityRawInput;
#endif

namespace AvatarViewer.Ui
{
    public class BlendshapeList : MonoBehaviour
    {

        public GameObject _template;

        private PageViewer _pageViewer;

        private bool rawInputAlreadyRunning;

        void Start()
        {
            foreach (var anim in ApplicationState.CurrentAvatar.Blendshapes)
            {
                CreateItem(anim.Key, anim.Value);
            }
            _pageViewer = GetComponentInParent<PageViewer>();
            _pageViewer.GoBackInitial.AddListener(SaveChanges);
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
            var controller = item.GetComponent<BlendshapeController>();
            controller.AvatarBlendshape = avatarBlendshape;
            controller.LoadValues(name);
        }

        private void SaveChanges()
        {
            _pageViewer.GoBackInitial.RemoveListener(SaveChanges);
            ApplicationPersistence.Save();
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
