using System;
using System.Collections.Generic;
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

        public static Dictionary<string, AssetBundle> RewardBundles { get; } = new();

    }

}