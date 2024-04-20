using UnityEngine;

namespace AvatarViewer
{
    public class RewardController : MonoBehaviour
    {

        private Rigidbody Rigidbody;

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("Hit");
            //Rigidbody.isKinematic = true;
        }
    }
}
