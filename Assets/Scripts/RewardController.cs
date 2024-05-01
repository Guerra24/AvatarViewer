using UnityEngine;

namespace AvatarViewer
{
    public class RewardController : MonoBehaviour
    {

        private Rigidbody Rigidbody;
        private AudioSource AudioSource;

        private void Start()
        {
            Rigidbody = GetComponent<Rigidbody>();
            AudioSource = GetComponent<AudioSource>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("Hit");
            //Rigidbody.isKinematic = true;

            AudioSource.Play();
        }
    }
}
