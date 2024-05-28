using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace AvatarViewer.UI.Settings
{
    public class AvatarPage : BaseSettingsPage
    {
        private Avatar Avatar;

        [SerializeField] private GameObject MirrorMotion;
        [SerializeField] private GameObject TranslationScale;
        [SerializeField] private GameObject Smoothing;
        [SerializeField] private GameObject DriftBack;
        [SerializeField] private GameObject AutoBlink;
        [SerializeField] private GameObject BlinkSmoothing;

        [SerializeField] private GameObject EyebrowStrength;
        [SerializeField] private GameObject EyebrowOffset;
        [SerializeField] private GameObject EyebrowSensitivity;
        [SerializeField] private GameObject GazeSmoothing;
        [SerializeField] private GameObject GazeSensitivity;
        [SerializeField] private GameObject GazeStrength;
        [SerializeField] private GameObject GazeOffsetX;
        [SerializeField] private GameObject GazeOffsetY;

        [SerializeField] private TMP_InputField EyeCloseThreshold;
        [SerializeField] private TMP_InputField EyeOpenThreshold;

        [SerializeField] private TMP_Dropdown OtherAvatars;

        [SerializeField] private LocalizedString DialogTitle;
        [SerializeField] private LocalizedString DialogContent;
        [SerializeField] private GameObject Dialog;

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

            EyeCloseThreshold.onEndEdit.AddListener((value) => Avatar.Settings.EyeCloseThreshold = float.Parse(value));
            EyeOpenThreshold.onEndEdit.AddListener((value) => Avatar.Settings.EyeOpenThreshold = float.Parse(value));
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
            data.SetTitle(DialogTitle.GetLocalizedString());
            data.SetContent(DialogContent.GetLocalizedString().AsFormat(other.Title, Avatar.Title));
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

            EyeCloseThreshold.text = Avatar.Settings.EyeCloseThreshold.ToString();
            EyeOpenThreshold.text = Avatar.Settings.EyeOpenThreshold.ToString();
        }

        private void SetupToggle(GameObject root, UnityAction<bool> toggleEvent)
        {
            root.transform.Find("Content/Toggle").gameObject.GetComponent<Toggle>().onValueChanged.AddListener(toggleEvent);
        }

        private void LoadToggle(GameObject root, bool state) => root.transform.Find("Content/Toggle").gameObject.GetComponent<Toggle>().isOn = state;
    }
}