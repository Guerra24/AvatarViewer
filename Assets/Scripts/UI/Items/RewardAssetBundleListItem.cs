using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace AvatarViewer.UI.Items
{
    public class RewardAssetBundleListItem : MonoBehaviour
    {

        private Guid Guid;
        private RewardAssetsBundle Bundle;

        [SerializeField] private TMP_Text Title;
        [SerializeField] private Button Remove;
        [SerializeField] private LocalizedString DialogTitle;
        [SerializeField] private LocalizedString DialogContent;
        [SerializeField] private Dialog _dialog;

        private void Awake()
        {
            Remove.onClick.AddListener(() => RemoveBundle().Forget());
        }

        public void LoadValues(Guid guid, RewardAssetsBundle bundle)
        {
            Guid = guid;
            Bundle = bundle;
            Title.text = bundle.Path.Replace(@"\", @"\\");
        }

        private async UniTaskVoid RemoveBundle()
        {
            var loaded = ApplicationState.RewardBundles[Guid];
            var title = await DialogTitle.GetLocalizedStringAsync();
            var content = await DialogContent.GetLocalizedStringAsync();

            foreach (var reward in ApplicationPersistence.AppSettings.Rewards)
            {
                if (reward.Value is ItemReward itemReward && loaded.RewardAssets.TryGetValue(itemReward.RewardAsset, out var rewardAsset))
                {
                    var tcs = new UniTaskCompletionSource<bool>();
                    _dialog.CreateDialog(
                        title,
                        content.AsFormat(rewardAsset.AssetName, itemReward.Title),
                        () => { itemReward.RewardAsset = Guid.Parse("158dcd30-65e2-4947-84b1-6776a712a052"); tcs.TrySetResult(false); },
                        () => tcs.TrySetResult(true));
                    if (await tcs.Task)
                        return;
                }
            }

            foreach (var kvp in loaded.RewardAssets)
                ApplicationState.RewardAssets.Remove(kvp.Key);
            ApplicationPersistence.AppSettings.RewardAssetsBundles.Remove(Guid);
            await loaded.Bundle.UnloadAsync(true);
            ApplicationState.RewardBundles.Remove(Guid);
            Destroy(gameObject);
        }
    }
}
