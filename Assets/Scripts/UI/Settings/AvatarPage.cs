using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AvatarViewer.Ui.Settings
{
    public class AvatarPage : BaseSettingsPage
    {
        private Avatar Avatar;

        public GameObject MirrorMotion;
        public GameObject TranslationScale;
        public GameObject Smoothing;
        public GameObject DriftBack;
        public GameObject AutoBlink;
        public GameObject BlinkSmoothing;

        public GameObject EyebrowStrength;
        public GameObject EyebrowOffset;
        public GameObject EyebrowSensitivity;
        public GameObject GazeSmoothing;
        public GameObject GazeSensitivity;
        public GameObject GazeStrength;
        public GameObject GazeOffsetX;
        public GameObject GazeOffsetY;

        public TMP_Dropdown OtherAvatars;

        public GameObject Dialog;

        protected override void Awake()
        {
            base.Awake();
            foreach (var avatar in ApplicationPersistence.AppSettings.Avatars)
                OtherAvatars.options.Add(new GuidDropdownData(avatar.Title, avatar.Guid));
            OtherAvatars.RefreshShownValue();

            Avatar = ApplicationState.CurrentAvatar;
            SetupToggle(MirrorMotion, (state) => Avatar.Settings.Mirror = state);
            TranslationScale.SetupSlider((value) => Avatar.Settings.TranslationScale = value);
            Smoothing.SetupSlider((value) => Avatar.Settings.Smoothing = value);
            SetupToggle(DriftBack, (state) => Avatar.Settings.DriftBack = state);
            SetupToggle(AutoBlink, (state) => Avatar.Settings.AutoBlink = state);
            BlinkSmoothing.SetupSlider((value) => Avatar.Settings.BlinkSmoothing = value);
            EyebrowStrength.SetupSlider((value) => Avatar.Settings.EyebrowStrength = value);
            EyebrowOffset.SetupSlider((value) => Avatar.Settings.EyebrowZero = value);
            EyebrowSensitivity.SetupSlider((value) => Avatar.Settings.EyebrowSensitivity = value);
            GazeSmoothing.SetupSlider((value) => Avatar.Settings.GazeSmoothing = value);
            GazeSensitivity.SetupSlider((value) => Avatar.Settings.GazeSensitivity = value);
            GazeStrength.SetupSlider((value) => Avatar.Settings.GazeStrength = value);
            GazeOffsetX.SetupSlider((value) => Avatar.Settings.GazeHorizontalOffset = value);
            GazeOffsetY.SetupSlider((value) => Avatar.Settings.GazeVerticalOffset = value);
        }

        private void Start()
        {
            Load();
        }

        private void Update()
        {
            if (!Avatar.Equals(ApplicationState.CurrentAvatar))
            {
                Avatar = ApplicationState.CurrentAvatar;
                Load();
            }
        }

        public void CopySettingsOnClick()
        {
            var other = ApplicationPersistence.AppSettings.Avatars.First(a => a.Guid == ((GuidDropdownData)OtherAvatars.options[OtherAvatars.value]).guid);
            var dialog = Instantiate(Dialog, GameObject.Find("Canvas").transform, false);
            var data = dialog.GetComponentInChildren<Dialog>();
            data.SetTitle("Copy settings");
            data.SetContent($"Settings from {other.Title} will be copied to {Avatar.Title}. Are you sure?");
            data.SetOnOkAction(() =>
            {
                Avatar.Settings = new AvatarSettings(other.Settings);
                Load();
            });
        }

        private void Load()
        {
            LoadToggle(MirrorMotion, Avatar.Settings.Mirror);
            TranslationScale.LoadSlider(Avatar.Settings.TranslationScale);
            Smoothing.LoadSlider(Avatar.Settings.Smoothing);
            LoadToggle(DriftBack, Avatar.Settings.DriftBack);
            LoadToggle(AutoBlink, Avatar.Settings.AutoBlink);
            BlinkSmoothing.LoadSlider(Avatar.Settings.BlinkSmoothing);
            EyebrowStrength.LoadSlider(Avatar.Settings.EyebrowStrength);
            EyebrowOffset.LoadSlider(Avatar.Settings.EyebrowZero);
            EyebrowSensitivity.LoadSlider(Avatar.Settings.EyebrowSensitivity);
            GazeSmoothing.LoadSlider(Avatar.Settings.GazeSmoothing);
            GazeSensitivity.LoadSlider(Avatar.Settings.GazeSensitivity);
            GazeStrength.LoadSlider(Avatar.Settings.GazeStrength);
            GazeOffsetX.LoadSlider(Avatar.Settings.GazeHorizontalOffset);
            GazeOffsetY.LoadSlider(Avatar.Settings.GazeVerticalOffset);
        }

        private void SetupToggle(GameObject root, UnityAction<bool> toggleEvent)
        {
            root.transform.Find("Content/Toggle").gameObject.GetComponent<Toggle>().onValueChanged.AddListener(toggleEvent);
        }

        private void LoadToggle(GameObject root, bool state) => root.transform.Find("Content/Toggle").gameObject.GetComponent<Toggle>().isOn = state;
    }
}