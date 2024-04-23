using System;
using OpenSee;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AvatarViewer.Ui
{
    public class MainController : MonoBehaviour
    {

        public OpenSeeIKTarget IKTarget;

        public TMP_Dropdown CameraPresets;
        public Button AddPreset;
        public Button Save;

        public Slider RotX;
        public Slider RotY;
        public Slider RotZ;

        public TMP_InputField PosX;
        public TMP_InputField PosY;
        public TMP_InputField PosZ;

        public Slider FOV;

        public Toggle Absolute;

        public Camera Camera;

        private CameraPreset CurrentCameraPreset;

        private void Awake()
        {
            ReloadCameraPresets();
            CameraPresets.onValueChanged.AddListener(OnCameraPresetChanged);
            AddPreset.onClick.AddListener(OnAddPresetClick);
            Save.onClick.AddListener(OnSaveClick);
            RotX.onValueChanged.AddListener(OnRotationValueChanged);
            RotY.onValueChanged.AddListener(OnRotationValueChanged);
            RotZ.onValueChanged.AddListener(OnRotationValueChanged);
            PosX.onValueChanged.AddListener(OnPositionValueChanged);
            PosY.onValueChanged.AddListener(OnPositionValueChanged);
            PosZ.onValueChanged.AddListener(OnPositionValueChanged);
            FOV.onValueChanged.AddListener(OnFOVValueChanged);
            Absolute.onValueChanged.AddListener(OnAbsoluteValueChanged);
        }

        private void Start()
        {
            var preset = ApplicationPersistence.AppSettings.DefaultCameraPreset;
            CameraPresets.value = CameraPresets.options.FindIndex(g => ((GuidDropdownData)g).guid == preset);
            CurrentCameraPreset = ApplicationPersistence.AppSettings.CameraPresets[preset];
            LoadPreset();
        }

        private void OnCameraPresetChanged(int value)
        {
            CurrentCameraPreset = ApplicationPersistence.AppSettings.CameraPresets[((GuidDropdownData)CameraPresets.options[value]).guid];
            if (CurrentCameraPreset.Absolute)
                Camera.transform.position = CurrentCameraPreset.Position;
            else
                Camera.transform.localPosition = CurrentCameraPreset.Position;
            Camera.transform.localRotation = CurrentCameraPreset.Rotation;
            LoadPreset();
        }

        private void OnAddPresetClick()
        {
            ApplicationPersistence.AppSettings.CameraPresets.Add(Guid.NewGuid(), new CameraPreset { Name = "New preset" });
            ReloadCameraPresets();
        }

        public void OnSaveClick()
        {
            ApplicationPersistence.Save();
        }

        private void OnRotationValueChanged(float _)
        {
            CurrentCameraPreset.Rotation = Camera.transform.localRotation = Quaternion.Euler(RotX.value, RotY.value, RotZ.value);
        }

        private void OnPositionValueChanged(string _)
        {
            var pos = CurrentCameraPreset.Position;
            if (float.TryParse(PosX.text, out var x))
                pos.x = x;
            if (float.TryParse(PosY.text, out var y))
                pos.y = y;
            if (float.TryParse(PosZ.text, out var z))
                pos.z = z;
            if (CurrentCameraPreset.Absolute)
                CurrentCameraPreset.Position = Camera.transform.position = pos;
            else
                CurrentCameraPreset.Position = Camera.transform.localPosition = pos;
        }

        private void OnFOVValueChanged(float value)
        {
            CurrentCameraPreset.FOV = Camera.fieldOfView = value;
        }

        private void OnAbsoluteValueChanged(bool state)
        {
            CurrentCameraPreset.Absolute = state;
            if (state)
            {
                CurrentCameraPreset.Position = Camera.transform.position;
            }
            else
            {
                CurrentCameraPreset.Position = Camera.transform.localPosition;
            }
            var pos = CurrentCameraPreset.Position;
            PosX.text = pos.x.ToString();
            PosY.text = pos.y.ToString();
            PosZ.text = pos.z.ToString();
        }

        public void Back()
        {
            SceneManager.LoadScene("Scenes/Menu");
        }

        public void ResetAvatar()
        {
            IKTarget.calibrate = true;
        }

        private void LoadPreset()
        {
            var angles = CurrentCameraPreset.Rotation.eulerAngles;
            RotX.value = angles.x > 180 ? angles.x - 360 : angles.x;
            RotY.value = angles.y > 180 ? angles.y - 360 : angles.y;
            RotZ.value = angles.z > 180 ? angles.z - 360 : angles.z;

            var pos = CurrentCameraPreset.Position;
            PosX.text = pos.x.ToString();
            PosY.text = pos.y.ToString();
            PosZ.text = pos.z.ToString();

            FOV.value = CurrentCameraPreset.FOV;
            Absolute.isOn = CurrentCameraPreset.Absolute;
        }

        private void ReloadCameraPresets()
        {
            CameraPresets.ClearOptions();
            foreach (var preset in ApplicationPersistence.AppSettings.CameraPresets)
            {
                CameraPresets.options.Add(new GuidDropdownData(preset.Value.Name, preset.Key));
            }
            CameraPresets.RefreshShownValue();
        }
    }

}
