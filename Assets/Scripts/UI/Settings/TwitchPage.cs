using UnityEngine;
using UnityEngine.SceneManagement;

public class TwitchPage : MonoBehaviour
{
    public void ConnectAccount()
    {
        PlayerPrefs.SetInt("SkipTwitchAccount", 0);
        SceneManager.LoadScene("Scenes/TwitchAuth");
    }
}
