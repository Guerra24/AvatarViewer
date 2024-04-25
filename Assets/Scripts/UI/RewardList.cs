using AvatarViewer.Ui.Settings;
using UnityEngine;

namespace AvatarViewer.Ui
{
    public class RewardList : BaseSettingsPage
    {

        public GameObject _template;

        void Start()
        {
            foreach (var reward in ApplicationPersistence.AppSettings.Rewards)
                CreateItem(reward.Key, reward.Value);
        }

        public void CreateItem(string id, Reward reward)
        {
            var item = Instantiate(_template, this.transform, false);
            var controller = item.GetComponent<RewardController>();
            controller.Reward = reward;
            controller.LoadValues(id);
        }

    }
}
