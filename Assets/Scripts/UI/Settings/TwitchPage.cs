using AvatarViewer.Twitch;
using AvatarViewer.Ui.Settings;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
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
            LoadTwitchAccount().Forget();
    }

    private async UniTaskVoid LoadTwitchAccount()
    {
        var user = twitchController.User;
        DisplayName.text = user.DisplayName;
        using var request = UnityWebRequestTexture.GetTexture(user.ProfileImageUrl);
        await request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            var texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            ProfileImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
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
