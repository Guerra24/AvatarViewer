using UnityEngine;

namespace AvatarViewer.SDK
{
    public class RewardAsset : MonoBehaviour
    {
        [HideInInspector] public RewardAssetInfo Info;

        private Rigidbody Rigidbody;
        private AudioSource AudioSource;

        private void Start()
        {
            Rigidbody = GetComponent<Rigidbody>();
            AudioSource = GetComponent<AudioSource>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (Info.DisablePhysicsOnCollision)
            {
                Rigidbody.isKinematic = true;
                Rigidbody.detectCollisions = false;
                transform.parent = collision.transform;
            }

            AudioSource.Play();
        }
    }
}
