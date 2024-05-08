using TMPro;
using UnityEngine;

public class AboutPage : MonoBehaviour
{

    [SerializeField] private TMP_Text Version;
    [SerializeField] private TMP_Text UnityVersion;


    private void Awake()
    {
        Version.text = Application.version;
        UnityVersion.text = Application.unityVersion;
    }
}
