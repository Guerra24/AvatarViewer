using UnityEngine;

namespace AvatarViewer.UI.Settings
{
    public class BaseSettingsPage : MonoBehaviour
    {
        private PageViewer _pageViewer;

        protected virtual void Awake()
        {
            _pageViewer = GetComponentInParent<PageViewer>();
            _pageViewer.GoBackInitial.AddListener(SaveChanges);
        }

        private void SaveChanges()
        {
            _pageViewer.GoBackInitial.RemoveListener(SaveChanges);
            ApplicationPersistence.Save();
        }
    }
}
