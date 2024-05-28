using AvatarViewer.Twitch;
using AvatarViewer.UI.Animations;
using AvatarViewer.UI.Settings;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TwitchPage : BaseSettingsPage
{
    public GameObject Dialog;

    public GameObject ConnectAccount;
    public GameObject CurrentAccount;

    public TMP_Text DisplayName;
    public Image ProfileImage;

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
        var dialog = Instantiate(Dialog, GameObject.Find("Canvas").transform, false);
        var data = dialog.GetComponentInChildren<Dialog>();
        data.SetTitle("Disconnect account");
        data.SetContent("Are you sure you want to disconnect your account?");
        data.SetOnOkAction(() =>
        {
            TwitchManager.Instance.Disconnect();
            ConnectAccount.SetActive(!TwitchManager.Instance.IsAccountConnected);
            CurrentAccount.SetActive(TwitchManager.Instance.IsAccountConnected);
        });
    }
}
