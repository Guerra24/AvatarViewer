using UnityEditor;

namespace AvatarViewer.Editor
{
    public static class Actions
    {
        [MenuItem("Avatar Viewer/Generate built-in rewards")]
        public static void BuildRewards()
        {
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", BuildAssetBundleOptions.StrictMode | BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        }
    }
}
