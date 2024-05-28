using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarViewer.UI.Items
{
    public abstract class RewardListItem<T> : MonoBehaviour where T : Reward
    {
        [HideInInspector]
        public string Id;
        [HideInInspector]
        public T Reward;

        [SerializeField]
        private TMP_Text Title;
        [SerializeField]
        private Image Image;

        protected virtual void Awake()
        {

        }

        public virtual void LoadValues(string id)
        {
            Id = id;
            Title.text = Reward.Title;
            Image.sprite = Reward.TwitchImage;
        }
    }
}
