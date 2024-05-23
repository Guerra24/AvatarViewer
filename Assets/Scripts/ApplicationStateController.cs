using UnityEngine;

namespace AvatarViewer
{
    public class ApplicationStateController : MonoBehaviour
    {
        private void OnDestroy() => DestroyData();

        private void OnApplicationQuit() => DestroyData();

        private void DestroyData()
        {
            foreach (var sound in ApplicationState.ExternalAudios)
                Destroy(sound.Value);
            ApplicationState.ExternalAudios.Clear();
            foreach (var avatar in ApplicationState.VrmData)
            {
                avatar.Value.Data.Dispose();
                avatar.Value.Dispose();
            }
            ApplicationState.VrmData.Clear();
            foreach (var avatar in ApplicationState.AvatarBundles)
                avatar.Value.Bundle.Unload(true);
            ApplicationState.AvatarBundles.Clear();
            foreach (var bundle in ApplicationState.RewardBundles)
                bundle.Value.Bundle.Unload(true);
            ApplicationState.RewardBundles.Clear();
        }
    }
}
