using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DitzelGames.FastIK;
using uLipSync;
using UniGLTF;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
#if UNITY_STANDALONE_WIN
using UnityRawInput;
#endif
using VRM;
using VRMShaders;
using VSeeFace;

namespace AvatarViewer
{
    public class AvatarLoader : MonoBehaviour
    {

        [HideInInspector]
        public GameObject avatar;

        [Header("Driver")]
        public OpenSeeVRMDriver Driver;
        [Header("IK Target at Head")]
        [SerializeField] private Transform HeadTarget;
        [Header("Kinematic target")]
        [SerializeField] private Transform KinematicInterpolation;
        [Header("Hand tracking")]
        [SerializeField] private Transform LeftWristTarget;
        [SerializeField] private Transform LeftElbowTarget;
        [SerializeField] private Transform RightWristTarget;
        [SerializeField] private Transform RightElbowTarget;
        [Header("Default T Pose")]
        [SerializeField] private AnimationClip TPose;
        [Header("Camera position")]
        [SerializeField] private Transform CameraPosition;
        [Header("Rewards")]
        [SerializeField] private Transform Rewards;
        [Header("uLipSync Profile Default")]
        [SerializeField] private Profile ProfileDefault;
        [Header("uLipSync Profile Female")]
        [SerializeField] private Profile ProfileFemale;
        [Header("uLipSync Profile Male")]
        [SerializeField] private Profile ProfileMale;

        private PlayableGraph graph;
        private AnimationMixerPlayable mixer;

        private bool ranOnFirstFrame = true;

        private Dictionary<BlendShapeKey, VsfAnimationDetails?> Animations = new();

        private RuntimeGltfInstance vrm;

        private Guid LoadedAvatar;

        public void Awake()
        {
#if UNITY_STANDALONE_WIN
            RawInput.WorkInBackground = true;
            RawInput.Start();
#endif
        }

        // Start is called before the first frame update
        void Start()
        {
            LoadAvatar().Forget();
        }

