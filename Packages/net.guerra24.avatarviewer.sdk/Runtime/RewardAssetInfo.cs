using UnityEngine;

namespace AvatarViewer.SDK
{
    [CreateAssetMenu(fileName = "RewardAssetInfo", menuName = "Avatar Viewer/RewardAssetInfo")]
    public class RewardAssetInfo : ScriptableObject
    {
        public SerializableGuid Guid;

        public string AssetName;

        public bool DisablePhysicsOnCollision;

        public GameObject Prefab;

    }
}
