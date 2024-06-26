﻿using System;
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

        public static Dictionary<Guid, RewardAssetInfo> RewardAssets { get; } = new();

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

    public class LoadedRewardAssetsBundle
    {
        public AssetBundle Bundle { get; }

        public Dictionary<Guid, RewardAssetInfo> RewardAssets { get; }

        public LoadedRewardAssetsBundle(AssetBundle bundle, Dictionary<Guid, RewardAssetInfo> rewardAssets)
        {
            Bundle = bundle;
            RewardAssets = rewardAssets;
        }
    }

}