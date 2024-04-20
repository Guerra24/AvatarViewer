using UnityEngine;

namespace AvatarViewer.Ui
{
    public class BlendshapeList : MonoBehaviour
    {

        public GameObject _template;

        private PageViewer _pageViewer;

        void Start()
        {
            foreach (var anim in ApplicationState.CurrentAvatar.Blendshapes)
            {
                CreateItem(anim.Key, anim.Value);
            }
            _pageViewer = GetComponentInParent<PageViewer>();
            _pageViewer.GoBackInitial.AddListener(SaveChanges);
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

    }
}
