using AvatarViewer.Twitch;
using AvatarViewer.Ui.Settings;
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

    private TwitchController twitchController;

    protected override void Awake()
    {
        base.Awake();
        twitchController = GameObject.Find("TwitchController").GetComponent<TwitchController>();
    }

    private void Start()
    {
        ConnectAccount.SetActive(!twitchController.IsAccountConnected);
        CurrentAccount.SetActive(twitchController.IsAccountConnected);
        if (twitchController.IsAccountConnected)
            LoadTwitchAccount();
    }

    private void LoadTwitchAccount()
    {
        var user = twitchController.User;
        DisplayName.text = user.DisplayName;
        ProfileImage.sprite = twitchController.ProfileImage;
    }

    public void ConnectAccountOnClick()
    {
        PlayerPrefs.SetInt("SkipTwitchAccount", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Scenes/TwitchAuth");
    }

    public void Disconnect()
    {
        var dialog = Instantiate(Dialog, GameObject.Find("Canvas").transform, false);
        var data = dialog.GetComponentInChildren<Dialog>();
        data.SetTitle("Disconnect account");
        data.SetContent("Are you sure you want to disconnect your account?");
        data.SetOnOkAction(() =>
        {
            twitchController.Disconnect();
            ConnectAccount.SetActive(!twitchController.IsAccountConnected);
            CurrentAccount.SetActive(twitchController.IsAccountConnected);
        });
    }
}
