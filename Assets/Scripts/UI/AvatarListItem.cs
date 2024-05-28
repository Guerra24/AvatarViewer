using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRM;

namespace AvatarViewer.UI
{
    public class AvatarListItem : Button
    {

        public Avatar Avatar { get; private set; }

        [SerializeField] private Image Thumbnail;
        [SerializeField] private TMP_Text Title;
        [SerializeField] private TMP_Text Version;
        [SerializeField] private TMP_Text Author;

        [SerializeField] private GameObject _templatePage;

        private PageViewer pageViewer;
        private float lastClick;

        protected override void Awake()
        {
            pageViewer = GetComponentInParent<PageViewer>();
            onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            if ((lastClick + 0.4f) > Time.time)
                pageViewer.OpenPage(_templatePage);
            ApplicationState.CurrentAvatar = Avatar;
            lastClick = Time.time;
        }

        public void Load(Avatar avatar)
        {
            Avatar = avatar;

            VRMMetaObject meta;
            if (!avatar.Vrm)
            {
                var bundle = ApplicationState.AvatarBundles[avatar.Guid];
                meta = bundle.Object.GetComponent<VRMMeta>().Meta;
            }
            else
            {
                meta = ApplicationState.VrmData[avatar.Guid].ReadMeta(createThumbnail: true);
            }

            if (meta.Thumbnail != null)
            {
                var thumb = meta.Thumbnail;
                Thumbnail.sprite = Sprite.Create(thumb, new Rect(0, 0, thumb.width, thumb.height), new Vector2(0.5f, 0.5f), 100, 1, SpriteMeshType.FullRect);
                Thumbnail.GetComponent<AspectRatioFitter>().aspectRatio = (float)thumb.width / thumb.height;
            }

            Title.text = avatar.Title;
            Version.text = $"Version: {meta.Version}";
            Author.text = $"Author: {meta.Author}";
        }

        public override bool Equals(object obj)
        {
            if (obj is AvatarListItem other)
                return Avatar.Guid.Equals(other.Avatar.Guid);
            return false;
        }

        public override int GetHashCode()
        {
            return Avatar.Guid.GetHashCode();
        }

    }
}
