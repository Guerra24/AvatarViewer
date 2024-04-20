using AvatarViewer.Trackers;
using TMPro;
using UnityEngine.UI;

namespace AvatarViewer.Ui.Settings
{
    public class TrackerPageBase<T> : BaseSettingsPage where T : TrackerSettings
    {
        private Tracker Tracker;
        protected T Settings => (T)ApplicationPersistence.AppSettings.Trackers[Tracker];

        public TMP_InputField ListenAddress;
        public TMP_InputField Port;
        public Toggle UseLocalTracker;

        public TrackerPageBase(Tracker tracker)
        {
            Tracker = tracker;
        }

        protected override void Awake()
        {
            base.Awake();

            ListenAddress.onEndEdit.AddListener(OnListenAddressEndEdit);
            Port.onEndEdit.AddListener(OnPortEndEdit);
            UseLocalTracker.onValueChanged.AddListener(OnUseLocalTrackerValueChanged);
        }

        protected virtual void Start()
        {
            ListenAddress.text = Settings.ListenAddress;
            Port.text = Settings.Port.ToString();
            UseLocalTracker.isOn = Settings.UseLocalTracker;
        }

        private void OnListenAddressEndEdit(string value)
        {
            Settings.ListenAddress = value;
        }

        private void OnPortEndEdit(string value)
        {
            Settings.Port = int.Parse(value);
        }

        private void OnUseLocalTrackerValueChanged(bool value)
        {
            Settings.UseLocalTracker = value;
        }

    }
}
