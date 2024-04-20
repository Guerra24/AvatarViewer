using System.Linq;
using TMPro;
using UnityEngine;

namespace AvatarViewer.Ui
{
    /*
    public class SettingsMapper : MonoBehaviour
    {

        private AppSettings AppSettings;

        public TMP_Dropdown Cameras;
        public TMP_Dropdown CameraCapabilites;
        public TMP_Dropdown Microphones;
        public TMP_Dropdown Qualities;

        public void Awake()
        {
            AppSettings = ApplicationPersistence.AppSettings;
        }

        public string ListenAddress
        {
            get => AppSettings.ListenAddress;
            set
            {
                AppSettings.ListenAddress = value;
                ApplicationPersistence.Save();
            }
        }

        public string Port
        {
            get => AppSettings.Port.ToString();
            set
            {
                AppSettings.Port = int.Parse(value);
                ApplicationPersistence.Save();
            }
        }

        public bool UseLocalTracker
        {
            get => AppSettings.UseLocalTracker;
            set
            {
                AppSettings.UseLocalTracker = value;
                ApplicationPersistence.Save();
            }
        }

        public int Camera
        {
            get => AppSettings.Camera;
            set
            {
                AppSettings.Camera = ((IdDropdownData)Cameras.options[value]).id;
                ApplicationPersistence.Save();
            }
        }

        public int CameraCapability
        {
            get => AppSettings.CameraCapability;
            set
            {
                AppSettings.CameraCapability = ((IdDropdownData)CameraCapabilites.options[value]).id;
                ApplicationPersistence.Save();
            }
        }

        public int Microphone
        {
            get
            {
                return UnityEngine.Microphone.devices.ToList().FindIndex(m => m == AppSettings.Microphone);
            }
            set
            {
                AppSettings.Microphone = UnityEngine.Microphone.devices[value];
                ApplicationPersistence.Save();
            }
        }

        public int Quality
        {
            get => AppSettings.Quality;
            set
            {
                AppSettings.Quality = ((IdDropdownData)Qualities.options[value]).id;
                ApplicationPersistence.Save();
            }
        }

        public T GetProperty<T>(string property)
        {
            return (T)GetType().GetProperty(property).GetValue(this);
        }
        public object GetProperty(string property)
        {
            return GetType().GetProperty(property).GetValue(this);
        }
    }
    */
}
