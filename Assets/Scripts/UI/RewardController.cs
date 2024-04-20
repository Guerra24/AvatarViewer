using System;
using System.Linq;
using SimpleFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarViewer.Ui
{
    public class RewardController : MonoBehaviour
    {
        [HideInInspector]
        public string Id;
        public Reward Reward;

        public TMP_Text Title;
        public TMP_Text Description;
        public TMP_Dropdown SpawnPoint;
        public TMP_InputField Timeout;
        public TMP_Dropdown Sound;
        public Button PickSound;
        public TMP_Text SoundPath;
        public GameObject CustomSound;
        public TMP_Dropdown Type;
        public Button PickAsset;
        public TMP_Text AssetPath;
        public GameObject Asset;

        public void Awake()
        {
            SpawnPoint.onValueChanged.AddListener(OnSpawnPointChanged);
            Timeout.onEndEdit.AddListener(OnTimeoutEditEnd);
            Sound.onValueChanged.AddListener(OnSoundChanged);
            PickSound.onClick.AddListener(OnPickSoundClick);
            Type.onValueChanged.AddListener(OnAssetTypeChanged);
            PickAsset.onClick.AddListener(OnPickAssetClick);
            SpawnPoint.AddOptions(Enum.GetValues(typeof(RewardSpawnPoint)).Cast<RewardSpawnPoint>().Select(r => r.ToString()).ToList());
            Sound.AddOptions(Enum.GetValues(typeof(AssetSound)).Cast<AssetSound>().Select(r => r.ToString()).ToList());
            Type.AddOptions(Enum.GetValues(typeof(AssetType)).Cast<AssetType>().Select(r => r.ToString()).ToList());
        }

        private void OnSpawnPointChanged(int item)
        {
            Reward.SpawnPoint = (RewardSpawnPoint)item;
        }

        private void OnTimeoutEditEnd(string value)
        {
            Reward.Timeout = float.Parse(value);
        }

        private void OnSoundChanged(int item)
        {
            Reward.Sound = (AssetSound)item;
            CustomSound.SetActive(Reward.Sound == AssetSound.Custom);
        }

        private void OnAssetTypeChanged(int item)
        {
            Reward.Type = (AssetType)item;
            Asset.SetActive(Reward.Type == AssetType.Custom);
        }

        private void OnPickAssetClick()
        {
            FileBrowser.ShowLoadDialog(AssetFilePicked, FileCancelled, FileBrowser.PickMode.Files);
        }

        private void AssetFilePicked(string[] files)
        {
            AssetPath.text = Reward.Path = files[0];
        }

        private void OnPickSoundClick()
        {
            FileBrowser.ShowLoadDialog(SoundFilePicked, FileCancelled, FileBrowser.PickMode.Files);
        }

        private void SoundFilePicked(string[] files)
        {
            SoundPath.text = Reward.SoundPath = files[0];
        }

        private void FileCancelled() { }

        public void LoadValues(string id)
        {
            Id = id;
            Title.text = Reward.Title;

            SpawnPoint.value = (int)Reward.SpawnPoint;
            Timeout.text = Reward.Timeout.ToString();
            Sound.value = (int)Reward.Sound;
            SoundPath.text = Reward.SoundPath;
            CustomSound.SetActive(Reward.Sound == AssetSound.Custom);
            Type.value = (int)Reward.Type;
            AssetPath.text = Reward.Path;
            Asset.SetActive(Reward.Type == AssetType.Custom);
        }
    }
}
