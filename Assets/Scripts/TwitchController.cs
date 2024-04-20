using System;
using TwitchLib.PubSub.Events;
using TwitchLib.Unity;
using UnityEngine;

namespace AvatarViewer
{
    public class TwitchController : MonoBehaviour
    {
        public PubSub PubSub { get; private set; }

        void Awake()
        {
            PubSub = new();
            PubSub.OnPubSubServiceConnected += PubSub_OnPubSubServiceConnected;
            PubSub.OnChannelPointsRewardRedeemed += PubSub_OnChannelPointsRewardRedeemed;
            PubSub.ListenToChannelPoints("49547647");
            PubSub.Connect();
        }


        private void PubSub_OnPubSubServiceConnected(object sender, EventArgs e)
        {
            Debug.Log("Connected");
            PubSub.SendTopics(Secrets.ACCESS_TOKEN);
        }

        private void PubSub_OnChannelPointsRewardRedeemed(object sender, OnChannelPointsRewardRedeemedArgs e)
        {
            Debug.Log($"{e.RewardRedeemed.Redemption.User.DisplayName} {e.RewardRedeemed.Redemption.Reward.Id}");
        }

        private void OnDestroy()
        {
            PubSub.Disconnect();
        }

        private void OnApplicationQuit()
        {
            PubSub.Disconnect();
        }
    }
}
