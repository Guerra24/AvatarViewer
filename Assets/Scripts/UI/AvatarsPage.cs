using System;
using System.Collections;
using System.IO;
using System.Linq;
using UniGLTF;
using UnityEngine;
using VRM;

namespace AvatarViewer.Ui
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
                StartCoroutine(AddAvatar(result.Path));
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

                    CreateItem(avatar);
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
