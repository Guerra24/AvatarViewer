using System;
using System.Collections.Generic;
using System.Linq;
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
using VSeeFace;

namespace AvatarViewer
{
    public class AvatarLoader : MonoBehaviour
    {

        private GameObject avatar;

        [Header("Driver")]
        public OpenSeeVRMDriver Driver;
        [Header("IK Target at Head")]
        public Transform HeadTarget;
        [Header("Kinematic target")]
        public Transform KinematicInterpolation;
        [Header("Left wrist target")]
        public Transform LeftWrist;
        [Header("Right wrist target")]
        public Transform RightWrist;
        [Header("Default T Pose")]
        public AnimationClip TPose;
        [Header("Camera position")]
        public Transform CameraPosition;
        [Header("Rewards")]
        public Transform Rewards;
        [Header("uLipSync Profile Default")]
        public Profile ProfileDefault;
        [Header("uLipSync Profile Female")]
        public Profile ProfileFemale;
        [Header("uLipSync Profile Male")]
        public Profile ProfileMale;

        private PlayableGraph graph;
        private AnimationMixerPlayable mixer;
        //private VRMBlendShapeProxy proxy;

        //private string defaultExpression = "Neutral";
        private BlendShapeKey defaultBlendShapeKey = BlendShapeKey.CreateFromPreset(BlendShapePreset.Neutral);

        //private string currentExpression = "Neutral";
        //private BlendShapeKey currentBlendShapeKey = BlendShapeKey.CreateFromPreset(BlendShapePreset.Neutral);
        private List<BlendShapeKey> activeBlendshapeKeys = new();

        private bool triggered, ranOnFirstFrame;

        private Dictionary<BlendShapeKey, VsfAnimationDetails?> Animations = new();

        private RuntimeGltfInstance vrm;

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
            if (!ApplicationState.CurrentAvatar.Vrm)
            {

                var prefab = ApplicationState.AvatarBundles[ApplicationState.CurrentAvatar.Guid].Object;

                avatar = Instantiate(prefab);
            }
            else
            {

                vrm = ApplicationState.VrmData[ApplicationState.CurrentAvatar.Guid].Load();
                vrm.ShowMeshes();
                vrm.EnableUpdateWhenOffscreen();

                avatar = vrm.gameObject;
            }

            var animator = avatar.GetComponent<Animator>();

            Driver.vrmBlendShapeProxy = avatar.GetComponent<VRMBlendShapeProxy>();
            Driver.leftEye = animator.GetBoneTransform(HumanBodyBones.LeftEye);
            Driver.rightEye = animator.GetBoneTransform(HumanBodyBones.RightEye);
            var head = animator.GetBoneTransform(HumanBodyBones.Head);
            var neck = animator.GetBoneTransform(HumanBodyBones.Neck);

            HeadTarget.position = head.position;
            HeadTarget.position += new Vector3(0, 0.1f, 0);

            var ik = head.gameObject.AddComponent<FastIKFabric>();
            ik.ChainLength = 4;
            ik.Target = KinematicInterpolation;

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
            lwik.Target = LeftWrist;

            var rightWrist = animator.GetBoneTransform(HumanBodyBones.RightHand);
            var rwik = rightWrist.gameObject.AddComponent<FastIKFabric>();
            rwik.ChainLength = 2;
            rwik.Target = RightWrist;

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

            /*var handTracker = GetComponent<HandTracker>();
            handTracker.LeftWrist = LeftWrist;
            handTracker.LeftThumb = ltd.Target;
            handTracker.LeftIndex = lid.Target;
            handTracker.LeftMiddle = lmd.Target;
            handTracker.LeftRing = lrd.Target;
            handTracker.LeftPinky = lld.Target;
            handTracker.RightWrist = RightWrist;*/

            /*var vrm = await VrmUtility.LoadAsync(Path.Combine(Application.streamingAssetsPath, "Dani_El_Axolote_1.3.0.vrm"));
            vrm.ShowMeshes();
            vrm.EnableUpdateWhenOffscreen();*/

            var yReference = neck.position.y + ((head.position.y - neck.position.y) * 2f);

            CameraPosition.position = new Vector3(0, yReference, 1.91f);
            Rewards.position = new Vector3(0, yReference, 0);

            if (ApplicationPersistence.AppSettings.LipSyncProvider == LipSyncProvider.uLipSync)
            {
                var lipSync = avatar.AddComponent<uLipSync.uLipSync>();
                lipSync.outputSoundGain = 0;
                switch (ApplicationPersistence.AppSettings.LipSyncProfile)
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
                lipSyncVRM.minVolume = -1.8f;
                lipSyncVRM.maxVolume = -0.5f;
                lipSyncVRM.AddBlendShape("A", "A");
                lipSyncVRM.AddBlendShape("I", "I");
                lipSyncVRM.AddBlendShape("U", "U");
                lipSyncVRM.AddBlendShape("E", "E");
                lipSyncVRM.AddBlendShape("O", "O");
                //lipSyncVRM.AddBlendShape("-", "Neutral");

                lipSync.onLipSyncUpdate.AddListener(lipSyncVRM.OnLipSyncUpdate);
            }
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
#if UNITY_STANDALONE_WIN
            /*
            if (RawInput.PressedKeys.Count > 0 && !triggered)
                foreach (var blendshape in ApplicationState.CurrentAvatar.Blendshapes)
                {
                    if (blendshape.Value.Hotkey.Count > 0 && RawInput.PressedKeys.Intersect(blendshape.Value.Hotkey).Count() == blendshape.Value.Hotkey.Count)
                    {
                        var anim = Animations.First(vp => vp.Key.Name == blendshape.Key).Key;
                        if (activeBlendshapeKeys.Contains(anim))
                            activeBlendshapeKeys.Remove(anim);
                        else
                            activeBlendshapeKeys.Add(anim);
                        triggered = true;
                    }
                }

            if (RawInput.PressedKeys.Count == 0 && triggered)
            {
                mixer.SetInputWeight(0, 1);
                foreach (var shape in Animations)
                {
                    if (activeBlendshapeKeys.Contains(shape.Key))
                    {
                        proxy.ImmediatelySetValue(shape.Key, 1f);

                        if (shape.Value.HasValue)
                        {
                            var animDetails = shape.Value.Value;
                            mixer.SetInputWeight(animDetails.animId, 1f);
                            mixer.GetInput(animDetails.animId).SetTime(0);
                            if (animDetails.humanMotion)
                                mixer.SetInputWeight(0, 0);
                        }
                    }
                    else
                    {
                        proxy.ImmediatelySetValue(shape.Key, 0f);
                        if (shape.Value.HasValue)
                            mixer.SetInputWeight(shape.Value.Value.animId, 0);
                    }
                }
                triggered = false;
            }*/
#endif
        }

        public void OnDestroy()
        {
#if UNITY_STANDALONE_WIN
            RawInput.Stop();
#endif
            if (graph.IsValid())
                graph.Destroy();
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
