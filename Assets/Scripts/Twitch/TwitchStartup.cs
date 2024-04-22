using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AvatarViewer.Twitch
{
    public class TwitchStartup : MonoBehaviour
    {

        private TwitchController TwitchController;

        public GameObject Content;
        public GameObject Buttons;
        public GameObject Placeholder;

        private void Awake()
        {
            TwitchController = GameObject.Find("TwitchController").GetComponent<TwitchController>();
        }

        private void Start()
        {
            Startup().Forget();
        }

        private async UniTaskVoid Startup()
        {
            if (PlayerPrefs.GetInt("SkipTwitchAccount", 0) == 0)
            {
                if (!await TwitchController.Manager.ValidateToken())
                {
                    Content.SetActive(true);
                    return;
                }
                else
                    TwitchController.Init();
            }

            await UniTask.Yield();
            await UniTask.NextFrame();
            await SceneManager.LoadSceneAsync("Scenes/Menu", LoadSceneMode.Single);
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
            if (await TwitchController.Manager.ValidateToken()) TwitchController.Init();
            else
            {
                Buttons.SetActive(true);
                Placeholder.SetActive(false);
                return;
            }

            await UniTask.Yield();
            await UniTask.NextFrame();
            await SceneManager.LoadSceneAsync("Scenes/Menu", LoadSceneMode.Single);
        }

        public void Skip() => SkipAsync().Forget();

        private async UniTaskVoid SkipAsync()
        {
            PlayerPrefs.SetInt("SkipTwitchAccount", 1);
            await UniTask.Yield();
            await UniTask.NextFrame();
            await SceneManager.LoadSceneAsync("Scenes/Menu", LoadSceneMode.Single);
        }

    }
}
