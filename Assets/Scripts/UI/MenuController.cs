using System;
using System.Collections;
using System.IO;
using Cysharp.Threading.Tasks;
using SimpleFileBrowser;
using UniGLTF;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRM;

namespace AvatarViewer.Ui
{
    public class MenuController : MonoBehaviour
    {

        public AvatarList List;

        public void StartTracker()
        {
            StartTrackerAsync().Forget();
        }

        private async UniTaskVoid StartTrackerAsync()
        {
            Debug.Log("Start");
            if (ApplicationState.CurrentAvatar != null)
            {
                var drillIn = GetComponent<PageDrillIn>();
                drillIn.Easing = AnimationEasing.EaseOut;
                await drillIn.StartAnimation().ToUniTask();

                SceneLoader.Scene = "Scenes/Main";
                await SceneManager.LoadSceneAsync("Scenes/Loader");
            }
        }

        public void AddAvatar()
        {
            FileBrowser.ShowLoadDialog(FilePicked, FileCanceled, FileBrowser.PickMode.Files);
        }

        public void RemoveAvatar()
        {
            var avatar = ApplicationState.CurrentAvatar;
            if (avatar == null)
                return;
            ApplicationState.CurrentAvatar = null;

            List.RemoveItem(avatar);

            if (ApplicationState.AvatarBundles.Remove(avatar.Guid, out var bundle))
                bundle.Bundle.UnloadAsync(true);
            ApplicationPersistence.AppSettings.Avatars.Remove(avatar);
            ApplicationPersistence.Save();
        }

        private void FilePicked(string[] files)
        {
            StartCoroutine(AddAvatar(files[0]));
        }

        IEnumerator AddAvatar(string path)
        {
            yield return null;

            if (string.Equals(Path.GetExtension(path), ".vsfavatar", StringComparison.OrdinalIgnoreCase))
            {
                var bundleOp = AssetBundle.LoadFromFileAsync(path);
                Debug.Log($"Loading bundle {path}");

                yield return bundleOp;

                var assetBundle = bundleOp.assetBundle;

                var assetOp = assetBundle.LoadAssetAsync<GameObject>("VSFAvatar");
                Debug.Log($"Loading asset");

                yield return assetOp;

                var prefab = assetOp.asset as GameObject;

                string error;
                if (VSeeFace.AvatarCheck.CheckAvatar(prefab, out error))
                {
                    var meta = prefab.GetComponent<VRMMeta>().Meta;

                    var avatar = new Avatar(meta.Title, path, false);
                    ApplicationPersistence.AppSettings.Avatars.Add(avatar);
                    ApplicationState.AvatarBundles.Add(avatar.Guid, new LoadedAvatar(assetBundle, prefab));
                    Debug.Log($"Added {meta.Title}");

                    ApplicationPersistence.Save();

                    List.CreateItem(avatar);
                }
                else
                {
                    yield return assetBundle.UnloadAsync(true);
                }
            }
            else
            {
                var loader = new VRMImporterContext(new VRMData(new AutoGltfFileParser(path).Parse()));
                var meta = loader.ReadMeta();

                var avatar = new Avatar(meta.Title, path, true);
                ApplicationPersistence.AppSettings.Avatars.Add(avatar);
                ApplicationState.VrmData.Add(avatar.Guid, loader);
                Debug.Log($"Added {meta.Title}");

                ApplicationPersistence.Save();

                List.CreateItem(avatar);

                Destroy(meta);
            }
        }

        private void FileCanceled()
        {

        }
    }

}
