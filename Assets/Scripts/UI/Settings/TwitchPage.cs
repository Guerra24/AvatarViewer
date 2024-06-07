using AvatarViewer.Twitch;
using AvatarViewer.UI.Animations;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AvatarViewer.UI.Settings
{
    public class TwitchPage : BaseSettingsPage
    {
        [SerializeField] private Dialog _dialog;

        [SerializeField] private GameObject ConnectAccount;
        [SerializeField] private GameObject CurrentAccount;

        [SerializeField] private TMP_Text DisplayName;
        [SerializeField] private Image ProfileImage;

        private void Start()
        {
            ConnectAccount.SetActive(!TwitchManager.Instance.IsAccountConnected);
            CurrentAccount.SetActive(TwitchManager.Instance.IsAccountConnected);
            if (TwitchManager.Instance.IsAccountConnected)
                LoadTwitchAccount();
        }

        private void LoadTwitchAccount()
        {
            var user = TwitchManager.Instance.User;
            DisplayName.text = user.DisplayName;
            ProfileImage.sprite = TwitchManager.Instance.ProfileImage;
        }

        public void ConnectAccountOnClick()
        {
            ConnectAccountOnClickAsync().Forget();
        }

        private async UniTaskVoid ConnectAccountOnClickAsync()
        {
            PlayerPrefs.SetInt("SkipTwitchAccount", 0);
            PlayerPrefs.Save();

            var anim = GetComponentInParent<PageDrillIn>();
            anim.Easing = AnimationEasing.EaseOut;
            await anim.StartAnimation();
            await SceneManager.LoadSceneAsync("Scenes/TwitchAuth");
        }

        public void Disconnect()
        {
            var dialog = Instantiate(_dialog, GameObject.Find("Canvas").transform, false);
            dialog.SetTitle("Disconnect account");
            dialog.SetContent("Are you sure you want to disconnect your account?");
            dialog.SetOnOkAction(() =>
            {
                TwitchManager.Instance.Disconnect();
                ConnectAccount.SetActive(!TwitchManager.Instance.IsAccountConnected);
                CurrentAccount.SetActive(TwitchManager.Instance.IsAccountConnected);
            });
        }
    }
}
