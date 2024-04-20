#nullable enable
using System.Diagnostics;
using Klak.Spout;
using OpenSee;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace AvatarViewer
{
    public static class RuntimeSettings
    {
        private static AppSettings Settings => ApplicationPersistence.AppSettings;

        public static void Apply(PostProcessLayer? PostProcessLayer = null, SpoutSender? SpoutSender = null, OpenSeeIKTarget? iKTarget = null, OpenSeeVRMDriver? driver = null)
        {
            switch (Settings.AntiAliasing)
            {
                case AntiAliasing.Disabled:
                    if (PostProcessLayer != null)
                        PostProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.None;
                    QualitySettings.antiAliasing = 0;
                    break;
                case AntiAliasing.FXAA:
                    if (PostProcessLayer != null)
                        PostProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
                    QualitySettings.antiAliasing = 0;
                    break;
                case AntiAliasing.SMAA:
                    if (PostProcessLayer != null)
                        PostProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
                    QualitySettings.antiAliasing = 0;
                    break;
                case AntiAliasing.TAA:
                    if (PostProcessLayer != null)
                        PostProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
                    QualitySettings.antiAliasing = 0;
                    break;
                case AntiAliasing.MSAA:
                    if (PostProcessLayer != null)
                        PostProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.None;
                    QualitySettings.antiAliasing = Settings.MSAALevel;
                    break;
            }
            QualitySettings.shadowResolution = Settings.ShadowResolution;
            QualitySettings.vSyncCount = Settings.VSync;
            Application.targetFrameRate = Settings.TargetFrameRate;
            if (SpoutSender != null)
                SpoutSender.enabled = Settings.CaptureMode == CaptureMode.Spout2;
            switch (Settings.Resolution)
            {
                case Resolution.Res720:
                    ApplicationState.RuntimeWidth = 1280;
                    ApplicationState.RuntimeHeight = 720;
                    break;
                case Resolution.Res1080:
                    ApplicationState.RuntimeWidth = 1920;
                    ApplicationState.RuntimeHeight = 1080;
                    break;
                case Resolution.Res1440:
                    ApplicationState.RuntimeWidth = 2560;
                    ApplicationState.RuntimeHeight = 1440;
                    break;
                case Resolution.ResCustom:
                    ApplicationState.RuntimeWidth = Settings.Width;
                    ApplicationState.RuntimeHeight = Settings.Height;
                    break;
            }
            if (Settings.CaptureMode == CaptureMode.GameWindow)
                Screen.SetResolution(ApplicationState.RuntimeWidth, ApplicationState.RuntimeHeight, false);
            if (iKTarget != null)
            {
                var settings = ApplicationState.CurrentAvatar.Settings;
                iKTarget.mirrorMotion = settings.Mirror;
                iKTarget.smooth = settings.Smoothing;
                iKTarget.driftBack = settings.DriftBack;
            }
            if (driver != null)
            {
                var settings = ApplicationState.CurrentAvatar.Settings;
                driver.autoBlink = settings.AutoBlink;
                driver.blinkSmoothing = settings.BlinkSmoothing;
                driver.eyeClosedThreshold = settings.EyeCloseThreshold;
                driver.eyeOpenedThreshold = settings.EyeOpenThreshold;
                driver.eyebrowStrength = settings.EyebrowStrength;
                driver.eyebrowZero = settings.EyebrowZero;
                driver.eyebrowSensitivity = settings.EyebrowSensitivity;
                driver.gazeSmoothing = settings.GazeSmoothing;
                driver.gazeStabilizer = 1.0f - settings.GazeSensitivity;
                driver.gazeStrength = settings.GazeStrength;
                driver.gazeCenter.x = settings.GazeHorizontalOffset;
                driver.gazeCenter.y = settings.GazeVerticalOffset;
            }
#if UNITY_STANDALONE
            using (var p = Process.GetCurrentProcess())
                p.PriorityClass = Settings.IncreasedPriority ? ProcessPriorityClass.AboveNormal : ProcessPriorityClass.Normal;
#endif
        }
    }
}