        public async UniTask LoadAvatar()
        {
            if (LoadedAvatar == ApplicationState.CurrentAvatar.Guid)
                return;
            LoadedAvatar = ApplicationState.CurrentAvatar.Guid;

            if (!ApplicationState.CurrentAvatar.Vrm && !ApplicationState.AvatarBundles.ContainsKey(ApplicationState.CurrentAvatar.Guid))
                await AssetBundle.LoadFromFileAsync(ApplicationState.CurrentAvatar.Path).ToUniTask().ContinueWith(async (assetBundle) => await assetBundle.LoadAssetAsync<GameObject>("VSFAvatar").ToUniTask().ContinueWith((asset) => ApplicationState.AvatarBundles.Add(ApplicationState.CurrentAvatar.Guid, new LoadedAvatar(assetBundle, asset as GameObject))));

            if (ApplicationState.CurrentAvatar.Vrm && !ApplicationState.VrmData.ContainsKey(ApplicationState.CurrentAvatar.Guid))
                ApplicationState.VrmData.Add(ApplicationState.CurrentAvatar.Guid, new VRMImporterContext(new VRMData(new AutoGltfFileParser(ApplicationState.CurrentAvatar.Path).Parse())));

            if (graph.IsValid())
                graph.Destroy();
            if (mixer.IsValid())
                mixer.Destroy();
            if (vrm != null)
            {
                vrm.Dispose();
                vrm = null;
                avatar = null;
            }

            if (avatar != null)
                Destroy(avatar);

            Driver.expressions.Clear();
            Animations.Clear();

            if (!ApplicationState.CurrentAvatar.Vrm)
            {
                var prefab = ApplicationState.AvatarBundles[ApplicationState.CurrentAvatar.Guid].Object;

                avatar = Instantiate(prefab);
            }
            else
            {
                vrm = await ApplicationState.VrmData[ApplicationState.CurrentAvatar.Guid].LoadAsync(new RuntimeOnlyAwaitCaller());
                vrm.ShowMeshes();
                vrm.EnableUpdateWhenOffscreen();

                avatar = vrm.gameObject;
            }

            LeftWristTarget.rotation = Quaternion.Euler(0, 0, 0);
            RightWristTarget.rotation = Quaternion.Euler(0, 0, 0);

            var animator = avatar.GetComponent<Animator>();

            Driver.vrmBlendShapeProxy = avatar.GetComponent<VRMBlendShapeProxy>();
            Driver.leftEye = animator.GetBoneTransform(HumanBodyBones.LeftEye);
            Driver.rightEye = animator.GetBoneTransform(HumanBodyBones.RightEye);
            var head = animator.GetBoneTransform(HumanBodyBones.Head);
            var neck = animator.GetBoneTransform(HumanBodyBones.Neck);

            HeadTarget.position = head.position;
            HeadTarget.position += new Vector3(0, 0.1f, 0);

            KinematicInterpolation.localRotation = Quaternion.identity;
            KinematicInterpolation.localPosition = Vector3.zero;

            var ik = head.gameObject.AddComponent<FastIKFabric>();
            ik.ChainLength = 4;
            ik.Target = KinematicInterpolation;
            ik.Init();

            if (!head.gameObject.TryGetComponent<SphereCollider>(out var _))
            {
                var sc = head.gameObject.AddComponent<SphereCollider>();

                if (head.gameObject.TryGetComponent<VRMSpringBoneColliderGroup>(out var collider) && collider.Colliders.Length > 0)
                {
                    var coll = collider.Colliders.First();
                    sc.radius = coll.Radius;
                    sc.center = coll.Offset;
                }
                else
                {
                    sc.radius = 0.1f;
                }

            }

            var chest = animator.GetBoneTransform(HumanBodyBones.Chest);

            if (!neck.gameObject.TryGetComponent<CapsuleCollider>(out var _))
            {
                var cc = neck.gameObject.AddComponent<CapsuleCollider>();
                if (neck.gameObject.TryGetComponent<VRMSpringBoneColliderGroup>(out var collider) && collider.Colliders.Length > 0)
                {
                    var coll = collider.Colliders.First();
                    cc.radius = coll.Radius;
                    cc.center = coll.Offset;
                }
                else
                {
                    cc.radius = 0.04f;
                }
                cc.height = head.position.y - chest.position.y;
            }

            if (!chest.gameObject.TryGetComponent<CapsuleCollider>(out var _))
            {
                var cc = chest.gameObject.AddComponent<CapsuleCollider>();
                var hips = animator.GetBoneTransform(HumanBodyBones.Hips);
                cc.radius = 0.08f;
                cc.height = head.position.y - hips.position.y;
            }

            var leftWrist = animator.GetBoneTransform(HumanBodyBones.LeftHand);
            var lwik = leftWrist.gameObject.AddComponent<FastIKFabric>();
            lwik.ChainLength = 2;
            lwik.Target = LeftWristTarget;
            lwik.Pole = LeftElbowTarget;

            var rightWrist = animator.GetBoneTransform(HumanBodyBones.RightHand);
            var rwik = rightWrist.gameObject.AddComponent<FastIKFabric>();
            rwik.ChainLength = 2;
            rwik.Target = RightWristTarget;
            rwik.Pole = RightElbowTarget;

            /*var ltd = animator.GetBoneTransform(HumanBodyBones.LeftThumbDistal).gameObject.AddComponent<FastIKFabric>();
            ltd.ChainLength = 2;

            var lid = animator.GetBoneTransform(HumanBodyBones.LeftIndexDistal).gameObject.AddComponent<FastIKFabric>();
            lid.ChainLength = 2;

            var lmd = animator.GetBoneTransform(HumanBodyBones.LeftMiddleDistal).gameObject.AddComponent<FastIKFabric>();
            lmd.ChainLength = 2;

            var lrd = animator.GetBoneTransform(HumanBodyBones.LeftRingDistal).gameObject.AddComponent<FastIKFabric>();
            lrd.ChainLength = 2;

            var lld = animator.GetBoneTransform(HumanBodyBones.LeftLittleDistal).gameObject.AddComponent<FastIKFabric>();
            lld.ChainLength = 2;*/

            var handTracker = GetComponent<HandTracker>();
            handTracker.LeftWrist = LeftWristTarget;
            handTracker.LeftElbow = LeftElbowTarget;

            handTracker.RightWrist = RightWristTarget;
            handTracker.RightElbow = RightElbowTarget;

            handTracker.AvatarLeftShoulder = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);

            handTracker.AvatarLeftThumbCmc = animator.GetBoneTransform(HumanBodyBones.LeftThumbProximal);
            handTracker.AvatarLeftIndexMcp = animator.GetBoneTransform(HumanBodyBones.LeftIndexProximal);
            handTracker.AvatarLeftMiddleMcp = animator.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);
            handTracker.AvatarLeftRingMcp = animator.GetBoneTransform(HumanBodyBones.LeftRingProximal);
            handTracker.AvatarLeftPinkyMcp = animator.GetBoneTransform(HumanBodyBones.LeftLittleProximal);

