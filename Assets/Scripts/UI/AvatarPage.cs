using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRM;

namespace AvatarViewer.Ui
{
    public class AvatarPage : MonoBehaviour
    {

        [SerializeField] private Image Thumbnail;
        [SerializeField] private TMP_Text Title;
        [SerializeField] private TMP_Text Version;
        [SerializeField] private TMP_Text Author;
        [SerializeField] private TMP_Text Contact;
        [SerializeField] private TMP_Text AllowedUser;
        [SerializeField] private TMP_Text VUsage;
        [SerializeField] private TMP_Text SUsage;
        [SerializeField] private TMP_Text CUsage;
        [SerializeField] private TMP_Text OtherPermission;
        [SerializeField] private TMP_Text License;
        [SerializeField] private TMP_Text OtherLicense;

        private PageViewerPage page;
        private Avatar avatar;

        private void Awake()
        {
            avatar = ApplicationState.CurrentAvatar;
            page = GetComponent<PageViewerPage>();
            page.Title = avatar.Title;
        }

        private void Start()
        {
            VRMMetaObject meta;
            if (!avatar.Vrm)
            {
                var bundle = ApplicationState.AvatarBundles[avatar.Guid];
                meta = bundle.Object.GetComponent<VRMMeta>().Meta;
            }
            else
            {
                meta = ApplicationState.VrmData[avatar.Guid].ReadMeta(true);
            }

            if (meta.Thumbnail != null)
            {
                var thumb = meta.Thumbnail;
                Thumbnail.sprite = Sprite.Create(thumb, new Rect(0, 0, thumb.width, thumb.height), new Vector2(0.5f, 0.5f), 100, 1, SpriteMeshType.FullRect);
                Thumbnail.GetComponent<AspectRatioFitter>().aspectRatio = (float)thumb.width / thumb.height;
            }

            Title.text = avatar.Title;
            Version.text = meta.Version;
            Author.text = meta.Author;
            Contact.text = meta.ContactInformation;
            AllowedUser.text = meta.AllowedUser.ToString();
            VUsage.text = meta.ViolentUssage.ToString();
            SUsage.text = meta.SexualUssage.ToString();
            CUsage.text = meta.CommercialUssage.ToString();
            OtherPermission.text = meta.OtherPermissionUrl;
            License.text = meta.LicenseType.ToString();
            OtherLicense.text = meta.OtherLicenseUrl;
        }

        public void StartTracker()
        {
            StartTrackerAsync().Forget();
        }

        private async UniTaskVoid StartTrackerAsync()
        {
            Debug.Log("Start");
            ApplicationState.CurrentAvatar = avatar;
            var drillIn = GetComponentInParent<PageDrillIn>();
            drillIn.Easing = AnimationEasing.EaseOut;
            await drillIn.StartAnimation().ToUniTask();

            SceneLoader.Scene = Scene.Main;
            await SceneManager.LoadSceneAsync("Scenes/Loader");
        }
    }
}
