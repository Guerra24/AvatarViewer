using AvatarViewer.Twitch;
using TwitchLib.PubSub.Events;
using UnityEngine;

namespace AvatarViewer.Ui
{
    public class RewardList : MonoBehaviour
    {

        public GameObject _template;

        private PageViewer _pageViewer;
        private TwitchController _twitch;

        void Start()
        {
            foreach (var reward in ApplicationPersistence.AppSettings.Rewards)
            {
                CreateItem(reward.Key, reward.Value);
            }
            _pageViewer = GetComponentInParent<PageViewer>();
            _pageViewer.GoBackInitial.AddListener(SaveChanges);
            _twitch = GameObject.Find("TwitchController").GetComponent<TwitchController>();
            _twitch.PubSub.OnChannelPointsRewardRedeemed += PubSub_OnChannelPointsRewardRedeemed;
        }

        private void PubSub_OnChannelPointsRewardRedeemed(object sender, OnChannelPointsRewardRedeemedArgs e)
        {
            if (ApplicationPersistence.AppSettings.Rewards.TryGetValue(e.RewardRedeemed.Redemption.Reward.Id, out var reward))
            {
                reward.Title = e.RewardRedeemed.Redemption.Reward.Title;
            }
            else
            {
                var id = e.RewardRedeemed.Redemption.Reward.Id;
                var r = new Reward
                {
                    Title = e.RewardRedeemed.Redemption.Reward.Title
                };
                ApplicationPersistence.AppSettings.Rewards.Add(id, r);
                MainThreadDispatcher.AddOnUpdate(() => CreateItem(id, r));
            }
        }

        public void CreateItem(string id, Reward reward)
        {
            var item = Instantiate(_template, this.transform, false);
            var controller = item.GetComponent<RewardController>();
            controller.Reward = reward;
            controller.LoadValues(id);
        }

        private void SaveChanges()
        {
            _pageViewer.GoBackInitial.RemoveListener(SaveChanges);
            ApplicationPersistence.Save();
        }

        private void OnDestroy()
        {
            _twitch.PubSub.OnChannelPointsRewardRedeemed -= PubSub_OnChannelPointsRewardRedeemed;
        }

        private void OnApplicationQuit()
        {
            _twitch.PubSub.OnChannelPointsRewardRedeemed -= PubSub_OnChannelPointsRewardRedeemed;
        }
    }
}
