using System;
using System.Collections;
using System.IO;
using AvatarViewer.Trackers;
using AvatarViewer.Twitch;
using Cysharp.Threading.Tasks;
using UniGLTF;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using VRM;

namespace AvatarViewer
{
    public class Startup : MonoBehaviour
    {

        private UIDocument _document;

        private ProgressBar _progressBar;

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
            _progressBar = _document.rootVisualElement.Q<ProgressBar>("Progress");
            RuntimeSettings.Apply();
            TrackerManager.Initialize(Path.Combine(Application.persistentDataPath, "trackers"), Application.temporaryCachePath);
        }

        void Start()
        {
            Load().Forget();
        }

        async UniTaskVoid Load()
        {
            await UniTask.Delay(TimeSpan.FromMilliseconds(500));
            _progressBar.visible = true;

            await ApplicationPersistence.Load();

            float percentPerStep = 1f / (ApplicationPersistence.AppSettings.Avatars.Count * 2 + 2);
            float baseProgress = 0;

            foreach (var avatar in ApplicationPersistence.AppSettings.Avatars)
            {
                if (!avatar.Vrm)
                {
                    Debug.Log($"Loading Bundle {avatar.Path}");
                    var assetBundle = await AssetBundle.LoadFromFileAsync(avatar.Path).ToUniTask(Progress.Create<float>(p => _progressBar.value = baseProgress + p * percentPerStep));
                    baseProgress += percentPerStep;

                    Debug.Log($"Loading Asset");
                    var asset = await assetBundle.LoadAssetAsync<GameObject>("VSFAvatar").ToUniTask(Progress.Create<float>(p => _progressBar.value = baseProgress + p * percentPerStep));
                    baseProgress += percentPerStep;

                    ApplicationState.AvatarBundles.Add(avatar.Guid, new LoadedAvatar(assetBundle, asset as GameObject));
                }
                else
                {
                    baseProgress += percentPerStep;
                    _progressBar.value = baseProgress;
                    await UniTask.Yield();
                    await UniTask.NextFrame();
                    GltfData data = new AutoGltfFileParser(avatar.Path).Parse();
                    var vrm = new VRMData(data);
                    ApplicationState.VrmData.Add(avatar.Guid, vrm);
                    await UniTask.Yield();
                    await UniTask.NextFrame();
                }
            }

            await UniTask.Yield();
            await UniTask.NextFrame();
            {

                await SceneManager.LoadSceneAsync("Scenes/AlwaysLoaded", LoadSceneMode.Additive).ToUniTask(Progress.Create<float>(p => _progressBar.value = baseProgress + p * percentPerStep));
                baseProgress += percentPerStep;
            }

            await UniTask.Yield();
            await UniTask.NextFrame();
            {
                await UniTask.SwitchToThreadPool();
                await TrackerManager.DownloadAll();
                await UniTask.SwitchToMainThread();
            }

            await UniTask.Yield();
            await UniTask.NextFrame();
            {
                var op = SceneManager.LoadSceneAsync("Scenes/TwitchAuth", LoadSceneMode.Single);
                op.allowSceneActivation = false;

                while (!op.isDone)
                {
                    _progressBar.value = baseProgress + op.progress.Remap(0, 0.9f, 0, 1) * percentPerStep;
                    if (op.progress >= 0.9f)
                        op.allowSceneActivation = true;
                    await UniTask.Yield();
                    await UniTask.NextFrame();
                }
            }

        }

    }
}
