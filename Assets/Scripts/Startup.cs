using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AvatarViewer.Trackers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniGLTF;
using UnityEngine;
using UnityEngine.Networking;
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

            DOTween.Init();

            await ApplicationPersistence.Load();

            RuntimeSettings.Apply();

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
                    ApplicationState.VrmData.Add(avatar.Guid, new VRMImporterContext(new VRMData(new AutoGltfFileParser(avatar.Path).Parse())));
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
                foreach (var kvp in ApplicationPersistence.AppSettings.Rewards)
                {
                    if (kvp.Value is ItemReward reward && reward.Sound == ItemRewardSound.Custom && !ApplicationState.ExternalAudios.ContainsKey(reward.SoundPath))
                    {
                        Debug.Log($"Loading sound {reward.SoundPath}");
                        if (File.Exists(reward.SoundPath))
                        {
                            using var request = UnityWebRequestMultimedia.GetAudioClip($"file://{reward.SoundPath}", AudioType.WAV);
                            await request.SendWebRequest();
                            if (request.result == UnityWebRequest.Result.Success)
                                ApplicationState.ExternalAudios.Add(reward.SoundPath, DownloadHandlerAudioClip.GetContent(request));
                        }
                    }
                }
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
