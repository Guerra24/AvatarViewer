using System;
using AvatarViewer.Ui.Animations;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AvatarViewer.Twitch
{
    public class TwitchStartup : MonoBehaviour
    {

        private TwitchController TwitchController;

        public GameObject Content;
        public GameObject Startup;
        public GameObject Buttons;
        public GameObject Placeholder;

        public PageDrillIn PageDrillIn;

        private void Awake()
        {
            TwitchController = GameObject.Find("TwitchController").GetComponent<TwitchController>();
        }

        private void Start()
        {
            StartAsync().Forget();
        }

        private async UniTaskVoid StartAsync()
        {
            if (PlayerPrefs.GetInt("SkipTwitchAccount", 0) == 0)
            {
                if (!await TwitchController.Manager.ValidateToken())
                {
                    Startup.SetActive(false);
                    Content.SetActive(true);
                    return;
                }
                else
                    await TwitchController.Init();
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

            await TwitchController.Manager.GenerateClientSecret();
            if (await TwitchController.Manager.ValidateToken()) await TwitchController.Init();
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
