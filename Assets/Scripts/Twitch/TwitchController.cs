using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using TwitchLib.PubSub.Events;
using TwitchLib.Unity;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace AvatarViewer.Twitch
{
    public class TwitchController : MonoBehaviour
    {
        public PubSub PubSub { get; private set; }
        public TwitchManager Manager { get; private set; }

        public User User { get; private set; }

        public bool IsAccountConnected { get; private set; }

        public Sprite ProfileImage { get; private set; }

        public GameObject Dialog;

        private TwitchAPI TwitchAPI;
        private Texture2D ProfileImageTexture;

        private string UserId = "";

        void Awake()
        {
            PubSub = new();
            Manager = new();
            TwitchAPI = new();
            TwitchAPI.Settings.ClientId = Manager.ClientId;
            PubSub.OnPubSubServiceConnected += PubSub_OnPubSubServiceConnected;
            PubSub.OnChannelPointsRewardRedeemed += PubSub_OnChannelPointsRewardRedeemed;
            PubSub.OnPubSubServiceClosed += PubSub_OnPubSubServiceClosed;
            PubSub.OnPubSubServiceError += PubSub_OnPubSubServiceError;
        }

        public async UniTask Init()
        {
            UserId = PlayerPrefs.GetString("UserId");
            TwitchAPI.Settings.AccessToken = PlayerPrefs.GetString("TwitchAccessToken");

            await UniTask.SwitchToThreadPool();

            User = (await TwitchAPI.Helix.Users.GetUsersAsync(new List<string> { UserId })).Users[0];

            await UniTask.SwitchToMainThread();

            using var request = UnityWebRequestTexture.GetTexture(User.ProfileImageUrl);
            await request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                ProfileImageTexture = DownloadHandlerTexture.GetContent(request);
                ProfileImage = Sprite.Create(ProfileImageTexture, new Rect(0, 0, ProfileImageTexture.width, ProfileImageTexture.height), new Vector2(0.5f, 0.5f), 100, 1, SpriteMeshType.FullRect);
            }

            await UpdateRewards();

            await UniTask.SwitchToThreadPool();

            PubSub.ListenToChannelPoints(UserId);
            PubSub.ListenToRewards(UserId);
            PubSub.Connect();
            IsAccountConnected = true;

            await UniTask.SwitchToMainThread();

            StartTokenValidation().Forget();
        }

        private void PubSub_OnPubSubServiceConnected(object sender, EventArgs e)
        {
            Debug.Log("Connected");
            PubSub.SendTopics(PlayerPrefs.GetString("TwitchAccessToken"));
        }

        private void PubSub_OnPubSubServiceClosed(object sender, EventArgs e)
        {
            Debug.Log("Disconnected");
        }

        private async void PubSub_OnPubSubServiceError(object sender, OnPubSubServiceErrorArgs e)
        {
            Debug.LogException(e.Exception);
            await Task.Delay(10000);
            PubSub.Connect();
        }

        private void PubSub_OnChannelPointsRewardRedeemed(object sender, OnChannelPointsRewardRedeemedArgs e)
        {
            Debug.Log($"{e.RewardRedeemed.Redemption.User.DisplayName} {e.RewardRedeemed.Redemption.Reward.Id}");
        }

        public async UniTaskVoid StartTokenValidation()
        {
            while (IsAccountConnected)
            {
                await UniTask.Delay(TimeSpan.FromHours(1));
                Debug.Log("Validating twitch token");
                if (!await Manager.ValidateToken())
                {
                    Debug.Log("Invalid token");
                    Disconnect();

                    var dialog = Instantiate(Dialog, GameObject.Find("Canvas").transform, false);
                    var data = dialog.GetComponentInChildren<Dialog>();
                    data.SetTitle("Authenticate");
                    data.SetContent("Twitch token became invalid; please authenticate again.\nThis will open the main menu.");
                    data.SetOnOkAction(() => SceneManager.LoadScene("Scenes/TwitchAuth", LoadSceneMode.Single));
                    return;
                }
            }
        }

        public async Task UpdateRewards()
        {
            await UniTask.SwitchToThreadPool();

            var rewards = await TwitchAPI.Helix.ChannelPoints.GetCustomReward(UserId);

            await UniTask.SwitchToMainThread();

            foreach (var twitchReward in rewards.Data)
            {
                if (ApplicationPersistence.AppSettings.Rewards.TryGetValue(twitchReward.Id, out var reward))
                {
                    reward.Title = twitchReward.Title;
                }
                else
                {
                    reward = new Reward
                    {
                        Title = twitchReward.Title,
                    };
                    ApplicationPersistence.AppSettings.Rewards.Add(twitchReward.Id, reward);
                }
                using var request = UnityWebRequestTexture.GetTexture(twitchReward.Image?.Url1x ?? twitchReward.DefaultImage?.Url1x ?? "https://static-cdn.jtvnw.net/custom-reward-images/default-1.png");
                await request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.Success)
                {
                    var texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                    reward.TwitchImage = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100, 1, SpriteMeshType.FullRect);
                }
            }

            foreach (var missmatch in ApplicationPersistence.AppSettings.Rewards.Keys.Except(rewards.Data.Select(d => d.Id).ToList()).ToList())
                ApplicationPersistence.AppSettings.Rewards.Remove(missmatch);

            ApplicationPersistence.Save();
        }

        public void Disconnect()
        {
            IsAccountConnected = false;
            PubSub.Disconnect();
            TwitchAPI.Settings.AccessToken = "";
            User = null;
            UserId = "";
            ClearProfileImage();
            PlayerPrefs.SetString("TwitchAccessToken", "");
            PlayerPrefs.SetString("UserId", "");
            PlayerPrefs.Save();
        }

        private void ClearProfileImage()
        {
            if (ProfileImage != null)
            {
                Destroy(ProfileImage);
                Destroy(ProfileImageTexture);
                ProfileImage = null;
            }
        }

        private void OnDestroy()
        {
            ClearProfileImage();
            PubSub.Disconnect();
        }

        private void OnApplicationQuit()
        {
            ClearProfileImage();
            PubSub.Disconnect();
        }
    }
}
