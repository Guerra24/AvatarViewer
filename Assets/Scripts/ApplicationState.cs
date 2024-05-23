using System;
using System.Collections.Generic;
using AvatarViewer.SDK;
using UnityEngine;
using VRM;

namespace AvatarViewer
{
    public static class ApplicationState
    {
        public static Avatar CurrentAvatar { get; set; }

        public static int RuntimeWidth, RuntimeHeight;

        public static Dictionary<Guid, LoadedAvatar> AvatarBundles { get; } = new();

        public static Dictionary<Guid, VRMImporterContext> VrmData { get; } = new();

        public static Dictionary<string, AudioClip> ExternalAudios { get; } = new();

        public static Dictionary<Guid, LoadedRewardAsset> RewardAssets { get; } = new();

        public static Dictionary<Guid, LoadedRewardAssetsBundle> RewardBundles { get; } = new();

    }


    public class LoadedAvatar
    {

        public AssetBundle Bundle { get; }
        public GameObject Object { get; }

        public LoadedAvatar(AssetBundle bundle, GameObject @object)
        {
            Bundle = bundle;
            Object = @object;
        }
    }

    public class LoadedRewardAsset
    {
        public GameObject Object { get; }
        public RewardAsset RewardAsset { get; }

        public LoadedRewardAsset(GameObject @object, RewardAsset rewardAsset)
        {
            Object = @object;
            RewardAsset = rewardAsset;
        }
    }

    public class LoadedRewardAssetsBundle
    {
        public AssetBundle Bundle { get; }

        public Dictionary<Guid, LoadedRewardAsset> RewardAssets { get; }

        public LoadedRewardAssetsBundle(AssetBundle bundle, Dictionary<Guid, LoadedRewardAsset> rewardAssets)
        {
            Bundle = bundle;
            RewardAssets = rewardAssets;
        }
    }

}