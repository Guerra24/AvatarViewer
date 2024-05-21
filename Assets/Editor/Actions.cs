using UnityEditor;

namespace AvatarViewer.Editor
{
    public static class Actions
    {
        [MenuItem("Avatar Viewer/Build rewards")]
        public static void BuildRewards()
        {
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", BuildAssetBundleOptions.StrictMode | BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        }
    }
}
