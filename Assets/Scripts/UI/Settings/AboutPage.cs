using TMPro;
using UnityEngine;

namespace AvatarViewer.Ui
{
    public class AboutPage : MonoBehaviour
    {

        [SerializeField] private TMP_Text Version;
        [SerializeField] private TMP_Text UnityVersion;
        [SerializeField] private TextAsset License;

        [SerializeField] private GameObject _markdownDialog;

        private void Awake()
        {
            Version.text = Application.version;
            UnityVersion.text = Application.unityVersion;
        }

        public void ShowLicense()
        {
            var dialog = Instantiate(_markdownDialog, GameObject.Find("Canvas").transform, false);
            var data = dialog.GetComponentInChildren<MarkdownDialog>();
            data.SetContent(License.text);
        }
    }
}
