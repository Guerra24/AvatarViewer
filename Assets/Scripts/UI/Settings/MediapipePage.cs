using AvatarViewer.Trackers;

namespace AvatarViewer.Ui.Settings
{
    public class MediapipePage : TrackerPageBase<MediapipeTrackerSettings>
    {
        public MediapipePage() : base(Tracker.Mediapipe)
        {
        }
    }
}
