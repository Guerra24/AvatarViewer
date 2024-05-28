using System;
using System.Collections.Generic;
using System.Linq;
using AvatarViewer.SDK;
using AvatarViewer.UI.Items;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AvatarViewer.UI.Settings
{
    public class RewardAssetBundlesPage : BaseSettingsPage
    {

        [SerializeField] private GameObject _template;

        protected override void Awake()
        {
            base.Awake();
            foreach (var kvp in ApplicationPersistence.AppSettings.RewardAssetsBundles)
                CreateItem(kvp.Key, kvp.Value);
        }

        private void CreateItem(Guid guid, RewardAssetsBundle bundle)
        {
            var item = Instantiate(_template, transform, false);
            item.GetComponent<RewardAssetBundleListItem>().LoadValues(guid, bundle);
        }

        public void AddBundle()
        {
            var res = NativeFileDialogSharp.Dialog.FileOpen("avr");
            if (res.IsOk)
                AddBundle(res.Path).Forget();
        }

        private async UniTaskVoid AddBundle(string path)
        {
            if (ApplicationPersistence.AppSettings.RewardAssetsBundles.Any(kvp => kvp.Value.Path == path))
                return;
            var bundle = await AssetBundle.LoadFromFileAsync(path);
            var request = bundle.LoadAllAssetsAsync<GameObject>();
            await request;
            var rewardAssets = new Dictionary<Guid, RewardAssetInfo>();
            foreach (var @object in request.allAssets)
            {
                var reward = @object as RewardAssetInfo;
                ApplicationState.RewardAssets.Add(reward.Guid, reward);
                rewardAssets.Add(reward.Guid, reward);
            }
            var rab = new RewardAssetsBundle(path);
            var guid = Guid.NewGuid();
            ApplicationPersistence.AppSettings.RewardAssetsBundles.Add(guid, rab);
            ApplicationState.RewardBundles.Add(guid, new LoadedRewardAssetsBundle(bundle, rewardAssets));

            CreateItem(guid, rab);
        }
    }
}
