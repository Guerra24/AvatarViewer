using AvatarViewer.SDK;
using UnityEditor;

namespace AvatarViewer.Editor
{
    public static class Actions
    {
        [MenuItem("Avatar Viewer/Generate built-in rewards")]
        public static void BuildRewards()
        {
            foreach (var asset in AssetDatabase.GetAssetPathsFromAssetBundle("builtin-rewards"))
            {
                var rewardAssetInfo = AssetDatabase.LoadAssetAtPath<RewardAssetInfo>(asset);
                var rewardAsset = rewardAssetInfo.Prefab.GetComponent<RewardAsset>();
                rewardAsset.Info = rewardAssetInfo;
                EditorUtility.SetDirty(rewardAsset);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", BuildAssetBundleOptions.StrictMode | BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        }
    }
}
