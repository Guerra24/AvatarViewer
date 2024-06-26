using UnityEditor;
using UnityEditor.SceneManagement;

namespace AvatarViewer.Editor
{
    [InitializeOnLoad]
    public static class EditorInitializer
    {
        static EditorInitializer()
        {
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/Startup.unity");
        }
    }
}
