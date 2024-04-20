using System;
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
            if (ApplicationPersistence.AppSettings.Rewards.TryGetValue(e.RewardRedeemed.Redemption.Reward.Id, out var reward))
            {
                MainThreadDispatcher.AddOnUpdate(() =>
                {
                    var spawnPoint = GetSpawn(reward.SpawnPoint);
                    var gameObject = GetBasebject(reward.Type);

                    SetupReward(Instantiate(gameObject, spawnPoint.position, spawnPoint.rotation * gameObject.transform.rotation), spawnPoint, reward);
                });
            }
        }

        private void SetupReward(GameObject gameObject, Transform spawnPoint, Reward reward)
        {
            gameObject.AddComponent<RewardController>();
            gameObject.AddComponent<DestoyOnTimeout>().Seconds = reward.Timeout;
            var rigidbody = gameObject.GetComponent<Rigidbody>();
            rigidbody.velocity = spawnPoint.forward.normalized * 5;
        }

        private GameObject GetBasebject(AssetType assetType)
        {
            switch (assetType)
            {
                case AssetType.Box:
                    return Box;
            }
            throw new Exception();
        }

        private Transform GetSpawn(RewardSpawnPoint spawnPoint)
        {
            switch (spawnPoint)
            {
                case RewardSpawnPoint.Above:
                    return Above;
                case RewardSpawnPoint.Front:
                    return Front;
                case RewardSpawnPoint.Left:
                    return Left;
                case RewardSpawnPoint.Right:
                    return Right;
                case RewardSpawnPoint.Random:
                    break;
            }
            throw new Exception();
        }

    }
}
