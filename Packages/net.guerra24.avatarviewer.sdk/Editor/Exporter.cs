﻿using System.IO;
using UnityEditor;
using UnityEngine;
using VSeeFace;

namespace AvatarViewer.SDK.Editor
{
    public static class Exporter
    {
        [MenuItem("Avatar Viewer/Export avatar bundle")]
        public static void ExportAvatarBundle()
        {
            GameObject obj = Selection.activeGameObject;
            string error;
            if (!AvatarCheck.CheckAvatar(obj, out error))
            {
                EditorUtility.DisplayDialog("Export Avatar Bundle", error, "OK");
                return;
            }

            string fullpath = EditorUtility.SaveFilePanel("Export Avatar Bundle", ".", obj.name, "ava");
            if (string.IsNullOrEmpty(fullpath))
                return;

            string filename = Path.GetFileName(fullpath);

            bool complete = false;
            string prefabPath = "Assets/AvATemporary.prefab";
            try
            {
                AssetDatabase.DeleteAsset(prefabPath);
                if (File.Exists(prefabPath))
                    File.Delete(prefabPath);

                bool succeededPack = false;
                PrefabUtility.SaveAsPrefabAsset(obj, prefabPath, out succeededPack);
                if (!succeededPack)
                {
                    Debug.Log("Prefab creation failed");
                    return;
                }

                AssetBundleBuild bundleBuild = new AssetBundleBuild();
                AssetDatabase.RemoveUnusedAssetBundleNames();
                bundleBuild.assetBundleName = filename;
                bundleBuild.assetNames = new string[] { prefabPath };
                bundleBuild.addressableNames = new string[] { "VSFAvatar" };

                BuildAssetBundleOptions options = BuildAssetBundleOptions.ForceRebuildAssetBundle | BuildAssetBundleOptions.StrictMode | BuildAssetBundleOptions.ChunkBasedCompression;
                if (obj.GetComponentsInChildren<UnityEngine.Video.VideoPlayer>(true).Length > 0)
                {
                    Debug.Log("VideoPlayer detected, using uncompressed asset bundle.");
                    options |= BuildAssetBundleOptions.UncompressedAssetBundle;
                }
                BuildPipeline.BuildAssetBundles(Application.temporaryCachePath, new AssetBundleBuild[] { bundleBuild }, options, EditorUserBuildSettings.activeBuildTarget);
                if (File.Exists(fullpath))
                    File.Delete(fullpath);

                File.Move(Path.Combine(Application.temporaryCachePath, filename), fullpath);

                EditorUtility.DisplayDialog("Export", "Export complete!", "OK");
                complete = true;
            }
            finally
            {
                try
                {
                    AssetDatabase.DeleteAsset(prefabPath);
                    if (File.Exists(prefabPath))
                        File.Delete(prefabPath);
                }
                catch { }

                if (!complete)
                    EditorUtility.DisplayDialog("Export", "Export failed! See the console for details.", "OK");
            }
        }

    }
}