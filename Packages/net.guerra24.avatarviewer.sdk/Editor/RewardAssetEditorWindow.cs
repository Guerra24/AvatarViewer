using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace AvatarViewer.SDK.Editor
{
    public class RewardAssetEditorWindow : EditorWindow
    {
        [MenuItem("Avatar Viewer/Export reward bundle")]
        public static void ExportRewardsBundle()
        {
            GetWindow<RewardAssetEditorWindow>();
        }

        private string[] bundles = new string[] { };

        private void Awake()
        {
            titleContent = new GUIContent("Export reward bundle");
        }

        private void OnGUI()
        {
            GUILayout.Label("Asset bundle", EditorStyles.boldLabel);

            var bundles = AssetDatabase.GetAllAssetBundleNames();

            if (bundles.Length == 0)
            {
                GUILayout.Label("No asset bundles found");
                return;
            }

            var bundleIndex = EditorGUILayout.Popup(0, bundles);
            var bundle = bundles[bundleIndex];

            var assets = AssetDatabase.GetAssetPathsFromAssetBundle(bundle);

            GUILayout.Label("Assets", EditorStyles.boldLabel);
            bool valid = true, updatedAssets = false;
            foreach (var asset in assets)
            {
                var rewardAssetInfo = AssetDatabase.LoadAssetAtPath<RewardAssetInfo>(asset);
                if (rewardAssetInfo != null)
                {
                    if (EditorGUILayout.Foldout(true, rewardAssetInfo.AssetName))
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(16f);
                        EditorGUILayout.BeginVertical();
                        if (rewardAssetInfo.Prefab != null)
                        {
                            var hasRewardAsset = rewardAssetInfo.Prefab.TryGetComponent<RewardAsset>(out var rewardAsset);
                            var hasRigidBody = rewardAssetInfo.Prefab.TryGetComponent<Rigidbody>(out var rigidbody);
                            var hasComponents = hasRewardAsset && hasRigidBody;
                            if (!hasComponents)
                                valid = false;

                            UnityEditor.Editor.CreateEditor(rewardAssetInfo).OnInspectorGUI();
                            if (!hasRewardAsset)
                                GUILayout.Label("Missing RewardAsset component");
                            if (!hasRigidBody)
                                GUILayout.Label("Missing RigidBody component");

                            if (hasRewardAsset && rewardAsset.Info != rewardAssetInfo)
                            {
                                rewardAsset.Info = rewardAssetInfo;
                                EditorUtility.SetDirty(rewardAsset);
                                updatedAssets = true;
                            }
                        }
                        else
                        {
                            valid = false;
                            GUILayout.Label("Missing Prefab");
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }

            if (updatedAssets)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            GUILayout.Label("Export", EditorStyles.boldLabel);
            if (valid)
            {
                bool showExport = true;
                string errors = "";

                if (PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64))
                {
                    errors += "Using Auto Graphics for Windows\n";
                    showExport = false;
                }
                if (PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.StandaloneLinux64))
                {
                    errors += "Using Auto Graphics for Linux\n";
                    showExport = false;
                }
                if (PlayerSettings.colorSpace != ColorSpace.Linear)
                {
                    errors += "Using non linear color space\n";
                    showExport = false;
                }

                if (showExport && GUILayout.Button("Export"))
                    Export(bundle, assets);
                if (!showExport)
                {
                    GUILayout.Label(errors);
                    if (GUILayout.Button("Fix errors"))
                    {
                        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64, false);
                        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneLinux64, false);
                        PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64, new GraphicsDeviceType[] { GraphicsDeviceType.Direct3D11, GraphicsDeviceType.Direct3D12 });
                        PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneLinux64, new GraphicsDeviceType[] { GraphicsDeviceType.Vulkan, GraphicsDeviceType.OpenGLCore });
                        PlayerSettings.colorSpace = ColorSpace.Linear;
                    }
                }
            }
            else
            {
                GUILayout.Label("Mssing components");
            }
        }

        private void Export(string bundle, string[] assets)
        {
            var filepath = EditorUtility.SaveFilePanel("Export Reward Bundle", ".", bundle, "avr");
            if (string.IsNullOrEmpty(filepath))
                return;

            var filename = Path.GetFileName(filepath);

            var bundleBuild = new AssetBundleBuild
            {
                assetBundleName = filename,
                assetNames = assets
            };

            var options = BuildAssetBundleOptions.ForceRebuildAssetBundle | BuildAssetBundleOptions.StrictMode | BuildAssetBundleOptions.ChunkBasedCompression;

            var temporaryFile = Path.Combine(Application.temporaryCachePath, filename);

            BuildPipeline.BuildAssetBundles(Application.temporaryCachePath, new AssetBundleBuild[] { bundleBuild }, options, EditorUserBuildSettings.activeBuildTarget);

            if (File.Exists(filepath))
                File.Delete(filepath);

            File.Move(temporaryFile, filepath);
            File.Delete(temporaryFile);

            EditorUtility.DisplayDialog("Export", "Export complete!", "OK");
        }
    }
}
