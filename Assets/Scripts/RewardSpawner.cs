using System;
using AvatarViewer.Twitch;
using TwitchLib.PubSub.Events;
using UnityEngine;

namespace AvatarViewer
{
    public class RewardSpawner : MonoBehaviour
    {

        private TwitchController TwitchController;

        public Transform Above;
        public Transform Front;
        public Transform Left;
        public Transform Right;

        void Start()
        {
            TwitchController = GameObject.Find("TwitchController").GetComponent<TwitchController>();
            TwitchController.PubSub.OnChannelPointsRewardRedeemed += PubSub_OnChannelPointsRewardRedeemed;
        }

        private void PubSub_OnChannelPointsRewardRedeemed(object sender, OnChannelPointsRewardRedeemedArgs e)
        {
            if (ApplicationPersistence.AppSettings.Rewards.TryGetValue(e.RewardRedeemed.Redemption.Reward.Id, out var r) && r is ItemReward reward)
            {
                MainThreadDispatcher.AddOnUpdate(() =>
                {
                    var itemReward = reward;
                    var spawnPoint = GetSpawn(itemReward.SpawnPoint);
                    if (ApplicationState.RewardAssets.TryGetValue(reward.RewardAsset, out var asset))
                    {
                        var gameObject = asset.Object;
                        SetupReward(Instantiate(gameObject, spawnPoint.position, spawnPoint.rotation * gameObject.transform.rotation), spawnPoint, reward);
                    }
                });
            }
        }

        private void SetupReward(GameObject gameObject, Transform spawnPoint, ItemReward reward)
        {
            if (!gameObject.TryGetComponent<AudioSource>(out var @as))
            {
                @as = gameObject.AddComponent<AudioSource>();
                @as.playOnAwake = false;
            }

            if (ApplicationState.ExternalAudios.TryGetValue(reward.SoundPath, out var clip))
                @as.clip = clip;

            @as.volume = reward.Volume * ApplicationPersistence.AppSettings.Volume;

            gameObject.AddComponent<DestoyOnTimeout>().Seconds = reward.Timeout;
            var rigidbody = gameObject.GetComponent<Rigidbody>();
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
            TwitchController.PubSub.OnChannelPointsRewardRedeemed -= PubSub_OnChannelPointsRewardRedeemed;
        }

        private void OnApplicationQuit()
        {
            TwitchController.PubSub.OnChannelPointsRewardRedeemed -= PubSub_OnChannelPointsRewardRedeemed;
        }

    }
}
