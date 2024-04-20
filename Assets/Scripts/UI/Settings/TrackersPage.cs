using System;
using System.Collections.Generic;
using System.Linq;
using AvatarViewer.Trackers;
using OpenSee;
using TMPro;
using UnityEngine;

namespace AvatarViewer.Ui.Settings
{
    public class TrackersPage : MonoBehaviour
    {
        private PageViewer _pageViewer;

        public TMP_Dropdown Cameras;
        public TMP_Dropdown CameraCaps;
        public TMP_Dropdown Microphones;
        public TMP_Dropdown LipSyncProvider;
        public TMP_Dropdown Tracker;

        private List<OpenSeeWebcam> _cameras;

        private void Awake()
        {
            _pageViewer = GetComponentInParent<PageViewer>();
            _pageViewer.GoBackInitial.AddListener(SaveChanges);

            _cameras = OpenSeeWebcamInfo.ListCameraDetails(false);

            _cameras.ForEach(c => Cameras.options.Add(new IdDropdownData(c.name, c.id)));
            Cameras.RefreshShownValue();

            Microphones.AddOptions(Microphone.devices.ToList());

            LipSyncProvider.AddOptions(Enum.GetNames(typeof(LipSyncProvider)).ToList());

            Tracker.AddOptions(Enum.GetNames(typeof(Tracker)).ToList());

            Cameras.onValueChanged.AddListener(OnCamerasValueChanged);
            CameraCaps.onValueChanged.AddListener(OnCameraCapsValueChanged);
            Microphones.onValueChanged.AddListener(OnMicrophonesValueChanged);
            LipSyncProvider.onValueChanged.AddListener(OnLipSyncProviderValueChanged);
            Tracker.onValueChanged.AddListener(OnTrackerValueChanged);
        }

        private void Start()
        {
            Cameras.value = Cameras.options.FindIndex(o => ((IdDropdownData)o).id == ApplicationPersistence.AppSettings.Camera);
            CameraCaps.value = CameraCaps.options.FindIndex(o => ((IdDropdownData)o).id == ApplicationPersistence.AppSettings.CameraCapability);
            Microphones.value = Microphones.options.FindIndex(o => o.text == ApplicationPersistence.AppSettings.Microphone);
            LipSyncProvider.value = (int)ApplicationPersistence.AppSettings.LipSyncProvider;
            Tracker.value = (int)ApplicationPersistence.AppSettings.Tracker;
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
        }

        private void OnTrackerValueChanged(int value)
        {
            ApplicationPersistence.AppSettings.Tracker = (Tracker)value;
        }

        private void SaveChanges()
        {
            _pageViewer.GoBackInitial.RemoveListener(SaveChanges);
            ApplicationPersistence.Save();
        }

    }
}
