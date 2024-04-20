using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace AvatarViewer.Ui.Settings
{
    public class QualityPage : BaseSettingsPage
    {

        private AppSettings Settings => ApplicationPersistence.AppSettings;

        public TMP_Dropdown AntiAliasing;
        public TMP_Dropdown MSAALevel;
        public TMP_Dropdown ShadowQuality;

        private PostProcessLayer PostProcessLayer;

        protected override void Awake()
        {
            base.Awake();
            AntiAliasing.AddOptions(Enum.GetNames(typeof(AntiAliasing)).ToList());
            ShadowQuality.AddOptions(Enum.GetNames(typeof(ShadowResolution)).ToList());

            MSAALevel.options.Add(new IdDropdownData("2X", 2));
            MSAALevel.options.Add(new IdDropdownData("4X", 4));
            MSAALevel.options.Add(new IdDropdownData("8X", 8));
            MSAALevel.RefreshShownValue();

            AntiAliasing.onValueChanged.AddListener(OnAntiAliasingValueChanged);
            MSAALevel.onValueChanged.AddListener(OnMSAALevelValueChanged);
            ShadowQuality.onValueChanged.AddListener(OnShadowQualityValueChanged);
            if (Camera.main.gameObject.TryGetComponent<PostProcessLayer>(out var component))
                PostProcessLayer = component;
        }

        private void Start()
        {
            AntiAliasing.value = (int)Settings.AntiAliasing;
            ShadowQuality.value = (int)Settings.ShadowResolution;

            MSAALevel.value = MSAALevel.options.FindIndex(o => ((IdDropdownData)o).id == Settings.MSAALevel);
        }

        private void OnAntiAliasingValueChanged(int value)
        {
            Settings.AntiAliasing = (AntiAliasing)value;
            switch (Settings.AntiAliasing)
            {
                case AvatarViewer.AntiAliasing.Disabled:
                    if (PostProcessLayer != null)
                        PostProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.None;
                    QualitySettings.antiAliasing = 0;
                    break;
                case AvatarViewer.AntiAliasing.FXAA:
                    if (PostProcessLayer != null)
                        PostProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
                    QualitySettings.antiAliasing = 0;
                    break;
                case AvatarViewer.AntiAliasing.SMAA:
                    if (PostProcessLayer != null)
                        PostProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
                    QualitySettings.antiAliasing = 0;
                    break;
                case AvatarViewer.AntiAliasing.TAA:
                    if (PostProcessLayer != null)
                        PostProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
                    QualitySettings.antiAliasing = 0;
                    break;
                case AvatarViewer.AntiAliasing.MSAA:
                    if (PostProcessLayer != null)
                        PostProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.None;
                    QualitySettings.antiAliasing = Settings.MSAALevel;
                    break;
            }
        }

        private void OnMSAALevelValueChanged(int value)
        {
            var item = (IdDropdownData)MSAALevel.options[value];

            Settings.MSAALevel = item.id;

            if (Settings.AntiAliasing == AvatarViewer.AntiAliasing.MSAA)
                QualitySettings.antiAliasing = Settings.MSAALevel;
        }

        private void OnShadowQualityValueChanged(int value)
        {
            QualitySettings.shadowResolution = Settings.ShadowResolution = (ShadowResolution)value;
        }

    }
}
