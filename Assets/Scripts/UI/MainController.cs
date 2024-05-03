using System;
using Cysharp.Threading.Tasks;
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
        public TMP_InputField PresetName;
        public Button AddPreset;
        public Button RemovePreset;
        public Button Save;

        public Slider SRotX;
        public Slider SRotY;
        public Slider SRotZ;
        public Slider SPosX;
        public Slider SPosY;
        public Slider SPosZ;

        public TMP_InputField IRotX;
        public TMP_InputField IRotY;
        public TMP_InputField IRotZ;
        public TMP_InputField IPosX;
        public TMP_InputField IPosY;
        public TMP_InputField IPosZ;

        public Slider SFOV;
        public TMP_InputField IFOV;

        public Toggle Absolute;

        public Camera Camera;

        public GameObject Dialog;

        private float RotX, RotY, RotZ, PosX, PosY, PosZ, FOV;

        public CameraPreset CurrentCameraPreset { get; private set; }

        private void Awake()
        {
            ReloadCameraPresets();
            CameraPresets.onValueChanged.AddListener(OnCameraPresetChanged);
            PresetName.onEndEdit.AddListener(OnPresetNameEndEdit);
            AddPreset.onClick.AddListener(OnAddPresetClick);
            RemovePreset.onClick.AddListener(OnRemovePresetClick);
            Save.onClick.AddListener(OnSaveClick);

            IRotX.onValueChanged.AddListener(OnRotationValueChanged);
            IRotY.onValueChanged.AddListener(OnRotationValueChanged);
            IRotZ.onValueChanged.AddListener(OnRotationValueChanged);

            IRotX.onEndEdit.AddListener(OnRotationEndEdit);
            IRotY.onEndEdit.AddListener(OnRotationEndEdit);
            IRotZ.onEndEdit.AddListener(OnRotationEndEdit);

            IPosX.onValueChanged.AddListener(OnPositionValueChanged);
            IPosY.onValueChanged.AddListener(OnPositionValueChanged);
            IPosZ.onValueChanged.AddListener(OnPositionValueChanged);

            IPosX.onEndEdit.AddListener(OnPositionEndEdit);
            IPosY.onEndEdit.AddListener(OnPositionEndEdit);
            IPosZ.onEndEdit.AddListener(OnPositionEndEdit);

            IFOV.onValueChanged.AddListener(OnFOVValueChanged);

            IFOV.onEndEdit.AddListener(OnFOVEndEdit);

            Absolute.onValueChanged.AddListener(OnAbsoluteValueChanged);
        }

        private void Start()
        {
            LoadPreset(ApplicationPersistence.AppSettings.DefaultCameraPreset);
        }

        private void Update()
        {
            if (SRotX.value != 0)
                IRotX.text = (RotX += SRotX.value * 6 * Time.deltaTime).ToString();
            if (SRotY.value != 0)
                IRotY.text = (RotY += SRotY.value * 6 * Time.deltaTime).ToString();
            if (SRotZ.value != 0)
                IRotZ.text = (RotZ += SRotZ.value * 8 * Time.deltaTime).ToString();

            if (SPosX.value != 0)
                IPosX.text = (PosX += SPosX.value * Time.deltaTime).ToString();
            if (SPosY.value != 0)
                IPosY.text = (PosY += SPosY.value * Time.deltaTime).ToString();
            if (SPosZ.value != 0)
                IPosZ.text = (PosZ += SPosZ.value * Time.deltaTime).ToString();

            if (SFOV.value != 0)
                IFOV.text = (FOV += SFOV.value * 6 * Time.deltaTime).ToString();
#if false
            Debug.Log($"D: {Texture.desiredTextureMemory / 1024 / 1024} L: {Texture.totalTextureMemory / 1024 / 1024}");
#endif
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

        private void OnPresetNameEndEdit(string text)
        {
            CurrentCameraPreset.Name = text;
        }

        private void OnAddPresetClick()
        {
            var preset = Guid.NewGuid();
            ApplicationPersistence.AppSettings.CameraPresets.Add(preset, new CameraPreset { Name = "New preset" });
            ReloadCameraPresets();
            LoadPreset(preset);
        }

        private void OnRemovePresetClick()
        {
            var preset = ((GuidDropdownData)CameraPresets.options[CameraPresets.value]).guid;

            var dialog = Instantiate(Dialog, GameObject.Find("Canvas").transform, false);
            var data = dialog.GetComponentInChildren<Dialog>();
            data.SetTitle("Delete camera preset");
            data.SetContent($"{ApplicationPersistence.AppSettings.CameraPresets[preset].Name}");
            data.SetOnOkAction(() =>
            {
                ApplicationPersistence.AppSettings.CameraPresets.Remove(preset);
                ReloadCameraPresets();
                LoadPreset(ApplicationPersistence.AppSettings.DefaultCameraPreset);
                ApplicationPersistence.Save();
            });
        }

        public void OnSaveClick()
        {
            var preset = ((GuidDropdownData)CameraPresets.options[CameraPresets.value]).guid;
            ApplicationPersistence.Save();
            ReloadCameraPresets();
            LoadPreset(preset);
        }

        private void OnRotationEndEdit(string _)
        {
            RotX = float.Parse(IRotX.text);
            RotY = float.Parse(IRotY.text);
            RotZ = float.Parse(IRotZ.text);
            CurrentCameraPreset.Rotation = Camera.transform.localRotation = Quaternion.Euler(RotX, RotY, RotZ);
        }

        private void OnRotationValueChanged(string _)
        {
            CurrentCameraPreset.Rotation = Camera.transform.localRotation = Quaternion.Euler(RotX, RotY, RotZ);
        }

        private void OnPositionEndEdit(string _)
        {
            var pos = CurrentCameraPreset.Position;
            pos.x = PosX = float.Parse(IPosX.text);
            pos.y = PosY = float.Parse(IPosY.text);
            pos.z = PosZ = float.Parse(IPosZ.text);
            if (CurrentCameraPreset.Absolute)
                CurrentCameraPreset.Position = Camera.transform.position = pos;
            else
                CurrentCameraPreset.Position = Camera.transform.localPosition = pos;
        }

        private void OnPositionValueChanged(string _)
        {
            var pos = CurrentCameraPreset.Position;
            pos.x = PosX;
            pos.y = PosY;
            pos.z = PosZ;
            if (CurrentCameraPreset.Absolute)
                CurrentCameraPreset.Position = Camera.transform.position = pos;
            else
                CurrentCameraPreset.Position = Camera.transform.localPosition = pos;
        }

        private void OnFOVEndEdit(string _)
        {
            CurrentCameraPreset.FOV = FOV = float.Parse(IFOV.text);

            float _1OverAspect = 1f / Camera.aspect;
            Camera.fieldOfView = 2f * Mathf.Atan(Mathf.Tan(FOV * Mathf.Deg2Rad * 0.5f) * _1OverAspect) * Mathf.Rad2Deg;
        }

        private void OnFOVValueChanged(string _)
        {
            CurrentCameraPreset.FOV = FOV;

            float _1OverAspect = 1f / Camera.aspect;
            Camera.fieldOfView = 2f * Mathf.Atan(Mathf.Tan(FOV * Mathf.Deg2Rad * 0.5f) * _1OverAspect) * Mathf.Rad2Deg;
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
            PosX = pos.x;
            PosY = pos.y;
            PosZ = pos.z;
            IPosX.text = PosX.ToString();
            IPosY.text = PosY.ToString();
            IPosZ.text = PosZ.ToString();
        }

        public void Back()
        {
            BackAsync().Forget();
        }

        private async UniTaskVoid BackAsync()
        {
            SceneLoader.Scene = "Scenes/Menu";
            await SceneManager.LoadSceneAsync("Scenes/Loader");
        }

        public void ResetAvatar()
        {
            IKTarget.calibrate = true;
        }

        private void LoadPreset(Guid preset)
        {
            CameraPresets.value = CameraPresets.options.FindIndex(g => ((GuidDropdownData)g).guid == preset);
            CurrentCameraPreset = ApplicationPersistence.AppSettings.CameraPresets[preset];
            LoadPreset();
        }

        private void LoadPreset()
        {
            var angles = CurrentCameraPreset.Rotation.eulerAngles;
            RotX = angles.x > 180 ? angles.x - 360 : angles.x;
            RotY = angles.y > 180 ? angles.y - 360 : angles.y;
            RotZ = angles.z > 180 ? angles.z - 360 : angles.z;

            IRotX.text = RotX.ToString();
            IRotY.text = RotY.ToString();
            IRotZ.text = RotZ.ToString();

            var pos = CurrentCameraPreset.Position;
            PosX = pos.x;
            PosY = pos.y;
            PosZ = pos.z;

            IPosX.text = PosX.ToString();
            IPosY.text = PosY.ToString();
            IPosZ.text = PosZ.ToString();

            FOV = CurrentCameraPreset.FOV;

            IFOV.text = FOV.ToString();
            Absolute.isOn = CurrentCameraPreset.Absolute;
            PresetName.text = CurrentCameraPreset.Name;
        }

        private void ReloadCameraPresets()
        {
            CameraPresets.ClearOptions();
            foreach (var preset in ApplicationPersistence.AppSettings.CameraPresets)
                CameraPresets.options.Add(new GuidDropdownData(preset.Value.Name, preset.Key));
            CameraPresets.RefreshShownValue();
        }
    }

}
