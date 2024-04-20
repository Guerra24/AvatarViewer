using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace AvatarViewer.Trackers
{
    public static class TrackerManager
    {
        private static Dictionary<Tracker, TrackerInfo> Trackers = new()
        {
            //{ Tracker.OpenSee, new TrackerInfo ("OpenSee", "" , new Version(0,0,0)) },
            { Tracker.Mediapipe, new TrackerInfo ("Mediapipe", "main", new Version(0,0,0)) },
        };

        private static string PersistentPath, TemporaryPath;
        private static HttpClient Client;

        public static void Initialize(string persistentPath, string temporaryPath)
        {
            PersistentPath = persistentPath;
            TemporaryPath = temporaryPath;
            Client = new HttpClient();
            Client.BaseAddress = new Uri("https://s3.guerra24.net/projects/avatarviewer/trackers/");

        }

        public static async Task Download(Tracker tracker)
        {
            var trackerInfo = Trackers[tracker];

            var finalDirectory = Directory.CreateDirectory(Path.Combine(PersistentPath, trackerInfo.Name.ToLower()));

            var versionFile = Path.Combine(finalDirectory.FullName, "version.txt");
            var executable = Path.Combine(finalDirectory.FullName, trackerInfo.Executable);

            if (File.Exists(versionFile) && Version.TryParse(await File.ReadAllTextAsync(versionFile), out var ver) && ver == trackerInfo.Version && File.Exists(executable))
                return;

            using var request = await Client.GetAsync($"{trackerInfo.Name.ToLower()}/{trackerInfo.Version}/{GetPlatform()}.zip", HttpCompletionOption.ResponseHeadersRead);
            if (!request.IsSuccessStatusCode)
                return;

            var tempPath = Path.Combine(TemporaryPath, $"{trackerInfo.Name.ToLower()}-{trackerInfo.Version}.zip");

            await File.WriteAllBytesAsync(tempPath, await request.Content.ReadAsByteArrayAsync());

            {
                foreach (var file in finalDirectory.EnumerateFiles())
                    file.Delete();
                foreach (var dir in finalDirectory.EnumerateDirectories())
                    dir.Delete(true);
            }

            ZipFile.ExtractToDirectory(tempPath, finalDirectory.FullName);

            File.Delete(tempPath);

            await File.WriteAllTextAsync(versionFile, trackerInfo.Version.ToString());

#if UNITY_STANDALONE_LINUX
            {
                using var chmod = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        FileName = "chmod",
                        Arguments = $"+x {executable}"
                    }
                };
                chmod.Start();
                chmod.WaitForExit();
            }
#endif
        }

        public static async Task DownloadAll()
        {
            foreach (var tracker in Trackers)
                await Download(tracker.Key);
        }

        public static TrackerInstance Launch(Tracker tracker, TrackerLauncherSettings settings)
        {
            var trackerInfo = Trackers[tracker];

            switch (tracker)
            {
                case Tracker.OpenSee:
                    break;
                case Tracker.Mediapipe:
                    return new MediapipeLauncher().Launch((MediapipeLauncherSettings)settings, trackerInfo, PersistentPath);
            }

            return null;
        }

        private static string GetPlatform()
        {
#if UNITY_STANDALONE_LINUX
            return "linux";
#endif
#if UNITY_STANDALONE_WIN
            return "win";
#endif
        }

    }

    public enum Tracker
    {
        OpenSee, Mediapipe
    }

    public interface ITrackerLauncher<T> where T : TrackerLauncherSettings
    {
        public TrackerInstance Launch(T settings, TrackerInfo tracker, string persistentPath);
    }

    public class TrackerLauncherSettings
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public int Camera { get; set; }
    }

    public class MediapipeLauncherSettings : TrackerLauncherSettings
    {
        public int CameraCap { get; set; }
    }

    public class MediapipeLauncher : ITrackerLauncher<MediapipeLauncherSettings>
    {
        public TrackerInstance Launch(MediapipeLauncherSettings settings, TrackerInfo tracker, string persistentPath)
        {
            var processStartInfo = new ProcessStartInfo();
            processStartInfo.ArgumentList.Add("--ip");
            processStartInfo.ArgumentList.Add(settings.Ip);
            processStartInfo.ArgumentList.Add("--port");
            processStartInfo.ArgumentList.Add(settings.Port.ToString());
            processStartInfo.ArgumentList.Add("--camera");
            processStartInfo.ArgumentList.Add(settings.Camera.ToString());
            processStartInfo.ArgumentList.Add("--camera-cap");
            processStartInfo.ArgumentList.Add(settings.CameraCap.ToString());
#if UNITY_STANDALONE_WIN
            processStartInfo.ArgumentList.Add("--mode");
            processStartInfo.ArgumentList.Add("mediapipe");
#endif
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;

            var workingDirectory = Path.Combine(persistentPath, tracker.Name.ToLower());

            processStartInfo.FileName = Path.Combine(workingDirectory, tracker.Executable);
            processStartInfo.WorkingDirectory = workingDirectory;

            var process = new Process();
            process.StartInfo = processStartInfo;
            process.EnableRaisingEvents = true;
            process.OutputDataReceived += (sender, e) => Debug.WriteLine($"{e.Data}\n");
            process.ErrorDataReceived += (sender, e) => Debug.WriteLine($"{e.Data}\n");
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return new TrackerInstance(process);
        }
    }

    public class TrackerInstance : IDisposable
    {
        private bool _disposed;

        public Process Process { get; }

        public TrackerInstance(Process process)
        {
            Process = process;
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;

            if (!Process.HasExited)
            {
                Process.CloseMainWindow();
                Process.Kill();
            }
            Process.Dispose();
        }
    }
}