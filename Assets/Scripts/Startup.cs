using System.Collections;
using System.IO;
using AvatarViewer.Trackers;
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
            ApplicationPersistence.Load();

            float percentPerStep = 1f / (ApplicationPersistence.AppSettings.Avatars.Count * 2 + 2);
            float baseProgress = 0;

            foreach (var avatar in ApplicationPersistence.AppSettings.Avatars)
            {
                if (!avatar.Vrm)
                {
                    var bundleOp = AssetBundle.LoadFromFileAsync(avatar.Path);
                    Debug.Log($"Loading Bundle {avatar.Path}");
                    while (!bundleOp.isDone)
                    {
                        _progressBar.value = baseProgress + bundleOp.progress * percentPerStep;
                        await UniTask.Yield();
                        await UniTask.NextFrame();
                    }
                    baseProgress += percentPerStep;

                    var assetBundle = bundleOp.assetBundle;

                    var assetOp = assetBundle.LoadAssetAsync<GameObject>("VSFAvatar");
                    Debug.Log($"Loading Asset");
                    while (!assetOp.isDone)
                    {
                        _progressBar.value = baseProgress + assetOp.progress * percentPerStep;
                        await UniTask.Yield();
                        await UniTask.NextFrame();
                    }
                    baseProgress += percentPerStep;

                    ApplicationState.AvatarBundles.Add(avatar.Guid, new LoadedAvatar(assetBundle, assetOp.asset as GameObject));
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

                var op = SceneManager.LoadSceneAsync("Scenes/AlwaysLoaded", LoadSceneMode.Additive);
                while (!op.isDone)
                {
                    _progressBar.value = baseProgress + op.progress * percentPerStep;
                    await UniTask.Yield();
                    await UniTask.NextFrame();
                }
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

                var op = SceneManager.LoadSceneAsync("Scenes/Menu", LoadSceneMode.Single);
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
