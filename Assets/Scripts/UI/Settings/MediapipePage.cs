using AvatarViewer.Trackers;

namespace AvatarViewer.UI.Settings
{
    public class MediapipePage : TrackerPageBase<MediapipeTrackerSettings>
    {
        public MediapipePage() : base(Tracker.Mediapipe)
        {
        }
    }
}
