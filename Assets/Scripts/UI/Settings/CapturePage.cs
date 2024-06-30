using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
#if UNITY_STANDALONE_WIN
using AvatarViewer.Native.Win;
#endif
using Klak.Spout;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AvatarViewer.UI.Settings
{
    public class CapturePage : BaseSettingsPage
    {
        private AppSettings Settings => ApplicationPersistence.AppSettings;

        [SerializeField] private TMP_Dropdown Mode;
        [SerializeField] private TMP_Dropdown Preset;
        [SerializeField] private TMP_InputField Width;
        [SerializeField] private TMP_InputField Height;
        [SerializeField] private TMP_Dropdown VSync;
        [SerializeField] private TMP_InputField FrameRate;
        [SerializeField] private Toggle IncreasedPriority;

        private SpoutSender SpoutSender;

        protected override void Awake()
        {
            base.Awake();
            Mode.AddOptions(Enum.GetNames(typeof(CaptureMode)).ToList());
            Preset.AddOptions(Enum.GetNames(typeof(Resolution)).ToList());
            VSync.AddOptions(new List<string> { "Disabled", "Enabled", "Enabled (Half rate)" });

            Mode.onValueChanged.AddListener(OnModeValueChanged);
            Preset.onValueChanged.AddListener(OnPresetValueChanged);
            Width.onEndEdit.AddListener(OnWidthHeightEndEdit);
            Height.onEndEdit.AddListener(OnWidthHeightEndEdit);
            VSync.onValueChanged.AddListener(OnVSyncValueChanged);
            FrameRate.onEndEdit.AddListener(OnFrameRateEndEdit);
            IncreasedPriority.onValueChanged.AddListener(OnIncreasedPriorityValueChanged);

            if (Camera.main.gameObject.TryGetComponent<SpoutSender>(out var component))
                SpoutSender = component;
        }

        private void Start()
        {
            Mode.value = (int)Settings.CaptureMode;
            Preset.value = (int)Settings.Resolution;
            Width.text = Settings.Width.ToString();
            Height.text = Settings.Height.ToString();
            VSync.value = Settings.VSync;
            FrameRate.text = Settings.TargetFrameRate.ToString();
            IncreasedPriority.isOn = Settings.IncreasedPriority;
        }

        public void OnModeValueChanged(int value)
        {
            Settings.CaptureMode = (CaptureMode)value;
            if (SpoutSender != null)
                SpoutSender.enabled = Settings.CaptureMode == CaptureMode.Spout2;
            if (Settings.CaptureMode == CaptureMode.GameWindow)
                Screen.SetResolution(ApplicationState.RuntimeWidth, ApplicationState.RuntimeHeight, false);
        }

        private void OnPresetValueChanged(int value)
        {
            Width.gameObject.SetActive(false);
            Height.gameObject.SetActive(false);
            Settings.Resolution = (Resolution)value;
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
                    Width.gameObject.SetActive(true);
                    Height.gameObject.SetActive(true);
                    break;
            }
            if (Settings.CaptureMode == CaptureMode.GameWindow)
                Screen.SetResolution(ApplicationState.RuntimeWidth, ApplicationState.RuntimeHeight, false);
        }

        private void OnWidthHeightEndEdit(string _)
        {
            if (int.TryParse(Width.text, out var width))
                ApplicationState.RuntimeWidth = Settings.Width = width;
            if (int.TryParse(Height.text, out var height))
                ApplicationState.RuntimeHeight = Settings.Height = height;

            if (Settings.CaptureMode == CaptureMode.GameWindow)
                Screen.SetResolution(ApplicationState.RuntimeWidth, ApplicationState.RuntimeHeight, false);
        }

        public void OnVSyncValueChanged(int value)
        {
            QualitySettings.vSyncCount = Settings.VSync = value;
        }

        public void OnFrameRateEndEdit(string value)
        {
            if (!int.TryParse(value, out var val))
                return;

            Application.targetFrameRate = Settings.TargetFrameRate = val;
        }

        public void OnIncreasedPriorityValueChanged(bool state)
        {
            Settings.IncreasedPriority = state;
#if UNITY_STANDALONE_WIN
            using (var p = Process.GetCurrentProcess())
                p.PriorityClass = state ? ProcessPriorityClass.AboveNormal : ProcessPriorityClass.Normal;
            if (SceneManager.GetActiveScene().buildIndex == 5)
            {
                if (state)
                    GPUPriority.ChangePriority(_D3DKMT_SCHEDULINGPRIORITYCLASS.D3DKMT_SCHEDULINGPRIORITYCLASS_HIGH, 7);
                else
                    GPUPriority.ChangePriority(_D3DKMT_SCHEDULINGPRIORITYCLASS.D3DKMT_SCHEDULINGPRIORITYCLASS_NORMAL, 0);
            }
#endif
        }
    }
}
