using System.Diagnostics;
using System.IO;
using OpenSee;
using UnityEngine;

namespace AvatarViewer.Trackers
{
    public class MediapipeLauncher : MonoBehaviour
    {

        /*private Process Process;
        private Job Job;*/
        private TrackerInstance TrackerInstance;
        private MediapipeTrackerSettings TrackerSettings => (MediapipeTrackerSettings)ApplicationPersistence.AppSettings.Trackers[Tracker.Mediapipe];

        void Start()
        {
            /*if (Job == null)
                Job = new();*/
            if (TrackerSettings.UseLocalTracker && ApplicationPersistence.AppSettings.Tracker == Tracker.Mediapipe)
                StartTracker();
        }

        private void StartTracker()
        {
            var settings = ApplicationPersistence.AppSettings;
            /*
            var processStartInfo = new ProcessStartInfo();
            processStartInfo.ArgumentList.Add("--ip");
            processStartInfo.ArgumentList.Add(TrackerSettings.UseLocalTracker ? "127.0.0.1" : TrackerSettings.ListenAddress);
            processStartInfo.ArgumentList.Add("--port");
            processStartInfo.ArgumentList.Add(TrackerSettings.Port.ToString());
            processStartInfo.ArgumentList.Add("--camera");
            processStartInfo.ArgumentList.Add(settings.Camera.ToString());
            processStartInfo.ArgumentList.Add("--camera-cap");
            processStartInfo.ArgumentList.Add(settings.CameraCapability.ToString());
            processStartInfo.ArgumentList.Add("--mode");
            processStartInfo.ArgumentList.Add("mediapipe");
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.FileName = Path.Combine(Application.streamingAssetsPath, "mediapipe", "main.exe");
            processStartInfo.WorkingDirectory = Path.Combine(Application.streamingAssetsPath, "mediapipe");

            Process = new Process();
            Process.StartInfo = processStartInfo;
            Process.EnableRaisingEvents = true;
            Process.OutputDataReceived += (sender, e) => UnityEngine.Debug.Log($"{e.Data}\n");
            Process.ErrorDataReceived += (sender, e) => UnityEngine.Debug.Log($"{e.Data}\n");
            Process.Start();
            Process.BeginOutputReadLine();
            Process.BeginErrorReadLine();
            Job.AddProcess(Process.Handle);*/
            var launcherSettings = new MediapipeLauncherSettings();
            launcherSettings.Ip = TrackerSettings.UseLocalTracker ? "127.0.0.1" : TrackerSettings.ListenAddress;
            launcherSettings.Port = TrackerSettings.Port;
            launcherSettings.Camera = settings.Camera;
            launcherSettings.CameraCap = settings.CameraCapability;
            TrackerInstance = TrackerManager.Launch(Tracker.Mediapipe, launcherSettings);
        }

        private void StopTracker()
        {
            /*if (Process == null)
                return;
            if (!Process.HasExited)
            {
                Process.CloseMainWindow();
                Process.Kill();
            }
            Process.Dispose();
            Process = null;*/
            if (TrackerInstance != null)
                TrackerInstance.Dispose();
        }

        private void CleanJob()
        {
            /*if (Job != null)
            {
                Job.Dispose();
                Job = null;
            }*/
        }

        private void OnDestroy()
        {
            StopTracker();
            CleanJob();
        }

        private void OnApplicationQuit()
        {
            StopTracker();
            CleanJob();
        }

    }
}
