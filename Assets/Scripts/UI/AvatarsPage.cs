using System;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniGLTF;
using UnityEngine;
using VRM;

namespace AvatarViewer.UI
{
    public class AvatarsPage : MonoBehaviour
    {

        [SerializeField] private RectTransform List;
        [SerializeField] private GameObject _template;

        void Awake()
        {
            foreach (var avatar in ApplicationPersistence.AppSettings.Avatars)
                CreateItem(avatar);
        }

        public void AddAvatar()
        {
            var result = NativeFileDialogSharp.Dialog.FileOpen("ava,vsfavatar,vrm");
            if (result.IsOk)
                AddAvatar(result.Path).Forget();
        }

        public void RemoveAvatar()
        {
            var avatar = ApplicationState.CurrentAvatar;
            if (avatar == null)
                return;
            ApplicationState.CurrentAvatar = null;

            RemoveItem(avatar);

            if (ApplicationState.AvatarBundles.Remove(avatar.Guid, out var bundle))
                bundle.Bundle.UnloadAsync(true);
            ApplicationPersistence.AppSettings.Avatars.Remove(avatar);
            ApplicationPersistence.Save();
        }

        private async UniTaskVoid AddAvatar(string path)
        {

            if (string.Equals(Path.GetExtension(path), ".vsfavatar", StringComparison.OrdinalIgnoreCase) || string.Equals(Path.GetExtension(path), ".ava", StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log($"Loading bundle {path}");

                var assetBundle = await AssetBundle.LoadFromFileAsync(path);

                Debug.Log($"Loading asset");
                var prefab = (await assetBundle.LoadAssetAsync<GameObject>("VSFAvatar")) as GameObject;

                string error;
                if (VSeeFace.AvatarCheck.CheckAvatar(prefab, out error))
                {
                    var meta = prefab.GetComponent<VRMMeta>().Meta;

                    var avatar = new Avatar(meta.Title, path, false);
                    ApplicationPersistence.AppSettings.Avatars.Add(avatar);
                    ApplicationState.AvatarBundles.Add(avatar.Guid, new LoadedAvatar(assetBundle, prefab));
                    Debug.Log($"Added {meta.Title}");

                    ApplicationPersistence.Save();

                    CreateItem(avatar);
                }
                else
                {
                    await assetBundle.UnloadAsync(true);
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

                CreateItem(avatar);

                Destroy(meta);
            }
        }

        private void CreateItem(Avatar avatar)
        {
            var item = Instantiate(_template, List, false);
            item.GetComponent<AvatarListItem>().Load(avatar);
        }

        private void RemoveItem(Avatar avatar)
        {
            var items = List.GetComponentsInChildren<AvatarListItem>();

            Destroy(items.Where(i => i.Avatar.Equals(avatar)).First().gameObject);
        }
    }

}