            /*ltd.Pole = handTracker.AvatarLeftThumbPole;
            lid.Pole = handTracker.AvatarLeftIndexPole;
            lmd.Pole = handTracker.AvatarLeftMiddlePole;
            lrd.Pole = handTracker.AvatarLeftRingPole;
            lld.Pole = handTracker.AvatarLeftPinkyPole;

            handTracker.AvatarLeftThumbTarget = ltd.Target;
            handTracker.AvatarLeftIndexTarget = lid.Target;
            handTracker.AvatarLeftMiddleTarget = lmd.Target;
            handTracker.AvatarLeftRingTarget = lrd.Target;
            handTracker.AvatarLeftPinkyTarget = lld.Target;*/

            handTracker.AvatarRightShoulder = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);

            var yReference = neck.position.y + ((head.position.y - neck.position.y) * 2f);

            CameraPosition.position = new Vector3(0, yReference, 1.91f);
            Rewards.position = new Vector3(0, yReference, 0);

            if (ApplicationPersistence.AppSettings.LipSyncProvider == LipSyncProvider.uLipSync)
            {
                var lipSync = avatar.AddComponent<uLipSync.uLipSync>();
                lipSync.outputSoundGain = 0;
                var settings = (uLipSyncSettings)ApplicationPersistence.AppSettings.LipSyncSettings[LipSyncProvider.uLipSync];
                switch (settings.Profile)
                {
                    case LipSyncProfile.Default:
                        lipSync.profile = ProfileDefault;
                        break;
                    case LipSyncProfile.Female:
                        lipSync.profile = ProfileFemale;
                        break;
                    case LipSyncProfile.Male:
                        lipSync.profile = ProfileMale;
                        break;
                    case LipSyncProfile.Custom:
                        break;
                }

                avatar.AddComponent<AudioSource>();
                var lipSyncMic = avatar.AddComponent<uLipSyncMicrophone>();
                lipSyncMic.index = MicUtil.GetDeviceList().Find(md => md.name == ApplicationPersistence.AppSettings.Microphone).index;

                var lipSyncVRM = avatar.AddComponent<uLipSyncBlendShapeVRM>();
                lipSyncVRM.usePhonemeBlend = true;
                lipSyncVRM.minVolume = settings.MinVolume;
                lipSyncVRM.maxVolume = settings.MaxVolume;
                lipSyncVRM.AddBlendShape("A", "A");
                lipSyncVRM.AddBlendShape("I", "I");
                lipSyncVRM.AddBlendShape("U", "U");
                lipSyncVRM.AddBlendShape("E", "E");
                lipSyncVRM.AddBlendShape("O", "O");
                //lipSyncVRM.AddBlendShape("-", "Neutral");

                lipSync.onLipSyncUpdate.AddListener(lipSyncVRM.OnLipSyncUpdate);
            }
            ranOnFirstFrame = false;

            foreach (var bundle in ApplicationState.AvatarBundles.Where(kp => kp.Key != ApplicationState.CurrentAvatar.Guid).ToList())
            {
                DestroyImmediate(bundle.Value.Object, true);
                await bundle.Value.Bundle.UnloadAsync(true);
                ApplicationState.AvatarBundles.Remove(bundle.Key);
            }

            foreach (var vrm in ApplicationState.VrmData.Where(kp => kp.Key != ApplicationState.CurrentAvatar.Guid).ToList())
            {
                vrm.Value.Data.Dispose();
                vrm.Value.Dispose();
                ApplicationState.VrmData.Remove(vrm.Key);
            }

            await Resources.UnloadUnusedAssets();
        }

        private void OnFirstFrame()
        {
            ranOnFirstFrame = true;
            BuildAnimationMapping(avatar);
            foreach (var anim in Animations)
            {
                var config = ApplicationState.CurrentAvatar.Blendshapes.GetValueOrDefault(anim.Key.Name);
                if (config == null)
                {
                    config = new AvatarBlendshape();
                    if (anim.Value.HasValue)
                    {
                        config.Type = AvatarBlendshapeType.Additive;
                    }
                    else
                    {
                        config.Type = AvatarBlendshapeType.Base;
                    }
                    ApplicationState.CurrentAvatar.Blendshapes.Add(anim.Key.Name, config);
                }
                Driver.expressions.Add(new OpenSeeVRMDriver.OpenSeeVRMExpression(anim.Key.Name, anim.Key, 1, 1, 1, true, true, 1, anim.Value.HasValue ? anim.Value.Value.animId : -1, config));
            }
            ApplicationPersistence.Save();
            Driver.InitExpressionMap();
            MainThreadDispatcher.Instance.AddOnUpdate(() =>
            {
                LeftWristTarget.rotation = Quaternion.Euler(0, 0, 65);
                RightWristTarget.rotation = Quaternion.Euler(0, 0, -65);
            });
        }

        public void BuildAnimationMapping(GameObject avatar)
        {
            var proxy = avatar.GetComponent<VRMBlendShapeProxy>();
            var animator = avatar.GetComponent<Animator>();
            var vsfAnimations = avatar.GetComponent<VSF_Animations>();

            graph = PlayableGraph.Create("Avatar Animation " + UnityEngine.Random.Range(0, 1000000));
            var output = AnimationPlayableOutput.Create(graph, "BaseOutput", animator);
            mixer = AnimationMixerPlayable.Create(graph, 1);
            output.SetSourcePlayable(mixer);

            var tposePlayable = AnimationClipPlayable.Create(graph, TPose);
            tposePlayable.SetApplyFootIK(false);
            mixer.SetInputCount(1);
            mixer.ConnectInput(0, tposePlayable, 0);
            mixer.SetInputWeight(0, 1);

            foreach (var item in proxy.GetValues())
            {
                if (string.Equals(item.Key.Name, "A", StringComparison.InvariantCultureIgnoreCase) ||
                   string.Equals(item.Key.Name, "I", StringComparison.InvariantCultureIgnoreCase) ||
                   string.Equals(item.Key.Name, "U", StringComparison.InvariantCultureIgnoreCase) ||
                   string.Equals(item.Key.Name, "E", StringComparison.InvariantCultureIgnoreCase) ||
                   string.Equals(item.Key.Name, "O", StringComparison.InvariantCultureIgnoreCase) ||
                   string.Equals(item.Key.Name, "Blink", StringComparison.InvariantCultureIgnoreCase) ||
                   string.Equals(item.Key.Name, "LookUp", StringComparison.InvariantCultureIgnoreCase) ||
                   string.Equals(item.Key.Name, "LookDown", StringComparison.InvariantCultureIgnoreCase) ||
                   string.Equals(item.Key.Name, "LookLeft", StringComparison.InvariantCultureIgnoreCase) ||
                   string.Equals(item.Key.Name, "LookRight", StringComparison.InvariantCultureIgnoreCase) ||
                   string.Equals(item.Key.Name, "Blink_L", StringComparison.InvariantCultureIgnoreCase) ||
                   string.Equals(item.Key.Name, "Blink_R", StringComparison.InvariantCultureIgnoreCase))
                    continue;
                Animations.Add(item.Key, null);
            }

            if (vsfAnimations != null)
            {
                for (int i = 0; i < vsfAnimations.animations.Length; i++)
                {
                    var anim = vsfAnimations.animations[i];
                    if (anim.animation == null)
                        continue;
                    int animId = i + 1;
                    var shapeKey = BlendShapeKey.CreateUnknown(anim.blendshapeName);

                    var clip = AnimationClipPlayable.Create(graph, anim.animation);
                    clip.SetApplyFootIK(false);

                    mixer.SetInputCount(animId + 1);
                    mixer.ConnectInput(animId, clip, 0);
                    mixer.SetInputWeight(animId, 0);

                    if (!Animations.ContainsKey(shapeKey))
                        Animations.Add(shapeKey, new VsfAnimationDetails(animId, anim.animation.humanMotion));
                    else
                        Animations[shapeKey] = new VsfAnimationDetails(animId, anim.animation.humanMotion);
                }
            }

            graph.Play();
            Driver.mixer = mixer;
        }

        public void Update()
        {
            if (!ranOnFirstFrame)
                OnFirstFrame();
        }

        public void OnDestroy()
        {
#if UNITY_STANDALONE_WIN
            RawInput.Stop();
#endif
            if (graph.IsValid())
                graph.Destroy();
            if (mixer.IsValid())
                mixer.Destroy();
            if (vrm != null)
            {
                vrm.Dispose();
                vrm = null;
            }
            ApplicationState.CurrentAvatar = null;
        }

        public void OnApplicationQuit()
        {
#if UNITY_STANDALONE_WIN
            RawInput.Stop();
#endif
            if (graph.IsValid())
                graph.Destroy();
            if (mixer.IsValid())
                mixer.Destroy();
            if (vrm != null)
            {
                vrm.Dispose();
                vrm = null;
            }
            ApplicationState.CurrentAvatar = null;
        }

    }

    public struct VsfAnimationDetails
    {
        public int animId { get; }
        public bool humanMotion { get; }

        public VsfAnimationDetails(int animId, bool humanMotion)
        {
            this.animId = animId;
            this.humanMotion = humanMotion;
        }

    }
}
