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
        [SerializeField] private TMP_Dropdown SpawnPoint;
        [SerializeField] private TMP_InputField Timeout;
        [SerializeField] private GameObject Volume;
        [SerializeField] private Button PickSound;
        [SerializeField] private TMP_Text SoundPath;
        [SerializeField] private Button ClearSound;
        [SerializeField] private TMP_Dropdown RewardAsset;

        protected override void Awake()
        {
            base.Awake();
            SpawnPoint.AddOptions(Enum.GetNames(typeof(ItemRewardSpawnPoint)).ToList());
            foreach (var rewardAsset in ApplicationState.RewardAssets)
                RewardAsset.options.Add(new GuidDropdownData(rewardAsset.Value.RewardAsset.Name, rewardAsset.Key));
            RewardAsset.RefreshShownValue();

            Volume.SetupSlider((value) => Reward.Volume = value);
            SpawnPoint.onValueChanged.AddListener(OnSpawnPointChanged);
            Timeout.onEndEdit.AddListener(OnTimeoutEditEnd);
            PickSound.onClick.AddListener(OnPickSoundClick);
            ClearSound.onClick.AddListener(() => SoundPath.text = Reward.SoundPath = "");
            RewardAsset.onValueChanged.AddListener(OnRewardAssetChanged);
        }

        private void OnSpawnPointChanged(int item)
        {
            Reward.SpawnPoint = (ItemRewardSpawnPoint)item;
        }

        private void OnTimeoutEditEnd(string value)
        {
            Reward.Timeout = float.Parse(value);
        }

        private void OnRewardAssetChanged(int item)
        {
            Reward.RewardAsset = ((GuidDropdownData)RewardAsset.options[item]).guid;
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

        public override void LoadValues(string id)
        {
            base.LoadValues(id);
            SpawnPoint.value = (int)Reward.SpawnPoint;
            Timeout.text = Reward.Timeout.ToString();
            SoundPath.text = Reward.SoundPath;
            RewardAsset.value = RewardAsset.options.FindIndex(g => ((GuidDropdownData)g).guid == Reward.RewardAsset);
            Volume.LoadSlider(Reward.Volume);
        }
    }
}
