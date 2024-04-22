using System;
using Cysharp.Threading.Tasks;
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

        public GameObject Dialog;

        void Awake()
        {
            PubSub = new();
            Manager = new();
            PubSub.OnPubSubServiceConnected += PubSub_OnPubSubServiceConnected;
            PubSub.OnChannelPointsRewardRedeemed += PubSub_OnChannelPointsRewardRedeemed;
        }

        public void Init()
        {
            PubSub.ListenToChannelPoints(PlayerPrefs.GetString("UserId"));
            PubSub.Connect();
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
            while (true)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(30));
                Debug.Log("Validating twitch token");
                if (!await Manager.ValidateToken())
                {
                    Debug.Log("Invalid token");
                    PubSub.Disconnect();
                    PlayerPrefs.SetString("TwitchAccessToken", "");
                    PlayerPrefs.SetString("UserId", "");

                    var dialog = Instantiate(Dialog, GameObject.Find("Canvas").transform, false);
                    var data = dialog.GetComponent<Dialog>();
                    data.SetTitle("Authenticate");
                    data.SetContent("Twitch token became invalid; please authenticate again.\nThis will open the main menu.");
                    data.SetOnOkAction(() => SceneManager.LoadScene("Scenes/TwitchAuth", LoadSceneMode.Single));
                    data.SetOnCancelAction(() => Destroy(dialog));
                    return;
                }
            }
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
