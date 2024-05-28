using System;
using System.Collections.Concurrent;
using AvatarViewer.Twitch;
using Cysharp.Threading.Tasks;
using TwitchLib.PubSub.Events;
using UnityEngine;

namespace AvatarViewer
{
    public class RewardSpawner : MonoBehaviour
    {

        [SerializeField] private Transform Above;
        [SerializeField] private Transform Front;
        [SerializeField] private Transform Left;
        [SerializeField] private Transform Right;

        private ConcurrentQueue<ItemReward> Rewards = new();
        private bool _processingRewards;

        private void Start()
        {
            TwitchManager.Instance.PubSub.OnChannelPointsRewardRedeemed += PubSub_OnChannelPointsRewardRedeemed;
        }

        private void Update()
        {
            if (_processingRewards)
                return;
            _processingRewards = true;
            UpdateAsync().Forget();
        }

        private async UniTaskVoid UpdateAsync()
        {
            while (Rewards.TryDequeue(out var reward))
            {
                var spawnPoint = GetSpawn(reward.SpawnPoint);
                if (ApplicationState.RewardAssets.TryGetValue(reward.RewardAsset, out var asset))
                    SetupReward(Instantiate(asset.Prefab, spawnPoint.position, spawnPoint.rotation * asset.Prefab.transform.rotation), spawnPoint, reward);
                await UniTask.Delay(200);
            }
            _processingRewards = false;
        }

        private void PubSub_OnChannelPointsRewardRedeemed(object sender, OnChannelPointsRewardRedeemedArgs e)
        {
            if (ApplicationPersistence.AppSettings.Rewards.TryGetValue(e.RewardRedeemed.Redemption.Reward.Id, out var r) && r is ItemReward reward)
                Rewards.Enqueue(reward);
        }

        private void SetupReward(GameObject asset, Transform spawnPoint, ItemReward reward)
        {
            if (!asset.TryGetComponent<AudioSource>(out var @as))
            {
                @as = asset.AddComponent<AudioSource>();
                @as.playOnAwake = false;
            }

            if (ApplicationState.ExternalAudios.TryGetValue(reward.SoundPath, out var clip))
                @as.clip = clip;

            @as.volume = reward.Volume * ApplicationPersistence.AppSettings.Volume;

            asset.AddComponent<DestoyOnTimeout>().Seconds = reward.Timeout;
            var rigidbody = asset.GetComponent<Rigidbody>();
            rigidbody.linearVelocity = spawnPoint.forward.normalized * 5;
        }

        private Transform GetSpawn(ItemRewardSpawnPoint spawnPoint)
        {
            switch (spawnPoint)
            {
                case ItemRewardSpawnPoint.Above:
                    return Above;
                case ItemRewardSpawnPoint.Front:
                    return Front;
                case ItemRewardSpawnPoint.Left:
                    return Left;
                case ItemRewardSpawnPoint.Right:
                    return Right;
                case ItemRewardSpawnPoint.Random:
                    switch (UnityEngine.Random.Range(0, 1.0f))
                    {
                        case <= 0.25f:
                            return Above;
                        case <= 0.5f:
                            return Front;
                        case <= 0.75f:
                            return Left;
                        case <= 1.0f:
                            return Right;
                    }
                    break;
            }
            throw new Exception();
        }

        private void OnDestroy()
        {
            TwitchManager.Instance.PubSub.OnChannelPointsRewardRedeemed -= PubSub_OnChannelPointsRewardRedeemed;
        }

        private void OnApplicationQuit()
        {
            TwitchManager.Instance.PubSub.OnChannelPointsRewardRedeemed -= PubSub_OnChannelPointsRewardRedeemed;
        }

    }
}
