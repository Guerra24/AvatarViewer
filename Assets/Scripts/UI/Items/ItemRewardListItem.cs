using System;
using System.Linq;
using SimpleFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarViewer.Ui.Items
{
    public class ItemRewardListItem : RewardListItem<ItemReward>
    {
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

        protected override void Awake()
        {
            base.Awake();
            SpawnPoint.onValueChanged.AddListener(OnSpawnPointChanged);
            Timeout.onEndEdit.AddListener(OnTimeoutEditEnd);
            Sound.onValueChanged.AddListener(OnSoundChanged);
            PickSound.onClick.AddListener(OnPickSoundClick);
            Type.onValueChanged.AddListener(OnAssetTypeChanged);
            PickAsset.onClick.AddListener(OnPickAssetClick);
            SpawnPoint.AddOptions(Enum.GetNames(typeof(ItemRewardSpawnPoint)).ToList());
            Sound.AddOptions(Enum.GetNames(typeof(ItemRewardSound)).ToList());
            Type.AddOptions(Enum.GetNames(typeof(ItemRewardAsset)).ToList());
        }

        private void OnSpawnPointChanged(int item)
        {
            Reward.SpawnPoint = (ItemRewardSpawnPoint)item;
        }

        private void OnTimeoutEditEnd(string value)
        {
            Reward.Timeout = float.Parse(value);
        }

        private void OnSoundChanged(int item)
        {
            Reward.Sound = (ItemRewardSound)item;
            CustomSound.SetActive(Reward.Sound == ItemRewardSound.Custom);
        }

        private void OnAssetTypeChanged(int item)
        {
            Reward.Asset = (ItemRewardAsset)item;
            Asset.SetActive(Reward.Asset == ItemRewardAsset.Custom);
        }

        private void OnPickAssetClick()
        {
            FileBrowser.ShowLoadDialog(AssetFilePicked, FileCancelled, FileBrowser.PickMode.Files);
        }

        private void AssetFilePicked(string[] files)
        {
            AssetPath.text = Reward.AssetPath = files[0];
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

        public override void LoadValues(string id)
        {
            base.LoadValues(id);
            SpawnPoint.value = (int)Reward.SpawnPoint;
            Timeout.text = Reward.Timeout.ToString();
            Sound.value = (int)Reward.Sound;
            SoundPath.text = Reward.SoundPath;
            CustomSound.SetActive(Reward.Sound == ItemRewardSound.Custom);
            Type.value = (int)Reward.Asset;
            AssetPath.text = Reward.AssetPath;
            Asset.SetActive(Reward.Asset == ItemRewardAsset.Custom);
        }
    }
}
