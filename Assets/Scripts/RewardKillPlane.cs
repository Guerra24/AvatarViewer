using UnityEngine;

namespace AvatarViewer
{
    public class RewardKillPlane : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Destroy(other.gameObject);
        }
    }
}
