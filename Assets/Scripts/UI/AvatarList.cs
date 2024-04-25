using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRM;

namespace AvatarViewer.Ui
{
    public class AvatarList : MonoBehaviour
    {

        public GameObject _template;

        // Start is called before the first frame update
        void Awake()
        {
            foreach (var avatar in ApplicationPersistence.AppSettings.Avatars)
            {
                CreateItem(avatar);
            }
        }

        public async void CreateItem(Avatar avatar)
        {
            var item = Instantiate(_template, this.transform, false);

            VRMMetaObject meta = null;

            if (!avatar.Vrm)
            {
                var bundle = ApplicationState.AvatarBundles[avatar.Guid];
                meta = bundle.Object.GetComponent<VRMMeta>().Meta;
            }
            else
            {
                using (var loader = new VRMImporterContext(ApplicationState.VrmData[avatar.Guid]))
                    meta = await loader.ReadMetaAsync(createThumbnail: true);
            }

            var thumb = meta.Thumbnail;

            item.GetComponent<AvatarListItem>().Avatar = avatar;

            item.transform.Find("Thumbnail").GetComponent<Image>().sprite = Sprite.Create(thumb, new Rect(0, 0, thumb.width, thumb.height), new Vector2(0.5f, 0.5f), 100, 1, SpriteMeshType.FullRect);

            item.transform.Find("Title").GetComponent<TMP_Text>().text = avatar.Title;
        }

        public void RemoveItem(Avatar avatar)
        {
            var items = GetComponentsInChildren<AvatarListItem>();

            Destroy(items.Where(i => i.Avatar.Equals(avatar)).First().gameObject);
        }

    }

}