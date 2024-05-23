using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AvatarViewer.SDK;
using AvatarViewer.Trackers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniGLTF;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using VRM;

namespace AvatarViewer
{
    public class Startup : MonoBehaviour
    {

        [SerializeField] private TMP_Text Status;
        [SerializeField] private LocalizedString MissingAvatar;
        [SerializeField] private LocalizedString MissingRewardBundle;

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

            DOTween.Init();

            await ApplicationPersistence.Load();

            RuntimeSettings.Apply();
            {
                var statusText = await MissingAvatar.GetLocalizedStringAsync();

                foreach (var avatar in ApplicationPersistence.AppSettings.Avatars.ToList())
                    if (!File.Exists(avatar.Path))
                    {
                        Status.text = statusText.AsFormat(avatar.Path.Replace(@"\", @"\\"));
                        await UniTask.Delay(TimeSpan.FromSeconds(2));
                        var dir = Path.GetDirectoryName(avatar.Path);
                        var res = NativeFileDialogSharp.Dialog.FileOpen("ava,vsfavatar,vrm", Directory.Exists(dir) ? dir : null);
                        Status.text = "";

                        if (res.IsOk)
                            avatar.Path = res.Path;
                        else
                            ApplicationPersistence.AppSettings.Avatars.Remove(avatar);
                    }
                ApplicationPersistence.Save();
            }

            float percentPerStep = 1f / (ApplicationPersistence.AppSettings.Avatars.Count * 2 + 2);
            float baseProgress = 0;
            _progressBar.visible = true;

            var tasks = new List<UniTask>();
            foreach (var avatar in ApplicationPersistence.AppSettings.Avatars.Where(v => !v.Vrm).ToList())
                tasks.Add(AssetBundle.LoadFromFileAsync(avatar.Path).ToUniTask().ContinueWith(async (assetBundle) =>
                {
                    baseProgress += percentPerStep;
                    _progressBar.value = baseProgress;
                    await assetBundle.LoadAssetAsync<GameObject>("VSFAvatar").ToUniTask().ContinueWith((asset) =>
                    {
                        ApplicationState.AvatarBundles.Add(avatar.Guid, new LoadedAvatar(assetBundle, asset as GameObject));
                        baseProgress += percentPerStep;
                        _progressBar.value = baseProgress;
                    });
                }));

            await UniTask.WhenAll(tasks);

            foreach (var avatar in ApplicationPersistence.AppSettings.Avatars.Where(a => a.Vrm).ToList())
            {
                baseProgress += percentPerStep;
                _progressBar.value = baseProgress;
                await UniTask.Yield();
                await UniTask.NextFrame();
                ApplicationState.VrmData.Add(avatar.Guid, new VRMImporterContext(new VRMData(new AutoGltfFileParser(avatar.Path).Parse())));
                await UniTask.Yield();
                await UniTask.NextFrame();
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
                var bundles = ApplicationPersistence.AppSettings.RewardAssetsBundles;
                var statusText = await MissingRewardBundle.GetLocalizedStringAsync();

                foreach (var kvp in bundles.ToList())
                {
                    if (!File.Exists(kvp.Value.Path))
                    {
                        Status.text = statusText.AsFormat(kvp.Value.Path.Replace(@"\", @"\\"));
                        await UniTask.Delay(TimeSpan.FromSeconds(2));
                        var dir = Path.GetDirectoryName(kvp.Value.Path);
                        var res = NativeFileDialogSharp.Dialog.FileOpen("avr", Directory.Exists(dir) ? dir : null);
                        Status.text = "";

                        if (res.IsOk)
                            kvp.Value.Path = res.Path;
                        else
                            ApplicationPersistence.AppSettings.RewardAssetsBundles.Remove(kvp.Key);
                    }
                }

                foreach (var kvp in bundles.Union(new Dictionary<Guid, RewardAssetsBundle>() { { Guid.Empty, new RewardAssetsBundle(Path.Combine(Application.streamingAssetsPath, "builtin-rewards")) } }).ToDictionary(k => k.Key, v => v.Value))
                {
                    var bundle = await AssetBundle.LoadFromFileAsync(kvp.Value.Path);
                    var request = bundle.LoadAllAssetsAsync<GameObject>();
                    await request;
                    var rewardAssets = new Dictionary<Guid, LoadedRewardAsset>();
                    foreach (var @object in request.allAssets)
                    {
                        var reward = @object as GameObject;
                        var rewardAsset = reward.GetComponent<RewardAsset>();
                        var lra = new LoadedRewardAsset(reward, rewardAsset);
                        ApplicationState.RewardAssets.Add(rewardAsset.Guid, lra);
                        rewardAssets.Add(rewardAsset.Guid, lra);
                    }
                    ApplicationState.RewardBundles.Add(kvp.Key, new LoadedRewardAssetsBundle(bundle, rewardAssets));
                }
            }

            await UniTask.Yield();
            await UniTask.NextFrame();
            {
                foreach (var kvp in ApplicationPersistence.AppSettings.Rewards)
                {
                    if (kvp.Value is ItemReward reward && !string.IsNullOrEmpty(reward.SoundPath) && File.Exists(reward.SoundPath) && !ApplicationState.ExternalAudios.ContainsKey(reward.SoundPath))
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
