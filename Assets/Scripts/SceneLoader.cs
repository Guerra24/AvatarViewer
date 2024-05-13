using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniGLTF;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRM;

namespace AvatarViewer
{
    public class SceneLoader : MonoBehaviour
    {

        public static Scene Scene;

        //private PageDrillIn PageDrillIn;

        /*private void Awake()
        {
            PageDrillIn = GetComponent<PageDrillIn>();
            GetComponent<CanvasGroup>().alpha = 0;
        }*/

        private void Start()
        {
            StartAsync().Forget();
        }

        private async UniTaskVoid StartAsync()
        {
            //await PageDrillIn.StartAnimation().ToUniTask();

            //PageDrillIn.Easing = AnimationEasing.EaseOut;

            await UniTask.Yield();
            await UniTask.NextFrame();
            {
                var scene = "";
                switch (Scene)
                {
                    case Scene.Menu:
                        scene = "Scenes/Menu";

                        var tasks = new List<UniTask>();
                        foreach (var avatar in ApplicationPersistence.AppSettings.Avatars.Where(v => !ApplicationState.AvatarBundles.ContainsKey(v.Guid) && !v.Vrm).ToList())
                            tasks.Add(AssetBundle.LoadFromFileAsync(avatar.Path).ToUniTask().ContinueWith(async (assetBundle) => await assetBundle.LoadAssetAsync<GameObject>("VSFAvatar").ToUniTask().ContinueWith((asset) => ApplicationState.AvatarBundles.Add(avatar.Guid, new LoadedAvatar(assetBundle, asset as GameObject)))));
                        await UniTask.WhenAll(tasks);

                        foreach (var vrm in ApplicationState.VrmData.ToList())
                        {
                            vrm.Value.Dispose();
                            ApplicationState.VrmData.Remove(vrm.Key);
                        }

                        foreach (var avatar in ApplicationPersistence.AppSettings.Avatars.Where(v => !ApplicationState.VrmData.ContainsKey(v.Guid) && v.Vrm).ToList())
                            ApplicationState.VrmData.Add(avatar.Guid, new VRMImporterContext(new VRMData(new AutoGltfFileParser(avatar.Path).Parse())));

                        break;
                    case Scene.Main:
                        scene = "Scenes/Main";

                        foreach (var bundle in ApplicationState.AvatarBundles.Where(kp => kp.Key != ApplicationState.CurrentAvatar.Guid).ToList())
                        {
                            DestroyImmediate(bundle.Value.Object, true);
                            await bundle.Value.Bundle.UnloadAsync(true);
                            ApplicationState.AvatarBundles.Remove(bundle.Key);
                        }

                        foreach (var vrm in ApplicationState.VrmData.Where(kp => kp.Key != ApplicationState.CurrentAvatar.Guid).ToList())
                        {
                            vrm.Value.Dispose();
                            ApplicationState.VrmData.Remove(vrm.Key);
                        }

                        break;
                }
                await SceneManager.LoadSceneAsync(scene);
                /*op.allowSceneActivation = false;

                while (!op.isDone)
                {
                    if (op.progress >= 0.9f)
                    {
                        //await PageDrillIn.StartAnimation().ToUniTask();
                        op.allowSceneActivation = true;
                    }
                    await UniTask.Yield();
                    await UniTask.NextFrame();
                }*/
            }
        }
    }

    public enum Scene
    {
        Menu, Main
    }
}
