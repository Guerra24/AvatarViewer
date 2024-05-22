using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarViewer.Ui.Items
{
    public class RewardAssetListItem : MonoBehaviour
    {
        private string Bundle;

        [SerializeField] private TMP_Text Title;
        [SerializeField] private Button Remove;

        private void Awake()
        {
            Remove.onClick.AddListener(() =>
            {
                ApplicationPersistence.AppSettings.RewardBundles.Remove(Bundle);
                ApplicationState.RewardBundles[Bundle].Unload(true);
                ApplicationState.RewardBundles.Remove(Bundle);
                Destroy(gameObject);
            });
        }

        public void LoadValues(string bundle)
        {
            Bundle = bundle;
            Title.text = bundle;
        }
    }
}
