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

        public GameObject Box;

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
                    break;
            }
            throw new Exception();
        }

    }
}
