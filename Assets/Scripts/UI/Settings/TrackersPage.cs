using System;
using System.Collections.Generic;
using System.Linq;
using AvatarViewer.Trackers;
using OpenSee;
using TMPro;
using uLipSync;
using UnityEngine;

namespace AvatarViewer.UI.Settings
{
    public class TrackersPage : BaseSettingsPage
    {

        private uLipSyncSettings uLipSync => (uLipSyncSettings)ApplicationPersistence.AppSettings.LipSyncSettings[AvatarViewer.LipSyncProvider.uLipSync];
        private OculusLipSyncSettings OculusLipSync => (OculusLipSyncSettings)ApplicationPersistence.AppSettings.LipSyncSettings[AvatarViewer.LipSyncProvider.OculusLipSync];

        [SerializeField]
        private TMP_Dropdown Cameras;
        [SerializeField]
        private TMP_Dropdown CameraCaps;
        [SerializeField]
        private TMP_Dropdown Microphones;
        [SerializeField]
        private TMP_Dropdown LipSyncProvider;
        [SerializeField]
        private GameObject uLipSyncProfileModernBase;
        [SerializeField]
        private TMP_Dropdown uLipSyncProfile;
        [SerializeField]
        private TMP_Dropdown Tracker;
        [SerializeField]
        private GameObject uLipSyncMinVolume;
        [SerializeField]
        private GameObject uLipSyncMaxVolume;
        [SerializeField]
        private GameObject OculusLipSyncGain;

        private List<OpenSeeWebcam> _cameras;

        protected override void Awake()
        {
            base.Awake();
            _cameras = OpenSeeWebcamInfo.ListCameraDetails(false);

            _cameras.ForEach(c => Cameras.options.Add(new IdDropdownData(c.name, c.id)));
            Cameras.RefreshShownValue();

            Microphones.AddOptions(Microphone.devices.ToList());

            LipSyncProvider.AddOptions(Enum.GetNames(typeof(LipSyncProvider)).ToList());

            uLipSyncProfile.AddOptions(Enum.GetNames(typeof(LipSyncProfile)).ToList());

            Tracker.AddOptions(Enum.GetNames(typeof(Tracker)).ToList());

            Cameras.onValueChanged.AddListener(OnCamerasValueChanged);
            CameraCaps.onValueChanged.AddListener(OnCameraCapsValueChanged);
            Microphones.onValueChanged.AddListener(OnMicrophonesValueChanged);
            LipSyncProvider.onValueChanged.AddListener(OnLipSyncProviderValueChanged);
            uLipSyncProfile.onValueChanged.AddListener(OnLipSyncProfileValueChanged);
            Tracker.onValueChanged.AddListener(OnTrackerValueChanged);

            uLipSyncBlendShapeVRM uLipSyncBlendShapeVRM = null;
            OpenSeeVRMDriver driver = null;
            var avatarLoader = GameObject.Find("AvatarLoader")?.GetComponent<AvatarLoader>();
            if (avatarLoader != null)
            {
                uLipSyncBlendShapeVRM = avatarLoader.avatar.GetComponent<uLipSyncBlendShapeVRM>();
                driver = avatarLoader.Driver;
            }

            uLipSyncMinVolume.SetupSlider((value) =>
            {
                uLipSync.MinVolume = value;
                if (uLipSyncBlendShapeVRM != null)
                    uLipSyncBlendShapeVRM.minVolume = value;
            });
            uLipSyncMaxVolume.SetupSlider((value) =>
            {
                uLipSync.MaxVolume = value;
                if (uLipSyncBlendShapeVRM != null)
                    uLipSyncBlendShapeVRM.maxVolume = value;
            });

            OculusLipSyncGain.SetupSlider((value) =>
            {
                OculusLipSync.Gain = value;
                if(driver != null)
                    driver.gain = value;
            });
        }

        private void Start()
        {
            Cameras.value = Cameras.options.FindIndex(o => ((IdDropdownData)o).id == ApplicationPersistence.AppSettings.Camera);
            CameraCaps.value = CameraCaps.options.FindIndex(o => ((IdDropdownData)o).id == ApplicationPersistence.AppSettings.CameraCapability);
            Microphones.value = Microphones.options.FindIndex(o => o.text == ApplicationPersistence.AppSettings.Microphone);
            LipSyncProvider.value = (int)ApplicationPersistence.AppSettings.LipSyncProvider;
            uLipSyncProfile.value = (int)uLipSync.Profile;
            Tracker.value = (int)ApplicationPersistence.AppSettings.Tracker;

            uLipSyncMinVolume.LoadSlider(uLipSync.MinVolume);
            uLipSyncMaxVolume.LoadSlider(uLipSync.MaxVolume);

            OculusLipSyncGain.LoadSlider(OculusLipSync.Gain);

            var showuLipSync = ApplicationPersistence.AppSettings.LipSyncProvider == AvatarViewer.LipSyncProvider.uLipSync;
            var showOculusLipSync = ApplicationPersistence.AppSettings.LipSyncProvider == AvatarViewer.LipSyncProvider.OculusLipSync;

            uLipSyncProfileModernBase.SetActive(showuLipSync);
            uLipSyncMinVolume.SetActive(showuLipSync);
            uLipSyncMaxVolume.SetActive(showuLipSync);

            OculusLipSyncGain.SetActive(showOculusLipSync);
        }

        private void OnCamerasValueChanged(int value)
        {
            CameraCaps.ClearOptions();
            var camera = _cameras[value];

            foreach (var cap in camera.caps)
                CameraCaps.options.Add(new IdDropdownData(cap.GetPrettyCapability(), cap.id));
            CameraCaps.RefreshShownValue();
            CameraCaps.value = 0;

            if (camera.id == ApplicationPersistence.AppSettings.Camera)
                CameraCaps.value = CameraCaps.options.FindIndex(o => ((IdDropdownData)o).id == ApplicationPersistence.AppSettings.CameraCapability);
            else
                ApplicationPersistence.AppSettings.CameraCapability = 0;

            ApplicationPersistence.AppSettings.Camera = ((IdDropdownData)Cameras.options[value]).id;
        }

        private void OnCameraCapsValueChanged(int value)
        {
            ApplicationPersistence.AppSettings.CameraCapability = ((IdDropdownData)CameraCaps.options[value]).id;
        }

        private void OnMicrophonesValueChanged(int value)
        {
            ApplicationPersistence.AppSettings.Microphone = Microphones.options[value].text;
        }

        private void OnLipSyncProviderValueChanged(int value)
        {
            ApplicationPersistence.AppSettings.LipSyncProvider = (LipSyncProvider)value;
            var showuLipSync = (LipSyncProvider)value == AvatarViewer.LipSyncProvider.uLipSync;
            var showOculusLipSync = (LipSyncProvider)value == AvatarViewer.LipSyncProvider.OculusLipSync;

            uLipSyncProfileModernBase.SetActive(showuLipSync);
            uLipSyncMinVolume.SetActive(showuLipSync);
            uLipSyncMaxVolume.SetActive(showuLipSync);

            OculusLipSyncGain.SetActive(showOculusLipSync);
        }

        private void OnLipSyncProfileValueChanged(int value)
        {
            uLipSync.Profile = (LipSyncProfile)value;
        }

        private void OnTrackerValueChanged(int value)
        {
            ApplicationPersistence.AppSettings.Tracker = (Tracker)value;
        }

    }
}
