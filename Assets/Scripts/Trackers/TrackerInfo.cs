using System;

namespace AvatarViewer.Trackers
{
    public class TrackerInfo
    {
        public string Name { get; }
        private string _executable;
        public string Executable
        {
            get
            {
#if UNITY_STANDALONE_WIN
                return $"{_executable}.exe";
#else
                return _executable;
#endif
            }
        }
        public Version Version { get; }

        public TrackerInfo(string name, string executable, Version version)
        {
            Name = name;
            _executable = executable;
            Version = version;
        }

    }
}