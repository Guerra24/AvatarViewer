using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace AvatarViewer.Ui.Items
{
    public class ItemRewardListItem : RewardListItem<ItemReward>
    {
        [SerializeField]
        private TMP_Dropdown SpawnPoint;
        [SerializeField]
        private TMP_InputField Timeout;
        [SerializeField]
        private TMP_Dropdown Sound;
        [SerializeField]
        private GameObject Volume;
        [SerializeField]
        private Button PickSound;
        [SerializeField]
        private TMP_Text SoundPath;
        [SerializeField]
        private GameObject CustomSound;
        [SerializeField]
        private TMP_Dropdown Type;
        [SerializeField]
        private Button PickAsset;
        [SerializeField]
        private TMP_Text AssetPath;
        [SerializeField]
        private GameObject Asset;

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
            Volume.SetupSlider((value) => Reward.Volume = value);
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
            var result = NativeFileDialogSharp.Dialog.FileOpen();
            if (result.IsOk)
                AssetPath.text = Reward.AssetPath = result.Path;
        }

        private void OnPickSoundClick()
        {
            var result = NativeFileDialogSharp.Dialog.FileOpen("wav");
            if (result.IsOk)
                SoundFilePickedAsync(result.Path).Forget();

        }

        private async UniTaskVoid SoundFilePickedAsync(string file)
        {
            if (ApplicationState.ExternalAudios.ContainsKey(file))
            {
                SoundPath.text = Reward.SoundPath = file;
            }
            else
            {
                using var request = UnityWebRequestMultimedia.GetAudioClip($"file://{file}", AudioType.WAV);
                await request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.Success)
                {
                    SoundPath.text = Reward.SoundPath = file;
                    ApplicationState.ExternalAudios.Add(Reward.SoundPath, DownloadHandlerAudioClip.GetContent(request));
                }
            }
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
            Volume.LoadSlider(Reward.Volume);
        }
    }
}
