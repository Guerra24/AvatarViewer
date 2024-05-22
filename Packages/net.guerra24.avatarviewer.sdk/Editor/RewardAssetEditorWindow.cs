using System.IO;
using UnityEditor;
using UnityEngine;

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

            var bundleIndex = EditorGUILayout.Popup(0, bundles);

            var assets = AssetDatabase.GetAssetPathsFromAssetBundle(bundles[bundleIndex]);

            GUILayout.Label("Assets", EditorStyles.boldLabel);
            bool valid = true;
            foreach (var asset in assets)
            {
                var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(asset);
                var hasRewardAsset = gameObject.TryGetComponent<RewardAsset>(out var rewardAsset);
                var hasRigidBody = gameObject.TryGetComponent<Rigidbody>(out var rigidbody);
                var hasComponents = hasRewardAsset && hasRigidBody;
                if (!hasComponents)
                    valid = false;

                if (EditorGUILayout.Foldout(true, hasRewardAsset ? rewardAsset.Name : gameObject.name))
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(16f);
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.ObjectField(gameObject, typeof(GameObject), false);
                    if (!hasRewardAsset)
                        GUILayout.Label("Missing RewardAsset component");
                    if(!hasRigidBody)
                        GUILayout.Label("Missing RigidBody component");
                    if (hasComponents)
                        GUILayout.Label("Reward valid");
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
            }

            GUILayout.Label("Export settings", EditorStyles.boldLabel);
            if (valid)
            {
                if (GUILayout.Button("Export"))
                    Export(assets);
            }
            else
            {
                GUILayout.Label("Mssing components");
            }
        }

        private void Export(string[] assets)
        {
            var filepath = EditorUtility.SaveFilePanel("Export Reward Bundle", ".", "rewards", "avr");
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
