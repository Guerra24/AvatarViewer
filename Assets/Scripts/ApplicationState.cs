using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using AvatarViewer.Trackers;
using UnityEngine;
using Cysharp.Threading.Tasks;

#if UNITY_STANDALONE_WIN
using UnityRawInput;
#endif
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
    }

    public static class ApplicationPersistence
    {
#if UNITY_EDITOR
        private static string FilePath = Path.Combine(Application.persistentDataPath, "Settings-editor.json");
#else
        private static string FilePath = Path.Combine(Application.persistentDataPath, "Settings.json");
#endif

        public static AppSettings AppSettings { get; private set; } = new AppSettings();

        public static async UniTask Load()
        {
            if (File.Exists(FilePath))
            {
                var content = await File.ReadAllTextAsync(FilePath);
                AppSettings = await UniTask.RunOnThreadPool(() => JsonConvert.DeserializeObject<AppSettings>(content, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }));
            }
        }

        public static void Save()
        {
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(AppSettings, Formatting.Indented, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }));
        }
    }

    public class AppSettings
    {
        public int Version { get; } = 1;
        public List<Avatar> Avatars { get; } = new();
        public int Camera { get; set; } = -1;
        public int CameraCapability { get; set; } = -1;
        public string Microphone { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Tracker Tracker { get; set; }
        public Guid DefaultCameraPreset { get; } = Guid.Parse("00000000-0000-0000-0000-000000000000");
        public Dictionary<string, Reward> Rewards { get; } = new();
        public Dictionary<Tracker, TrackerSettings> Trackers { get; } = new()
        {
            { Tracker.OpenSee, new OpenSeeTrackerSettings() },
            { Tracker.Mediapipe, new MediapipeTrackerSettings()},
        };
        public Dictionary<Guid, CameraPreset> CameraPresets { get; } = new()
        {
            { Guid.Parse("00000000-0000-0000-0000-000000000000"), new CameraPreset{ Name= "Default", Absolute = false, Position = new Vector3(), Rotation = Quaternion.identity } },
        };
        [JsonConverter(typeof(StringEnumConverter))]
        public AntiAliasing AntiAliasing { get; set; } = AntiAliasing.Disabled;
        public int MSAALevel { get; set; } = 2;
        [JsonConverter(typeof(StringEnumConverter))]
        public ShadowResolution ShadowResolution { get; set; } = ShadowResolution.VeryHigh;
        [JsonConverter(typeof(StringEnumConverter))]
        public CaptureMode CaptureMode { get; set; }
        public int VSync { get; set; } = 1;
        public int TargetFrameRate { get; set; } = 60;
        [JsonConverter(typeof(StringEnumConverter))]
        public Resolution Resolution { get; set; } = Resolution.Res720;
        public int Width { get; set; } = 1280;
        public int Height { get; set; } = 720;
        public bool IncreasedPriority { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public LipSyncProvider LipSyncProvider { get; set; } = LipSyncProvider.uLipSync;
        public float Volume { get; set; } = 1.0f;
        public Dictionary<LipSyncProvider, LipSyncSettings> LipSyncSettings { get; } = new()
        {
            { LipSyncProvider.uLipSync, new uLipSyncSettings() },
            { LipSyncProvider.OculusLipSync, new OculusLipSyncSettings() },
        };
        [JsonConverter(typeof(QuaternionJsonConverter))]
        public Quaternion DirectionalLightRotation { get; set; } = Quaternion.Euler(170, 10, 0);
        public float DirectionalLightIntensity { get; set; } = 1f;
        public float DirectionalLightShadowStrength { get; set; } = 1f;
        public float AmbientIntensity { get; set; }
        [JsonConverter(typeof(ColorJsonConverter))]
        public Color AmbientColor { get; set; } = Color.white;
    }

    public enum LipSyncProfile
    {
        Default, Female, Male, Custom
    }

    public enum AntiAliasing
    {
        Disabled, FXAA, SMAA, TAA, MSAA
    }

    public enum CaptureMode
    {
        GameWindow, Spout2
    }

    public enum Resolution
    {
        Res720, Res1080, Res1440, ResCustom
    }

    public enum LipSyncProvider
    {
        uLipSync, OculusLipSync
    }

    public abstract class LipSyncSettings { }

    public class uLipSyncSettings : LipSyncSettings
    {
        public float MinVolume { get; set; } = -1.8f;
        public float MaxVolume { get; set; } = -0.5f;
        [JsonConverter(typeof(StringEnumConverter))]
        public LipSyncProfile Profile { get; set; } = LipSyncProfile.Default;
    }

    public class OculusLipSyncSettings : LipSyncSettings
    {
        public float Gain { get; set; } = 2.0f;
    }

    public abstract class TrackerSettings
    {
        public bool UseLocalTracker { get; set; } = true;
        public string ListenAddress { get; set; } = "127.0.0.1";
        public int Port { get; set; }
    }


    public class OpenSeeTrackerSettings : TrackerSettings
    {

        public int Quality { get; set; } = 3;

        public OpenSeeTrackerSettings()
        {
            Port = 11573;
        }
    }

    public class MediapipeTrackerSettings : TrackerSettings
    {
        public MediapipeTrackerSettings()
        {
            Port = 49983;
        }
    }

    public class CameraPreset
    {
        public string Name { get; set; }
        [JsonConverter(typeof(Vector3JsonConverter))]
        public Vector3 Position { get; set; }
        [JsonConverter(typeof(QuaternionJsonConverter))]
        public Quaternion Rotation { get; set; }
        public float FOV { get; set; } = 25.05336f;
        public bool Absolute { get; set; }
    }

    public class Avatar
    {
        public Guid Guid { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public bool Vrm { get; set; }
        public AvatarSettings Settings { get; set; }
        public Dictionary<string, AvatarBlendshape> Blendshapes { get; set; }

        public Avatar(string title, string path, bool vrm)
        {
            Guid = Guid.NewGuid();
            Title = title;
            Path = path;
            Vrm = vrm;
            Blendshapes = new();
            Settings = new();
        }

        public override bool Equals(object obj)
        {
            return obj is Avatar avatar && Guid.Equals(avatar.Guid);
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }
    }

    public class AvatarSettings
    {
        public bool Mirror { get; set; }
        public float TranslationScale { get; set; } = 0.05f;
        public float Smoothing { get; set; } = 0.5f;
        public bool DriftBack { get; set; }
        public bool AutoBlink { get; set; }
        public float BlinkSmoothing { get; set; } = 0.75f;
        public float EyeCloseThreshold { get; set; } = 0.2f;
        public float EyeOpenThreshold { get; set; } = 0.55f;
        public float EyebrowStrength { get; set; } = 1.0f;
        public float EyebrowZero { get; set; } = 0;
        public float EyebrowSensitivity { get; set; } = 1.0f;
        public float GazeSmoothing { get; set; } = 0.6f;
        public float GazeSensitivity { get; set; } = 0.9f;
        public float GazeStrength { get; set; } = 1.0f;
        public float GazeVerticalOffset { get; set; } = 0;
        public float GazeHorizontalOffset { get; set; } = 0;

        public AvatarSettings()
        {
        }

        public AvatarSettings(AvatarSettings o)
        {
            Mirror = o.Mirror;
            TranslationScale = o.TranslationScale;
            Smoothing = o.Smoothing;
            DriftBack = o.DriftBack;
            AutoBlink = o.AutoBlink;
            BlinkSmoothing = o.BlinkSmoothing;
            EyeCloseThreshold = o.EyeCloseThreshold;
            EyeOpenThreshold = o.EyeOpenThreshold;
            EyebrowStrength = o.EyebrowStrength;
            EyebrowZero = o.EyebrowZero;
            EyebrowSensitivity = o.EyebrowSensitivity;
            GazeSmoothing = o.GazeSmoothing;
            GazeSensitivity = o.GazeSensitivity;
            GazeStrength = o.GazeStrength;
            GazeVerticalOffset = o.GazeVerticalOffset;
            GazeHorizontalOffset = o.GazeHorizontalOffset;
        }
    }

    public class AvatarBlendshape
    {

        public bool Enabled { get; set; } = true;

        [JsonConverter(typeof(StringEnumConverter))]
        public AvatarBlendshapeType Type { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public AvatarBlendshapeMode Mode { get; set; } = AvatarBlendshapeMode.Toggle;
#if UNITY_STANDALONE_WIN
        public List<RawKey> Hotkey { get; } = new();
#else
        public List<int> Hotkey { get; } = new();
#endif
        public float Transition { get; set; }
    }

    public enum AvatarBlendshapeType
    {
        Base, Additive
    }

    public enum AvatarBlendshapeMode
    {
        Hold, Toggle
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

    public class Reward
    {
        public string Title { get; set; }
        [JsonIgnore]
        public Sprite TwitchImage { get; set; }

    }

    public class ItemReward : Reward
    {
        public ItemReward() { }
        public ItemReward(Reward reward)
        {
            Title = reward.Title;
            TwitchImage = reward.TwitchImage;
        }

        public float Timeout { get; set; } = 15;
        [JsonConverter(typeof(StringEnumConverter))]
        public ItemRewardAsset Asset { get; set; } = ItemRewardAsset.Box;
        public string AssetPath { get; set; } = "";
        public float Volume { get; set; } = 1.0f;
        [JsonConverter(typeof(StringEnumConverter))]
        public ItemRewardSound Sound { get; set; } = ItemRewardSound.Cardboard;
        public string SoundPath { get; set; } = "";
        [JsonConverter(typeof(StringEnumConverter))]
        public ItemRewardSpawnPoint SpawnPoint { get; set; } = ItemRewardSpawnPoint.Above;
    }

    public class CameraReward : Reward
    {
        public CameraReward() { }

        public CameraReward(Reward reward)
        {
            Title = reward.Title;
            TwitchImage = reward.TwitchImage;
        }

        public float Timeout { get; set; } = 15;
        public Guid CameraPreset { get; set; }
    }

    public class AvatarReward : Reward
    {
        public AvatarReward() { }

        public AvatarReward(Reward reward)
        {
            Title = reward.Title;
            TwitchImage = reward.TwitchImage;
        }

        public Guid Avatar { get; set; }
    }

    public class PickAvatarReward : Reward
    {
        public PickAvatarReward() { }
        public PickAvatarReward(Reward reward)
        {
            Title = reward.Title;
            TwitchImage = reward.TwitchImage;
        }

        public List<ChoicePickAvatarReward> Choices { get; } = new();
    }

    public class ChoicePickAvatarReward
    {
        public string Text { get; set; }
        public Guid Avatar { get; set; }

        public ChoicePickAvatarReward(string text, Guid avatar)
        {
            Text = text;
            Avatar = avatar;
        }
    }

    public enum ItemRewardSpawnPoint
    {
        Above, Front, Left, Right, Random
    }

    public enum ItemRewardAsset
    {
        Custom, Box
    }

    public enum ItemRewardSound
    {
        Custom, Cardboard
    }
}