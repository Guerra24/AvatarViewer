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
            SetupSlider(TranslationScale, (value) => Avatar.Settings.TranslationScale = value);
            SetupSlider(Smoothing, (value) => Avatar.Settings.Smoothing = value);
            SetupToggle(DriftBack, (state) => Avatar.Settings.DriftBack = state);
            SetupToggle(AutoBlink, (state) => Avatar.Settings.AutoBlink = state);
            SetupSlider(BlinkSmoothing, (value) => Avatar.Settings.BlinkSmoothing = value);
            SetupSlider(EyebrowStrength, (value) => Avatar.Settings.EyebrowStrength = value);
            SetupSlider(EyebrowOffset, (value) => Avatar.Settings.EyebrowZero = value);
            SetupSlider(EyebrowSensitivity, (value) => Avatar.Settings.EyebrowSensitivity = value);
            SetupSlider(GazeSmoothing, (value) => Avatar.Settings.GazeSmoothing = value);
            SetupSlider(GazeSensitivity, (value) => Avatar.Settings.GazeSensitivity = value);
            SetupSlider(GazeStrength, (value) => Avatar.Settings.GazeStrength = value);
            SetupSlider(GazeOffsetX, (value) => Avatar.Settings.GazeHorizontalOffset = value);
            SetupSlider(GazeOffsetY, (value) => Avatar.Settings.GazeVerticalOffset = value);
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
            LoadSlider(TranslationScale, Avatar.Settings.TranslationScale);
            LoadSlider(Smoothing, Avatar.Settings.Smoothing);
            LoadToggle(DriftBack, Avatar.Settings.DriftBack);
            LoadToggle(AutoBlink, Avatar.Settings.AutoBlink);
            LoadSlider(BlinkSmoothing, Avatar.Settings.BlinkSmoothing);
            LoadSlider(EyebrowStrength, Avatar.Settings.EyebrowStrength);
            LoadSlider(EyebrowOffset, Avatar.Settings.EyebrowZero);
            LoadSlider(EyebrowSensitivity, Avatar.Settings.EyebrowSensitivity);
            LoadSlider(GazeSmoothing, Avatar.Settings.GazeSmoothing);
            LoadSlider(GazeSensitivity, Avatar.Settings.GazeSensitivity);
            LoadSlider(GazeStrength, Avatar.Settings.GazeStrength);
            LoadSlider(GazeOffsetX, Avatar.Settings.GazeHorizontalOffset);
            LoadSlider(GazeOffsetY, Avatar.Settings.GazeVerticalOffset);
        }

        private void SetupSlider(GameObject root, UnityAction<float> sliderEvent)
        {
            var text = root.transform.Find("Content/Text").gameObject.GetComponent<TMP_Text>();
            var slider = root.transform.Find("Content/Slider").gameObject.GetComponent<Slider>();
            slider.onValueChanged.AddListener(sliderEvent);
            slider.onValueChanged.AddListener((value) => text.text = value.ToString("n2"));
        }

        private void LoadSlider(GameObject root, float value) => root.transform.Find("Content/Slider").gameObject.GetComponent<Slider>().value = value;

        private void SetupToggle(GameObject root, UnityAction<bool> toggleEvent)
        {
            root.transform.Find("Content/Toggle").gameObject.GetComponent<Toggle>().onValueChanged.AddListener(toggleEvent);
        }

        private void LoadToggle(GameObject root, bool state) => root.transform.Find("Content/Toggle").gameObject.GetComponent<Toggle>().isOn = state;
    }
}
