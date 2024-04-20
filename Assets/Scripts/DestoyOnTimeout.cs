using System.Collections;
using UnityEngine;

namespace AvatarViewer
{
    public class DestoyOnTimeout : MonoBehaviour
    {
        public float Seconds;

        private void Start()
        {
            StartCoroutine(DestroySelf());
        }

        IEnumerator DestroySelf()
        {
            yield return new WaitForSecondsRealtime(Seconds);
            Destroy(gameObject);
        }

    }
}
