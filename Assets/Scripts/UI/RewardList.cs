using AvatarViewer.UI.Items;
using AvatarViewer.UI.Settings;
using UnityEngine;

namespace AvatarViewer.UI
{
    public class RewardList : BaseSettingsPage
    {

        [SerializeField] private GameObject _templateRewardListItem;
        [SerializeField] private GameObject _templateItemRewardListItem;
        [SerializeField] private GameObject _templateCameraRewardListItem;
        [SerializeField] private GameObject _templateAvatarRewardListItem;
        [SerializeField] private GameObject _templatePickAvatarRewardListItem;

        void Start()
        {
            RefreshList();
        }

        public void RefreshList()
        {
            for (int i = 0; i < transform.childCount; i++)
                Destroy(transform.GetChild(i).gameObject);
            foreach (var reward in ApplicationPersistence.AppSettings.Rewards)
            {
                switch (reward.Value)
                {
                    case ItemReward ir:
                        CreateItemReward(reward.Key, ir);
                        break;
                    case CameraReward cr:
                        CreateCameraReward(reward.Key, cr);
                        break;
                    case AvatarReward ar:
                        CreateAvatarReward(reward.Key, ar);
                        break;
                    case PickAvatarReward par:
                        CreatePickAvatarReward(reward.Key, par);
                        break;
                    default:
                        CreateReward(reward.Key, reward.Value);
                        break;
                }
            }
        }

        public void CreateReward(string id, Reward reward)
        {
            var item = Instantiate(_templateRewardListItem, this.transform, false);
            var controller = item.GetComponent<InitialRewardListItem>();
            controller.Reward = reward;
            controller.LoadValues(id);
        }

        public void CreateItemReward(string id, ItemReward reward)
        {
            var item = Instantiate(_templateItemRewardListItem, this.transform, false);
            var controller = item.GetComponent<ItemRewardListItem>();
            controller.Reward = reward;
            controller.LoadValues(id);
        }

        public void CreateCameraReward(string id, CameraReward reward)
        {
            var item = Instantiate(_templateCameraRewardListItem, this.transform, false);
            var controller = item.GetComponent<CameraRewardListItem>();
            controller.Reward = reward;
            controller.LoadValues(id);
        }

        public void CreateAvatarReward(string id, AvatarReward reward)
        {
            var item = Instantiate(_templateAvatarRewardListItem, this.transform, false);
            var controller = item.GetComponent<AvatarRewardListItem>();
            controller.Reward = reward;
            controller.LoadValues(id);
        }

        public void CreatePickAvatarReward(string id, PickAvatarReward reward)
        {
            var item = Instantiate(_templatePickAvatarRewardListItem, this.transform, false);
            var controller = item.GetComponent<PickAvatarRewardListItem>();
            controller.Reward = reward;
            controller.LoadValues(id);
        }

    }
}
