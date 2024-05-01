using UnityEngine;

namespace AvatarViewer
{
    public class ApplicationStateController : MonoBehaviour
    {
        private void OnApplicationQuit()
        {
            foreach (var sound in ApplicationState.ExternalAudios)
                Destroy(sound.Value);
            foreach (var avatar in ApplicationState.VrmData)
            {
                avatar.Value.Dispose();
                avatar.Value.Data.Dispose();
            }
            foreach (var avatar in ApplicationState.AvatarBundles)
                avatar.Value.Bundle.Unload(true);
        }
    }
}
