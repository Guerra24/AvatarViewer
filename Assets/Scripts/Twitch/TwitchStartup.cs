using System;
using AvatarViewer.UI.Animations;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AvatarViewer.Twitch
{
    public class TwitchStartup : MonoBehaviour
    {

        public GameObject Content;
        public GameObject Startup;
        public GameObject Buttons;
        public GameObject Placeholder;

        public PageDrillIn PageDrillIn;

        private void Start()
        {
            StartAsync().Forget();
        }

        private async UniTaskVoid StartAsync()
        {
            if (PlayerPrefs.GetInt("SkipTwitchAccount", 0) == 0)
            {
                if (!await TwitchManager.Instance.Auth.ValidateToken())
                {
                    Startup.SetActive(false);
                    Content.SetActive(true);
                    return;
                }
                else
                    await TwitchManager.Instance.Init();
            }

            await UniTask.Yield();
            await UniTask.NextFrame();

            await PageDrillIn.StartAnimation().ToUniTask();
            await SceneManager.LoadSceneAsync("Scenes/Menu", LoadSceneMode.Single).ToUniTask();
        }

        public void ConnectAccount() => ConnectAccountAsync().Forget();

        private async UniTaskVoid ConnectAccountAsync()
        {
            await UniTask.Delay(TimeSpan.FromMilliseconds(250));
            Buttons.SetActive(false);
            Placeholder.SetActive(true);

            await UniTask.Yield();
            await UniTask.NextFrame();

            await TwitchManager.Instance.Auth.GenerateClientSecret();
            if (await TwitchManager.Instance.Auth.ValidateToken()) await TwitchManager.Instance.Init();
            else
            {
                Buttons.SetActive(true);
                Placeholder.SetActive(false);
                return;
            }

            await UniTask.Yield();
            await UniTask.NextFrame();

            await PageDrillIn.StartAnimation().ToUniTask();
            await SceneManager.LoadSceneAsync("Scenes/Menu", LoadSceneMode.Single).ToUniTask();
        }

        public void Skip() => SkipAsync().Forget();

        private async UniTaskVoid SkipAsync()
        {
            PlayerPrefs.SetInt("SkipTwitchAccount", 1);
            PlayerPrefs.Save();
            await UniTask.Yield();
            await UniTask.NextFrame();

            await PageDrillIn.StartAnimation().ToUniTask();
            await SceneManager.LoadSceneAsync("Scenes/Menu", LoadSceneMode.Single).ToUniTask();
        }

    }
}
