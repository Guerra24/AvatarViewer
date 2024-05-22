using AvatarViewer.SDK;
using AvatarViewer.Ui.Items;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AvatarViewer.Ui.Settings
{
    public class RewardAssetsPage : BaseSettingsPage
    {

        [SerializeField] private GameObject _template;

        protected override void Awake()
        {
            base.Awake();
            foreach (var bundle in ApplicationPersistence.AppSettings.RewardBundles)
                CreateItem(bundle);
        }

        private void CreateItem(string bundle)
        {
            var item = Instantiate(_template, transform, false);
            item.GetComponent<RewardAssetListItem>().LoadValues(bundle);
        }

        public void AddBundle()
        {
            var res = NativeFileDialogSharp.Dialog.FileOpen("avr");
            if (res.IsOk)
                AddBundle(res.Path).Forget();
        }

        private async UniTaskVoid AddBundle(string path)
        {
            if (ApplicationPersistence.AppSettings.RewardBundles.Contains(path))
                return;
            var bundle = await AssetBundle.LoadFromFileAsync(path);
            var request = bundle.LoadAllAssetsAsync<GameObject>();
            await request;
            foreach (var @object in request.allAssets)
            {
                var reward = @object as GameObject;
                var rewardAsset = reward.GetComponent<RewardAsset>();
                ApplicationState.RewardAssets.Add(rewardAsset.Guid, new LoadedRewardAsset(reward, rewardAsset));
            }
            ApplicationState.RewardBundles.Add(path, bundle);
            ApplicationPersistence.AppSettings.RewardBundles.Add(path);

            CreateItem(path);
        }
    }
}
