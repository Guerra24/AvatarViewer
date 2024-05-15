using System.Linq;
using AvatarViewer;
using AvatarViewer.Twitch;
using Cysharp.Threading.Tasks;
using TwitchLib.PubSub.Events;
using UnityEngine;

public class AvatarRewardController : MonoBehaviour
{
    [SerializeField] private AvatarLoader AvatarLoader;

    private TwitchController twitchController;
    private bool running;

    private void Awake()
    {
        twitchController = GameObject.Find("TwitchController").GetComponent<TwitchController>();
        twitchController.PubSub.OnChannelPointsRewardRedeemed += PubSub_OnChannelPointsRewardRedeemed;
        twitchController.PubSub.OnRewardRedeemed += PubSub_OnRewardRedeemed;
    }

    private void PubSub_OnRewardRedeemed(object sender, OnRewardRedeemedArgs e)
    {
        if (ApplicationPersistence.AppSettings.Rewards.TryGetValue(e.RewardId.ToString(), out var r2) && r2 is PickAvatarReward pickAvatarReward)
        {
            if (running)
                return;
            try
            {
                running = true;
                var text = e.Message.Trim();
                var choice = pickAvatarReward.Choices.FirstOrDefault(c => c.Text == text);
                if (choice != null)
                {
                    if (ApplicationState.CurrentAvatar.Guid == choice.Avatar)
                        return;
                    ApplicationState.CurrentAvatar = ApplicationPersistence.AppSettings.Avatars.First(a => a.Guid == choice.Avatar);
                    MainThreadDispatcher.AddOnUpdate(() =>
                    {
                        AvatarLoader.LoadAvatar().Forget();
                        running = false;
                    });
                }
            }
            finally
            {
                running = false;
            }
        }
    }

    private void PubSub_OnChannelPointsRewardRedeemed(object sender, OnChannelPointsRewardRedeemedArgs e)
    {
        if (ApplicationPersistence.AppSettings.Rewards.TryGetValue(e.RewardRedeemed.Redemption.Reward.Id, out var r1) && r1 is AvatarReward avatarReward)
        {
            if (ApplicationState.CurrentAvatar.Guid == avatarReward.Avatar || running)
                return;
            running = true;
            ApplicationState.CurrentAvatar = ApplicationPersistence.AppSettings.Avatars.First(a => a.Guid == avatarReward.Avatar);
            MainThreadDispatcher.AddOnUpdate(() =>
            {
                AvatarLoader.LoadAvatar().Forget();
                running = false;
            });
        }
    }

    private void OnDestroy()
    {
        twitchController.PubSub.OnChannelPointsRewardRedeemed -= PubSub_OnChannelPointsRewardRedeemed;
        twitchController.PubSub.OnRewardRedeemed -= PubSub_OnRewardRedeemed;
    }

    private void OnApplicationQuit()
    {
        twitchController.PubSub.OnChannelPointsRewardRedeemed -= PubSub_OnChannelPointsRewardRedeemed;
        twitchController.PubSub.OnRewardRedeemed -= PubSub_OnRewardRedeemed;
    }
}
