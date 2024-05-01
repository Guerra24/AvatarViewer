using System;
using System.Collections.Generic;
using System.Linq;
using AvatarViewer.Trackers;
using OpenSee;
using TMPro;
using UnityEngine;

namespace AvatarViewer.Ui.Settings
{
    public class TrackersPage : BaseSettingsPage
    {

        [SerializeField]
        private TMP_Dropdown Cameras;
        [SerializeField]
        private TMP_Dropdown CameraCaps;
        [SerializeField]
        private TMP_Dropdown Microphones;
        [SerializeField]
        private TMP_Dropdown LipSyncProvider;
        [SerializeField]
        private GameObject LipSyncProfileModernBase;
        [SerializeField]
        private TMP_Dropdown LipSyncProfile;
        [SerializeField]
        private TMP_Dropdown Tracker;

        private List<OpenSeeWebcam> _cameras;

        protected override void Awake()
        {
            base.Awake();
            _cameras = OpenSeeWebcamInfo.ListCameraDetails(false);

            _cameras.ForEach(c => Cameras.options.Add(new IdDropdownData(c.name, c.id)));
            Cameras.RefreshShownValue();

            Microphones.AddOptions(Microphone.devices.ToList());

            LipSyncProvider.AddOptions(Enum.GetNames(typeof(LipSyncProvider)).ToList());

            LipSyncProfile.AddOptions(Enum.GetNames(typeof(LipSyncProfile)).ToList());

            Tracker.AddOptions(Enum.GetNames(typeof(Tracker)).ToList());

            Cameras.onValueChanged.AddListener(OnCamerasValueChanged);
            CameraCaps.onValueChanged.AddListener(OnCameraCapsValueChanged);
            Microphones.onValueChanged.AddListener(OnMicrophonesValueChanged);
            LipSyncProvider.onValueChanged.AddListener(OnLipSyncProviderValueChanged);
            LipSyncProfile.onValueChanged.AddListener(OnLipSyncProfileValueChanged);
            Tracker.onValueChanged.AddListener(OnTrackerValueChanged);
        }

        private void Start()
        {
            Cameras.value = Cameras.options.FindIndex(o => ((IdDropdownData)o).id == ApplicationPersistence.AppSettings.Camera);
            CameraCaps.value = CameraCaps.options.FindIndex(o => ((IdDropdownData)o).id == ApplicationPersistence.AppSettings.CameraCapability);
            Microphones.value = Microphones.options.FindIndex(o => o.text == ApplicationPersistence.AppSettings.Microphone);
            LipSyncProvider.value = (int)ApplicationPersistence.AppSettings.LipSyncProvider;
            LipSyncProfile.value = (int)ApplicationPersistence.AppSettings.LipSyncProfile;
            Tracker.value = (int)ApplicationPersistence.AppSettings.Tracker;
            LipSyncProfileModernBase.SetActive(ApplicationPersistence.AppSettings.LipSyncProvider == AvatarViewer.LipSyncProvider.uLipSync);
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
            LipSyncProfileModernBase.SetActive(ApplicationPersistence.AppSettings.LipSyncProvider == AvatarViewer.LipSyncProvider.uLipSync);
        }

        private void OnLipSyncProfileValueChanged(int value)
        {
            ApplicationPersistence.AppSettings.LipSyncProfile = (LipSyncProfile)value;
        }

        private void OnTrackerValueChanged(int value)
        {
            ApplicationPersistence.AppSettings.Tracker = (Tracker)value;
        }

    }
}
