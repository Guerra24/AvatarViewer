using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using TwitchLib.PubSub.Events;
using TwitchLib.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AvatarViewer.Twitch
{
    public class TwitchController : MonoBehaviour
    {
        public PubSub PubSub { get; private set; }
        public TwitchManager Manager { get; private set; }

        public User User { get; private set; }

        public bool IsAccountConnected { get; private set; }

        public GameObject Dialog;

        private TwitchAPI TwitchAPI;

        void Awake()
        {
            PubSub = new();
            Manager = new();
            TwitchAPI = new();
            TwitchAPI.Settings.ClientId = Manager.ClientId;
            PubSub.OnPubSubServiceConnected += PubSub_OnPubSubServiceConnected;
            PubSub.OnChannelPointsRewardRedeemed += PubSub_OnChannelPointsRewardRedeemed;
        }

        public async Task Init()
        {
            TwitchAPI.Settings.AccessToken = PlayerPrefs.GetString("TwitchAccessToken");

            User = (await TwitchAPI.Helix.Users.GetUsersAsync(new List<string> { PlayerPrefs.GetString("UserId") })).Users[0];

            PubSub.ListenToChannelPoints(PlayerPrefs.GetString("UserId"));
            PubSub.Connect();
            IsAccountConnected = true;
            StartTokenValidation().Forget();
        }

        private void PubSub_OnPubSubServiceConnected(object sender, EventArgs e)
        {
            Debug.Log("Connected");
            PubSub.SendTopics(PlayerPrefs.GetString("TwitchAccessToken"));
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

        public void Disconnect()
        {
            IsAccountConnected = false;
            PubSub.Disconnect();
            TwitchAPI.Settings.AccessToken = "";
            User = null;
            PlayerPrefs.SetString("TwitchAccessToken", "");
            PlayerPrefs.SetString("UserId", "");
            PlayerPrefs.Save();
        }

        private void OnDestroy() => PubSub.Disconnect();

        private void OnApplicationQuit() => PubSub.Disconnect();
    }
}