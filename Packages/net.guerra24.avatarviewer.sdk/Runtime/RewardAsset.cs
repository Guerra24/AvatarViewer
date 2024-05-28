using UnityEngine;

namespace AvatarViewer.SDK
{
    public class RewardAsset : MonoBehaviour
    {
        public SerializableGuid Guid;

        public string Name;

        public bool DisablePhysicsOnCollision;

        private Rigidbody Rigidbody;
        private AudioSource AudioSource;

        private void Start()
        {
            Rigidbody = GetComponent<Rigidbody>();
            AudioSource = GetComponent<AudioSource>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (DisablePhysicsOnCollision)
            {
                Rigidbody.isKinematic = true;
                Rigidbody.detectCollisions = false;
                transform.parent = collision.transform;
            }

            AudioSource.Play();
        }
    }
}
