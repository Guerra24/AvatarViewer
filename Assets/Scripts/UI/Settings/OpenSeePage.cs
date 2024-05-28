using AvatarViewer.Trackers;
using TMPro;

namespace AvatarViewer.UI.Settings
{
    public class OpenSeePage : TrackerPageBase<OpenSeeTrackerSettings>
    {

        public TMP_Dropdown Quality;

        public OpenSeePage() : base(Tracker.OpenSee)
        {
        }

        protected override void Awake()
        {
            base.Awake();
            Quality.options.Add(new IdDropdownData("High (Wink)", 4));
            Quality.options.Add(new IdDropdownData("High", 3));
            Quality.options.Add(new IdDropdownData("Medium", 2));
            Quality.options.Add(new IdDropdownData("Low", 1));
            Quality.RefreshShownValue();
            Quality.onValueChanged.AddListener(OnQualityValueChanged);
        }

        protected override void Start()
        {
            base.Start();
            Quality.value = Quality.options.FindIndex(o => ((IdDropdownData)o).id == Settings.Quality);
        }

        private void OnQualityValueChanged(int value)
        {
            Settings.Quality = ((IdDropdownData)Quality.options[value]).id;
        }

    }
}
