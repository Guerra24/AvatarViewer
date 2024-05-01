using System;
using AvatarViewer.Twitch;
using TwitchLib.PubSub.Events;
using UnityEngine;
using UnityEngine.Audio;

namespace AvatarViewer
{
    public class RewardSpawner : MonoBehaviour
    {

        private TwitchController TwitchController;

        public Transform Above;
        public Transform Front;
        public Transform Left;
        public Transform Right;

        public GameObject Box;

        public AudioResource Cardboard;

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
                    var gameObject = GetBasebject(itemReward.Asset);

                    SetupReward(Instantiate(gameObject, spawnPoint.position, spawnPoint.rotation * gameObject.transform.rotation), spawnPoint, reward);
                });
            }
        }

        private void SetupReward(GameObject gameObject, Transform spawnPoint, ItemReward reward)
        {
            gameObject.AddComponent<RewardController>();
            var @as = gameObject.AddComponent<AudioSource>();
            if (reward.Sound == ItemRewardSound.Custom)
            {
                if (ApplicationState.ExternalAudios.TryGetValue(reward.SoundPath, out var clip))
                    @as.clip = clip;
            }
            else
                @as.resource = GetAudio(reward.Sound);
            @as.volume = reward.Volume * ApplicationPersistence.AppSettings.Volume;

            gameObject.AddComponent<DestoyOnTimeout>().Seconds = reward.Timeout;
            var rigidbody = gameObject.GetComponent<Rigidbody>();
            rigidbody.velocity = spawnPoint.forward.normalized * 5;
        }

        private GameObject GetBasebject(ItemRewardAsset assetType)
        {
            switch (assetType)
            {
                case ItemRewardAsset.Box:
                    return Box;
            }
            throw new Exception();
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

        private AudioResource GetAudio(ItemRewardSound sound)
        {
            switch (sound)
            {
                case ItemRewardSound.Cardboard:
                    return Cardboard;
            }
            throw new Exception();
        }

    }
}
