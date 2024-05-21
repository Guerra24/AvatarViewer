using System;
using UnityEngine;

namespace AvatarViewer.SDK
{
    public class RewardAsset : MonoBehaviour
    {
        public SerializableGuid Guid;

        public string Name;

        private Rigidbody Rigidbody;
        private AudioSource AudioSource;

        private void Start()
        {
            Rigidbody = GetComponent<Rigidbody>();
            AudioSource = GetComponent<AudioSource>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            //Rigidbody.isKinematic = true;

            AudioSource.Play();
        }
    }
}
